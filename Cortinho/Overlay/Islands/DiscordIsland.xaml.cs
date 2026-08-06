using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Cortinho.Overlay.Islands
{
    /// <summary>Ilha Discord Rich Presence (PHASE-OVERLAY.md §5.6) — não abre uma segunda conexão RPC:
    /// lê/liga o mesmo DiscordPresenceService/GamePresenceWatcher que o NotchWindow já possui, exposto
    /// via NotchWindow.DiscordConfigured/DiscordTrackingEnabled/ToggleDiscordTracking/DiscordStateChanged.
    /// Some inteira se não houver Client ID configurado (mesmo critério do card no notch).</summary>
    public partial class DiscordIsland : System.Windows.Controls.UserControl
    {
        private readonly NotchWindow? _notch;
        private DispatcherTimer? _clockTimer;

        public DiscordIsland()
        {
            InitializeComponent();
            _notch = System.Windows.Application.Current.Windows.OfType<NotchWindow>().FirstOrDefault();
            if (_notch != null)
                _notch.DiscordStateChanged += OnDiscordStateChanged;
        }

        public bool IsAvailable => _notch?.DiscordConfigured == true;

        public void Refresh()
        {
            UpdateVisual();
            _clockTimer ??= CreateClockTimer();
            _clockTimer.Start();
        }

        public void Stop() => _clockTimer?.Stop();

        private DispatcherTimer CreateClockTimer()
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (_, _) => UpdateElapsedTime();
            return timer;
        }

        private void OnDiscordStateChanged() => Dispatcher.Invoke(UpdateVisual);

        private void Toggle_Click(object sender, MouseButtonEventArgs e) => _notch?.ToggleDiscordTracking();

        private void UpdateVisual()
        {
            bool enabled = _notch?.DiscordTrackingEnabled == true;

            ToggleTrack.Background = ResourceBrush(enabled ? "AccentBrush" : "Surface3Brush");
            ToggleKnob.HorizontalAlignment = enabled ? System.Windows.HorizontalAlignment.Right : System.Windows.HorizontalAlignment.Left;
            ToggleKnob.Margin = enabled ? new Thickness(0, 0, 2, 0) : new Thickness(2, 0, 0, 0);

            var glyphBrush = ResourceBrush(enabled ? "AccentBrush" : "TextTertiaryBrush");
            GamepadBody.Fill = glyphBrush;
            GamepadDetail.Fill = glyphBrush;

            string? appName = _notch?.DiscordCurrentAppName;
            AppNameText.Text = !enabled ? "Rastreio desativado" : appName ?? "Nada rodando";
            StatusDot.Background = ResourceBrush(enabled && appName != null ? "SuccessBrush" : "TextTertiaryBrush");

            UpdateElapsedTime();
        }

        private void UpdateElapsedTime()
        {
            var started = _notch?.DiscordCurrentAppStartedUtc;
            if (started is null)
            {
                TimeText.Text = "--:--";
                return;
            }

            var elapsed = DateTime.UtcNow - started.Value;
            TimeText.Text = elapsed.TotalHours >= 1
                ? elapsed.ToString(@"h\:mm\:ss")
                : elapsed.ToString(@"mm\:ss");
        }

        private static System.Windows.Media.Brush ResourceBrush(string key) => (System.Windows.Media.Brush)System.Windows.Application.Current.FindResource(key);
    }
}
