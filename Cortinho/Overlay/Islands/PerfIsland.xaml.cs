using Cortinho.Services;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Cortinho.Overlay.Islands
{
    /// <summary>Ilha Desempenho (PHASE-OVERLAY.md §5.7) — CPU/GPU via PerfService próprio (instância
    /// separada da do NotchWindow: dois DispatcherTimers lendo o mesmo PerformanceCounter concorrentemente
    /// não é thread-safe, então cada superfície tem a sua). FPS fora de escopo — só dois cards.
    /// Start()/Stop() ligam a leitura só enquanto o overlay está aberto (mesma ideia do NotchWindow,
    /// que só atualiza enquanto visível).</summary>
    public partial class PerfIsland : System.Windows.Controls.UserControl
    {
        private readonly PerfService _perfService = new();
        private DispatcherTimer? _timer;
        private bool _updateInProgress;

        public PerfIsland()
        {
            InitializeComponent();
            Root.Visibility = _perfService.IsAvailable ? Visibility.Visible : Visibility.Collapsed;
        }

        public bool IsAvailable => _perfService.IsAvailable;

        public void Start()
        {
            if (!_perfService.IsAvailable) return;

            _timer ??= CreateTimer();
            _ = UpdateAsync();
            _timer.Start();
        }

        public void Stop() => _timer?.Stop();

        public void Dispose()
        {
            _timer?.Stop();
            _perfService.Dispose();
        }

        private DispatcherTimer CreateTimer()
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1.5) };
            timer.Tick += (_, _) => _ = UpdateAsync();
            return timer;
        }

        private async Task UpdateAsync()
        {
            if (_updateInProgress) return;
            _updateInProgress = true;
            try
            {
                var (cpu, gpu) = await Task.Run(() => (_perfService.GetCpuUsage(), _perfService.GetGpuUsage()));

                CpuValueText.Text = $"{cpu:F0}%";
                SetBar(CpuBarFillCol, CpuBarEmptyCol, CpuBarFill, CpuValueText, cpu);

                if (gpu is null)
                {
                    GpuValueText.Text = "—";
                    SetBar(GpuBarFillCol, GpuBarEmptyCol, GpuBarFill, GpuValueText, 0);
                }
                else
                {
                    GpuValueText.Text = $"{gpu.Value:F0}%";
                    SetBar(GpuBarFillCol, GpuBarEmptyCol, GpuBarFill, GpuValueText, gpu.Value);
                }
            }
            finally
            {
                _updateInProgress = false;
            }
        }

        private static void SetBar(ColumnDefinition fillCol, ColumnDefinition emptyCol, Border fill, TextBlock valueText, double percent)
        {
            percent = Math.Clamp(percent, 0, 100);
            fillCol.Width = new GridLength(percent, GridUnitType.Star);
            emptyCol.Width = new GridLength(100 - percent, GridUnitType.Star);

            var brush = (System.Windows.Media.Brush)System.Windows.Application.Current.FindResource(percent > 85 ? "DangerBrush" : "AccentBrush");
            fill.Background = brush;
            valueText.Foreground = percent > 85
                ? brush
                : (System.Windows.Media.Brush)System.Windows.Application.Current.FindResource("TextPrimaryBrush");
        }
    }
}
