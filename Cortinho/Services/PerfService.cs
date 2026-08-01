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

        /// <returns>null se a categoria "GPU Engine" não existir nessa máquina (sem provider) — módulo deve se esconder.</returns>
        public double? GetGpuUsage()
        {
            if (!_gpuCategoryAvailable) return null;

            try
            {
                var category = new PerformanceCounterCategory("GPU Engine");
                var currentInstances = new HashSet<string>(category.GetInstanceNames());

                foreach (var stale in _gpuCounters.Keys.Where(k => !currentInstances.Contains(k)).ToList())
                {
                    _gpuCounters[stale].Dispose();
                    _gpuCounters.Remove(stale);
                }

                var byEngineType = new Dictionary<string, double>();

                foreach (var instanceName in currentInstances)
                {
                    var idx = instanceName.IndexOf("engtype_", StringComparison.Ordinal);
                    if (idx < 0) continue;

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

                    double value;
                    try { value = counter.NextValue(); }
                    catch { continue; }

                    var engineType = instanceName[(idx + "engtype_".Length)..];
                    byEngineType[engineType] = byEngineType.GetValueOrDefault(engineType) + value;
                }

                return byEngineType.Count > 0 ? Math.Clamp(byEngineType.Values.Max(), 0, 100) : 0;
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
