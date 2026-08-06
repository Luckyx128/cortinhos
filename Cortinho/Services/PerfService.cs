using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Cortinho.Services
{
    public sealed class PerfService : IDisposable
    {
        private readonly PerformanceCounter? _cpuCounter;
        private readonly bool _gpuCategoryAvailable;
        private readonly Dictionary<string, PerformanceCounter> _gpuCounters = new();

        public bool IsAvailable { get; }

        public PerfService()
        {
            try
            {
                _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                _cpuCounter.NextValue(); // primeira leitura de um contador de delta é sempre 0 — descarta
                IsAvailable = true;
            }
            catch
            {
                IsAvailable = false;
            }

            _gpuCategoryAvailable = PerformanceCounterCategory.Exists("GPU Engine");
        }

        public double GetCpuUsage()
        {
            try { return Math.Clamp(_cpuCounter?.NextValue() ?? 0, 0, 100); }
            catch { return 0; }
        }

        // Só o engine 3D. A categoria "GPU Engine" tem uma instância por (processo × engine) — nesta
        // máquina, 651 no total, e ler TODAS custa ~1s de CPU por ciclo (medido), o que a 1.5s de
        // intervalo queimava mais de meio núcleo continuamente. Filtrar por engtype_3D corta pra ~145
        // instâncias, e 3D é justamente o que interessa num overlay de jogo (era o grupo que dominava
        // o Max() de qualquer forma na prática). Vídeo/Copy/Compute deixam de contar.
        private const string EngineSuffix = "engtype_3D";

        /// <returns>null se a categoria "GPU Engine" não existir nessa máquina (sem provider) — módulo deve se esconder.</returns>
        public double? GetGpuUsage()
        {
            if (!_gpuCategoryAvailable) return null;

            try
            {
                var category = new PerformanceCounterCategory("GPU Engine");
                var currentInstances = new HashSet<string>(
                    category.GetInstanceNames().Where(n => n.EndsWith(EngineSuffix, StringComparison.Ordinal)));

                foreach (var stale in _gpuCounters.Keys.Where(k => !currentInstances.Contains(k)).ToList())
                {
                    _gpuCounters[stale].Dispose();
                    _gpuCounters.Remove(stale);
                }

                double total = 0;

                foreach (var instanceName in currentInstances)
                {
                    if (!_gpuCounters.TryGetValue(instanceName, out var counter))
                    {
                        try
                        {
                            counter = new PerformanceCounter("GPU Engine", "Utilization Percentage", instanceName, true);
                            counter.NextValue(); // descarta a primeira leitura dessa instância nova
                            _gpuCounters[instanceName] = counter;
                        }
                        catch { /* instância pode ter sumido entre o GetInstanceNames e agora */ }
                        continue; // sem valor confiável ainda nesta rodada pra essa instância
                    }

                    try { total += counter.NextValue(); }
                    catch { continue; }
                }

                return Math.Clamp(total, 0, 100);
            }
            catch
            {
                return null;
            }
        }

        public void Dispose()
        {
            _cpuCounter?.Dispose();
            foreach (var counter in _gpuCounters.Values)
                counter.Dispose();
            _gpuCounters.Clear();
        }
    }
}
