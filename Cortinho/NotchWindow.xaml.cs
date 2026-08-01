using Cortinho.Models;
using Cortinho.Native;
using Cortinho.Services;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Linq;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Cortinho
{
    public partial class NotchWindow : Window
    {
        private readonly MicService _micService = new();
        private readonly NotificationService _notificationService = new();
        private readonly PerfService _perfService = new();
        private readonly DiscordPresenceService _discordPresenceService = new();
        private GamePresenceWatcher? _discordWatcher;
        private DiscordSettings _discordSettings = new();
        private GlobalInputWatcher? _inputWatcher;
        private HwndSource? _hwndSource;
        private DispatcherTimer? _autoCollapseTimer;
        private DispatcherTimer? _perfTimer;
        private const int HOTKEY_ID_MUTE = 9000;

        private const double CompactWidth = 92;
        private const double PeekWidth = 232;

        private enum NotchAnchor { Left, Center, Right }
        private NotchAnchor _anchor = NotchAnchor.Center;
        private bool _isTransitioning;

        private const double CompactHeight = 36;
        private const double ExpandedWidth = 400;
        private const double ExpandedHeight = 357;
        private bool _isExpanded;


        public NotchWindow()
        {
            InitializeComponent();
            Top = SystemParameters.WorkArea.Top;
            Left = (SystemParameters.WorkArea.Width - Width) / 2;

            UpdateMicIcon();
            _ = InitializeNotificationsAsync();
            InitializePerf();
            InitializeShortcuts();
            InitializeDiscord();
        }

        private void InitializeDiscord()
        {
            _discordSettings = DiscordSettingsStore.Load();
            if (string.IsNullOrWhiteSpace(_discordSettings.ClientId))
                return; // sem Client ID configurado — sem provider, módulo fica oculto

            DiscordPanel.Visibility = Visibility.Visible;
            SetDiscordToggleVisual(_discordSettings.Enabled);

            if (_discordSettings.Enabled)
                StartDiscordWatcher();
            else
                DiscordValueText.Text = "Desativado";
        }

        private void StartDiscordWatcher()
        {
            if (!_discordPresenceService.IsInitialized)
                _discordPresenceService.Initialize(_discordSettings.ClientId);

            if (_discordWatcher == null)
            {
                _discordWatcher = new GamePresenceWatcher(_discordPresenceService, () => ShortcutStore.Load());
                _discordWatcher.ActiveAppChanged += appName =>
                    Dispatcher.Invoke(() => DiscordValueText.Text = appName ?? "Nada rodando");
            }

            _discordWatcher.Start();
            DiscordValueText.Text = "Nada rodando";
        }

        private void StopDiscordWatcher()
        {
            _discordWatcher?.Stop();
            _discordPresenceService.ClearPresence();
            DiscordValueText.Text = "Desativado";
        }

        private void DiscordToggle_Click(object sender, MouseButtonEventArgs e)
        {
            _discordSettings.Enabled = !_discordSettings.Enabled;
            DiscordSettingsStore.Save(_discordSettings);
            SetDiscordToggleVisual(_discordSettings.Enabled);

            if (_discordSettings.Enabled)
                StartDiscordWatcher();
            else
                StopDiscordWatcher();
        }

        private void SetDiscordToggleVisual(bool on)
        {
            DiscordToggleTrack.Background = new SolidColorBrush(on
                ? System.Windows.Media.Color.FromRgb(0x6E, 0xA8, 0xFE)
                : System.Windows.Media.Color.FromRgb(0x3A, 0x3A, 0x3A));
            DiscordToggleKnob.HorizontalAlignment = on ? System.Windows.HorizontalAlignment.Right : System.Windows.HorizontalAlignment.Left;
            DiscordToggleKnob.Margin = on ? new Thickness(0, 0, 2, 0) : new Thickness(2, 0, 0, 0);
        }

        private sealed class ShortcutDisplayItem
        {
            public string Name { get; init; } = "";
            public string IconGeometry { get; init; } = "";
            public ShortcutItem Source { get; init; } = null!;
        }

        private void InitializeShortcuts()
        {
            var pinned = ShortcutStore.Load().Where(s => s.Pinned).Take(5).ToList();
            if (pinned.Count == 0)
                return; // sem atalhos fixados — módulo fica oculto

            ShortcutsPanel.Visibility = Visibility.Visible;
            ShortcutsItems.ItemsSource = pinned.Select(s => new ShortcutDisplayItem
            {
                Name = s.Name,
                IconGeometry = GetShortcutIconGeometry(s.Type),
                Source = s
            }).ToList();
        }

        private static string GetShortcutIconGeometry(ShortcutType type) => type switch
        {
            ShortcutType.Application => "M4 5h16v14H4Z M4 9h16",
            ShortcutType.Folder => "M3 6.5A2.5 2.5 0 0 1 5.5 4h3.6l2 2H18.5A2.5 2.5 0 0 1 21 8.5v8A2.5 2.5 0 0 1 18.5 19h-13A2.5 2.5 0 0 1 3 16.5Z",
            ShortcutType.Url => "M9 15l6-6 M8.5 8.5l-1.8 1.8a3 3 0 0 0 4.2 4.2l1-1 M15.5 15.5l1.8-1.8a3 3 0 0 0-4.2-4.2l-1 1",
            ShortcutType.Command => "M4 5h16v14H4Z M7.5 9.5l3 2.5-3 2.5 M13 15h4",
            _ => "M4 5h16v14H4Z"
        };

        private void ShortcutButton_Click(object sender, MouseButtonEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is ShortcutDisplayItem item)
                ShortcutLauncherService.Launch(item.Source);
        }

        private void InitializePerf()
        {
            if (!_perfService.IsAvailable)
                return; // sem provider de CPU — esconde o módulo inteiro, igual ao spec pede

            PerfPanel.Visibility = Visibility.Visible;

            _perfTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1.5) };
            _perfTimer.Tick += (_, _) => _ = UpdatePerfAsync();
            _perfTimer.Start();
        }

        private bool _perfUpdateInProgress;

        private async Task UpdatePerfAsync()
        {
            if (_perfUpdateInProgress) return; // tick anterior ainda rodando — pula essa rodada em vez de sobrepor
            _perfUpdateInProgress = true;
            try
            {
                // Enumerar/criar PerformanceCounter (principalmente os de GPU) pode levar dezenas de ms —
                // rodar isso na UI thread trava o hook global de mouse (mesma thread), então roda em background.
                var (cpu, gpu) = await Task.Run(() => (_perfService.GetCpuUsage(), _perfService.GetGpuUsage()));

                CpuValueText.Text = $"{cpu:F0}%";
                SetPerfBar(CpuBarFillCol, CpuBarEmptyCol, CpuBarFill, cpu);

                if (gpu is null)
                {
                    GpuValueText.Text = "—";
                    SetPerfBar(GpuBarFillCol, GpuBarEmptyCol, GpuBarFill, 0);
                }
                else
                {
                    GpuValueText.Text = $"{gpu.Value:F0}%";
                    SetPerfBar(GpuBarFillCol, GpuBarEmptyCol, GpuBarFill, gpu.Value);
                }
            }
            finally
            {
                _perfUpdateInProgress = false;
            }
        }

        private static void SetPerfBar(ColumnDefinition fillCol, ColumnDefinition emptyCol, Border fill, double percent)
        {
            percent = Math.Clamp(percent, 0, 100);
            fillCol.Width = new GridLength(percent, GridUnitType.Star);
            emptyCol.Width = new GridLength(100 - percent, GridUnitType.Star);
            fill.Background = new SolidColorBrush(percent > 85
                ? System.Windows.Media.Color.FromRgb(0xF0, 0x50, 0x3C)
                : System.Windows.Media.Color.FromRgb(0x6E, 0xA8, 0xFE));
        }

        private sealed class NotificationDisplayItem
        {
            public string Title { get; init; } = "";
            public string Body { get; init; } = "";
            public string TimeLabel { get; init; } = "";
            public BitmapImage? Icon { get; init; }
        }

        private async Task InitializeNotificationsAsync()
        {
            bool available = await _notificationService.InitializeAsync();
            if (!available) return;

            _notificationService.NotificationsChanged += (_, _) =>
                Dispatcher.Invoke(() => _ = RefreshNotificationsAsync());

            NotificationsPanel.Visibility = Visibility.Visible;
            await RefreshNotificationsAsync();
        }

        private async Task RefreshNotificationsAsync()
        {
            var items = await _notificationService.GetNotificationsAsync();

            var displayItems = new List<NotificationDisplayItem>();
            foreach (var item in items.Take(3))
            {
                displayItems.Add(new NotificationDisplayItem
                {
                    Title = item.Title,
                    Body = item.Body,
                    TimeLabel = item.Time.ToLocalTime().ToString("HH:mm"),
                    Icon = item.LogoRef != null ? await LoadIconAsync(item.LogoRef) : null
                });
            }

            NotificationsItems.ItemsSource = displayItems;
            NoNotificationsLabel.Visibility = displayItems.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private static async Task<BitmapImage?> LoadIconAsync(global::Windows.Storage.Streams.RandomAccessStreamReference streamRef)
        {
            try
            {
                using var stream = await streamRef.OpenReadAsync();
                var reader = new global::Windows.Storage.Streams.DataReader(stream);
                await reader.LoadAsync((uint)stream.Size);
                var bytes = new byte[stream.Size];
                reader.ReadBytes(bytes);

                using var ms = new System.IO.MemoryStream(bytes);
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = ms;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        private void ClearNotifications_Click(object sender, MouseButtonEventArgs e)
        {
            _notificationService.ClearAll();
            _ = RefreshNotificationsAsync();
        }
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == NativeMethods.WM_HOTKEY && wParam.ToInt32() == HOTKEY_ID_MUTE)
            {
                _micService.ToggleMute();
                UpdateMicIcon();
                PlayMicSound(_micService.IsMuted);
                handled = true;
            }
            return IntPtr.Zero;
        }

        protected override void OnClosed(EventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            NativeMethods.UnregisterHotKey(hwnd, HOTKEY_ID_MUTE);
            _hwndSource?.RemoveHook(WndProc);
            _inputWatcher?.Dispose();
            _perfTimer?.Stop();
            _perfService.Dispose();
            _discordWatcher?.Dispose();
            _discordPresenceService.Dispose();
            base.OnClosed(e);
        }

        private void UpdateMicIcon()
        {
            bool isMuted = _micService.IsMuted;

            MicOnIcon.Visibility = isMuted ? Visibility.Collapsed : Visibility.Visible;
            MicOffIcon.Visibility = isMuted ? Visibility.Visible : Visibility.Collapsed;
            MicCaption.Text = isMuted ? "Mic mutado" : "Mic ativo";
            MicUnderline.Visibility = isMuted ? Visibility.Visible : Visibility.Collapsed;

            MicToggleOnIcon.Visibility = isMuted ? Visibility.Collapsed : Visibility.Visible;
            MicToggleOffIcon.Visibility = isMuted ? Visibility.Visible : Visibility.Collapsed;
            MicToggleLabel.Text = isMuted ? "Ativar mic" : "Mutar mic";
            MicToggleLabel.Foreground = new SolidColorBrush(isMuted
                ? System.Windows.Media.Color.FromRgb(0xF0, 0x50, 0x3C)
                : System.Windows.Media.Color.FromRgb(0xEC, 0xEE, 0xF2));
            MicToggleState.Text = isMuted ? "OFF" : "ON";
        }

        private void PlayMicSound(bool isMuted)
        {
            if (isMuted)
                System.Media.SystemSounds.Hand.Play();
            else
                System.Media.SystemSounds.Asterisk.Play();

        }


        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var hwnd = new WindowInteropHelper(this).Handle;
            int currentStyle = NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE);
            NativeMethods.SetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE, currentStyle | NativeMethods.WS_EX_NOACTIVATE | NativeMethods.WS_EX_TOOLWINDOW);
            _hwndSource = HwndSource.FromHwnd(hwnd);
            _hwndSource?.AddHook(WndProc);

            NativeMethods.RegisterHotKey(hwnd, HOTKEY_ID_MUTE, NativeMethods.MOD_CONTROL | NativeMethods.MOD_ALT, NativeMethods.VK_M);

            _inputWatcher = new GlobalInputWatcher();
            _inputWatcher.MouseDown += OnGlobalMouseDown;
            _inputWatcher.EscapePressed += OnGlobalEscapePressed;
        }

        private void OnGlobalMouseDown(System.Windows.Point screenPoint)
        {
            if (!_isExpanded || _isTransitioning) return;
            var windowBounds = new Rect(Left, Top, Width, Height);
            if (!windowBounds.Contains(screenPoint))
                ToggleExpanded();
        }

        private void OnGlobalEscapePressed()
        {
            if (_isExpanded && !_isTransitioning)
                ToggleExpanded();
        }

        private void NotchRoot_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_isExpanded || _isTransitioning) return;
            MicCaption.Visibility = Visibility.Visible;
            AnimateWidth(PeekWidth);
        }

        private void NotchRoot_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_isExpanded || _isTransitioning) return;
            MicCaption.Visibility = Visibility.Collapsed;
            AnimateWidth(CompactWidth);
        }

        private void AnimateWidth(double targetWidth)
        {
            double targetLeft = GetLeftForAnchor(_anchor, targetWidth);

            if (!SystemParameters.ClientAreaAnimation)
            {
                Width = targetWidth;
                Left = targetLeft;
                return;
            }

            var ease = new BackEase { Amplitude = 0.35, EasingMode = EasingMode.EaseOut };
            var duration = new Duration(TimeSpan.FromMilliseconds(220));

            BeginAnimation(WidthProperty, new DoubleAnimation(targetWidth, duration) { EasingFunction = ease });
            BeginAnimation(LeftProperty, new DoubleAnimation(targetLeft, duration) { EasingFunction = ease });
        }

        private void NotchRoot_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Expandido não arrasta nem fecha por clique interno — fechar já é coberto por clicar fora,
            // Esc ou o timer de auto-collapse. Controles internos (mic, atalhos, notificações) tratam o próprio clique.
            if (_isExpanded) return;
            if (_isTransitioning) return;
            double startLeft = Left;
            double startTop = Top;

            MicCaption.Visibility = Visibility.Collapsed;
            if (!_isExpanded) Width = CompactWidth;

            Cursor = System.Windows.Input.Cursors.SizeAll;
            NotchScale.ScaleX = 1.03;
            NotchScale.ScaleY = 1.03;
            NotchShadow.BlurRadius = 40;
            NotchShadow.ShadowDepth = 14;
            NotchShadow.Opacity = 0.5;

            BeginAnimation(LeftProperty, null);
            BeginAnimation(TopProperty, null);

            DragMove();

            Cursor = System.Windows.Input.Cursors.Arrow;
            NotchScale.ScaleX = 1;
            NotchScale.ScaleY = 1;
            NotchShadow.BlurRadius = 16;
            NotchShadow.ShadowDepth = 2;
            NotchShadow.Opacity = 0.34;

            bool wasDrag = Math.Abs(Left - startLeft) > 2 || Math.Abs(Top - startTop) > 2;

            if (wasDrag)
                SnapToNearestAnchor();
            else
                ToggleExpanded();
        }

        private void ToggleExpanded()
        {
            _isExpanded = !_isExpanded;
            _isTransitioning = true;

            double targetWidth = _isExpanded ? ExpandedWidth : CompactWidth;
            double targetHeight = _isExpanded ? ExpandedHeight : CompactHeight;
            double targetLeft = GetLeftForAnchor(_anchor, targetWidth);

            CompactContent.Visibility = _isExpanded ? Visibility.Collapsed : Visibility.Visible;
            ExpandedContent.Visibility = _isExpanded ? Visibility.Visible : Visibility.Collapsed;

            AnimateSize(targetWidth, targetHeight, targetLeft);

            if (_isExpanded)
                ResetAutoCollapseTimer();
            else
                _autoCollapseTimer?.Stop();

            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(240) };
            timer.Tick += (_, _) =>
            {
                _isTransitioning = false;
                timer.Stop();
            };
            timer.Start();
        }

        private void AnimateSize(double targetWidth, double targetHeight, double targetLeft)
        {
            if (!SystemParameters.ClientAreaAnimation)
            {
                Width = targetWidth;
                Height = targetHeight;
                Left = targetLeft;
                return;
            }

            BeginAnimation(WidthProperty, null);
            BeginAnimation(HeightProperty, null);

            var ease = new CubicEase { EasingMode = EasingMode.EaseOut };
            var duration = new Duration(TimeSpan.FromMilliseconds(220));

            BeginAnimation(WidthProperty, new DoubleAnimation(targetWidth, duration) { EasingFunction = ease });
            BeginAnimation(HeightProperty, new DoubleAnimation(targetHeight, duration) { EasingFunction = ease });
            BeginAnimation(LeftProperty, new DoubleAnimation(targetLeft, duration) { EasingFunction = ease });
        }

        private void MicToggleButton_Click(object sender, MouseButtonEventArgs e)
        {
            _micService.ToggleMute();
            UpdateMicIcon();
            PlayMicSound(_micService.IsMuted);
        }


        private void SnapToNearestAnchor()
        {
            var workArea = SystemParameters.WorkArea;
            var candidates = new (NotchAnchor Anchor, double Left)[]
            {
        (NotchAnchor.Left, workArea.Left + 12),
        (NotchAnchor.Center, workArea.Left + (workArea.Width - CompactWidth) / 2),
        (NotchAnchor.Right, workArea.Right - CompactWidth - 12),
            };

            var closest = candidates.OrderBy(c => Math.Abs(c.Left - Left)).First();
            _anchor = closest.Anchor;

            var ease = new BackEase { Amplitude = 0.35, EasingMode = EasingMode.EaseOut };
            var duration = new Duration(TimeSpan.FromMilliseconds(220));

            BeginAnimation(LeftProperty, new DoubleAnimation(closest.Left, duration) { EasingFunction = ease });
            BeginAnimation(TopProperty, new DoubleAnimation(workArea.Top, duration) { EasingFunction = ease });
        }

        private double GetLeftForAnchor(NotchAnchor anchor, double width)
        {
            var workArea = SystemParameters.WorkArea;
            return anchor switch
            {
                NotchAnchor.Left => workArea.Left + 12,
                NotchAnchor.Right => workArea.Right - width - 12,
                _ => workArea.Left + (workArea.Width - width) / 2,
            };
        }

        private void ResetAutoCollapseTimer()
        {
            if (_autoCollapseTimer == null)
            {
                _autoCollapseTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3.2) };
                _autoCollapseTimer.Tick += (_, _) =>
                {
                    _autoCollapseTimer!.Stop();
                    if (_isExpanded && !_isTransitioning)
                        ToggleExpanded();
                };
            }
            _autoCollapseTimer.Stop();
            _autoCollapseTimer.Start();
        }

        private void NotchRoot_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_isExpanded) ResetAutoCollapseTimer();
        }
    }
}