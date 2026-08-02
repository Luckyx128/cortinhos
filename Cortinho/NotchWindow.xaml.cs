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

        // Largura base = só o slot do mic. Cada slot extra (Discord, notificações) visível soma mais espaço —
        // "34" é estimativa (slot 30 + gap 4) que pode precisar de ajuste visual fino.
        private const double BaseCompactWidth = 92;
        private const double ExtraSlotWidthCompact = 34;

        private enum NotchAnchor { Left, Center, Right }
        private NotchAnchor _anchor = NotchAnchor.Center;
        private bool _isTransitioning;
        private bool _isPeeking;
        private int _notificationCount;

        private enum StatusKind { None, Alert, Progress, Error }
        private StatusKind _statusKind = StatusKind.None;
        private DispatcherTimer? _statusAutoRecedeTimer;

        // Ícones duotone reais (assets/icons-export/notification.svg e x.svg) — 2 camadas cada.
        private const string AlertIconL1 = "M18.75 9v.704c0 .845.24 1.671.692 2.374l1.108 1.723c1.011 1.574.239 3.713-1.52 4.21a25.8 25.8 0 0 1-14.06 0c-1.759-.497-2.531-2.636-1.52-4.21l1.108-1.723a4.4 4.4 0 0 0 .693-2.374V9c0-3.866 3.022-7 6.749-7s6.75 3.134 6.75 7";
        private const string AlertIconL2 = "M7.243 18.545a5.002 5.002 0 0 0 9.513 0c-3.145.59-6.367.59-9.513 0";
        private const string ErrorIconL1 = "M22 12c0 5.523-4.477 10-10 10S2 17.523 2 12S6.477 2 12 2s10 4.477 10 10";
        private const string ErrorIconL2 = "M8.97 8.97a.75.75 0 0 1 1.06 0L12 10.94l1.97-1.97a.75.75 0 1 1 1.06 1.06L13.06 12l1.97 1.97a.75.75 0 0 1-1.06 1.06L12 13.06l-1.97 1.97a.75.75 0 0 1-1.06-1.06L10.94 12l-1.97-1.97a.75.75 0 0 1 0-1.06";

        private const double CompactHeight = 36;
        private const double ExpandedWidth = 400;
        private const double ExpandedHeight = 357;
        private bool _isExpanded;


        public NotchWindow()
        {
            InitializeComponent();
            Top = SystemParameters.WorkArea.Top;

            UpdateMicIcon();
            _ = InitializeNotificationsAsync();
            InitializePerf();
            InitializeShortcuts();
            InitializeDiscord();

            ShortcutLauncherService.LaunchFailed += msg =>
                Dispatcher.Invoke(() => ShowError(msg));

            Width = GetCompactTargetWidth();
            Left = GetLeftForAnchor(_anchor, Width);
        }

        // ---- Sistema de Status (alert/progress/error) — substitui a fileira de slots compacta.
        // Prioridade: error > alert > progress, um por vez (PHASE-1-NOTCH.md §2).

        public void ShowAlert(string label)
        {
            if (_statusKind == StatusKind.Error) return; // error tem prioridade, ignora

            SetStatus(StatusKind.Alert, label, AlertIconL1, 0.5, AlertIconL2, 1);

            _statusAutoRecedeTimer?.Stop();
            _statusAutoRecedeTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2.6) };
            _statusAutoRecedeTimer.Tick += (_, _) => ClearStatus();
            _statusAutoRecedeTimer.Start();
        }

        public void ShowError(string label)
        {
            _statusAutoRecedeTimer?.Stop(); // error nunca auto-esconde — só no clique (StatusLabel_Click)
            SetStatus(StatusKind.Error, label, ErrorIconL1, 0.5, ErrorIconL2, 1);
        }

        public void ShowProgress(double percent)
        {
            if (_statusKind is StatusKind.Error or StatusKind.Alert) return; // prioridade maior já ocupando

            _statusKind = StatusKind.Progress;
            SlotsRow.Visibility = Visibility.Collapsed;
            StatusLabel.Visibility = Visibility.Collapsed;
            StatusProgressBar.Visibility = Visibility.Visible;

            percent = Math.Clamp(percent, 0, 100);
            StatusProgressFillCol.Width = new GridLength(percent, GridUnitType.Star);
            StatusProgressEmptyCol.Width = new GridLength(100 - percent, GridUnitType.Star);

            if (percent >= 100) ClearStatus();
        }

        public void ClearStatus()
        {
            _statusAutoRecedeTimer?.Stop();
            _statusKind = StatusKind.None;
            SlotsRow.Visibility = Visibility.Visible;
            StatusLabel.Visibility = Visibility.Collapsed;
            StatusProgressBar.Visibility = Visibility.Collapsed;
        }

        private void SetStatus(StatusKind kind, string label, string iconL1, double iconL1Opacity, string iconL2, double iconL2Opacity)
        {
            _statusKind = kind;
            SlotsRow.Visibility = Visibility.Collapsed;
            StatusProgressBar.Visibility = Visibility.Collapsed;
            StatusLabel.Visibility = Visibility.Visible;
            StatusLabelText.Text = label;

            StatusIconPath.Data = Geometry.Parse(iconL1);
            StatusIconPath.Opacity = iconL1Opacity;
            StatusIconPathSecondary.Data = Geometry.Parse(iconL2);
            StatusIconPathSecondary.Opacity = iconL2Opacity;

            bool isError = kind == StatusKind.Error;
            StatusLabel.Background = ResourceBrush(isError ? "DangerSoftBrush" : "AccentSoftBrush");
            StatusLabel.BorderBrush = isError ? ResourceBrush("DangerBrush") : null;
            StatusLabel.BorderThickness = new Thickness(isError ? 1 : 0);

            var fg = ResourceBrush(isError ? "DangerBrush" : "TextPrimaryBrush");
            StatusIconPath.Fill = fg;
            StatusIconPathSecondary.Fill = fg;
            StatusLabelText.Foreground = fg;
        }

        private void StatusLabel_Click(object sender, MouseButtonEventArgs e)
        {
            if (_statusKind == StatusKind.Error)
                ClearStatus();
        }

        private double GetCompactTargetWidth()
        {
            double width = BaseCompactWidth;
            if (DiscordSlot.Visibility == Visibility.Visible) width += ExtraSlotWidthCompact;
            if (NotificationSlot.Visibility == Visibility.Visible) width += ExtraSlotWidthCompact;
            return width;
        }

        // Peek usa a mesma largura do expandido — assim clicar pra expandir só precisa crescer a altura,
        // sem salto de largura (antes, com Discord+notificação juntos, o peek passava dos 400px do expandido).
        private double GetPeekTargetWidth() => ExpandedWidth;

        /// <summary>Realinha Width/Left do estado compact/peek atual — chamado quando um slot aparece/some fora de um hover ativo.</summary>
        private void SyncCompactSize()
        {
            if (_isExpanded || _isTransitioning) return;
            double targetWidth = _isPeeking ? GetPeekTargetWidth() : GetCompactTargetWidth();
            Width = targetWidth;
            Left = GetLeftForAnchor(_anchor, targetWidth);
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

            UpdateDiscordSlot();
        }

        private void StartDiscordWatcher()
        {
            if (!_discordPresenceService.IsInitialized)
                _discordPresenceService.Initialize(_discordSettings.ClientId);

            if (_discordWatcher == null)
            {
                _discordWatcher = new GamePresenceWatcher(_discordPresenceService, () => ShortcutStore.Load());
                _discordWatcher.ActiveAppChanged += appName =>
                    Dispatcher.Invoke(() =>
                    {
                        DiscordValueText.Text = appName ?? "Nada rodando";
                        UpdateDiscordSlot();
                    });
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

            UpdateDiscordSlot();
        }

        /// <summary>Slot 2 da pill compacta/peek — glifo gamepad, só visível enquanto o tracking do Discord está ligado.</summary>
        private void UpdateDiscordSlot()
        {
            bool tracking = _discordSettings.Enabled;
            DiscordSlot.Visibility = tracking ? Visibility.Visible : Visibility.Collapsed;
            TopDiscordPill.Visibility = tracking ? Visibility.Visible : Visibility.Collapsed;

            if (tracking)
            {
                var accentBrush = ResourceBrush("AccentBrush");
                DiscordSlotBody.Fill = accentBrush;
                DiscordSlotDetail.Fill = accentBrush;

                string? app = _discordWatcher?.CurrentAppName;
                DiscordSlotUnderline.Visibility = Visibility.Visible;
                DiscordCaption.Text = app ?? "Discord";
                TopDiscordCaption.Text = app ?? "Discord";
            }
            else
            {
                DiscordCaption.Visibility = Visibility.Collapsed;
            }

            SyncCompactSize();
        }

        private void SetDiscordToggleVisual(bool on)
        {
            DiscordToggleTrack.Background = ResourceBrush(on ? "AccentBrush" : "Surface3Brush");
            DiscordToggleKnob.HorizontalAlignment = on ? System.Windows.HorizontalAlignment.Right : System.Windows.HorizontalAlignment.Left;
            DiscordToggleKnob.Margin = on ? new Thickness(0, 0, 2, 0) : new Thickness(2, 0, 0, 0);
        }

        /// <summary>Lê um brush de Theme/Tokens.xaml — centraliza cor em vez de hardcode espalhado no code-behind.</summary>
        private static System.Windows.Media.Brush ResourceBrush(string key) => (System.Windows.Media.Brush)System.Windows.Application.Current.FindResource(key);

        private sealed class ShortcutDisplayItem
        {
            public string Name { get; init; } = "";
            public string IconLayer1 { get; init; } = "";
            public double IconLayer1Opacity { get; init; } = 1;
            public string IconLayer2 { get; init; } = "";
            public double IconLayer2Opacity { get; init; } = 1;
            public ShortcutItem Source { get; init; } = null!;
        }

        private void InitializeShortcuts()
        {
            var pinned = ShortcutStore.Load().Where(s => s.Pinned).Take(5).ToList();
            if (pinned.Count == 0)
                return; // sem atalhos fixados — módulo fica oculto

            ShortcutsPanel.Visibility = Visibility.Visible;
            ShortcutsItems.ItemsSource = pinned.Select(s =>
            {
                var (l1, l1op, l2, l2op) = GetShortcutIconLayers(s.Type);
                return new ShortcutDisplayItem
                {
                    Name = s.Name,
                    IconLayer1 = l1,
                    IconLayer1Opacity = l1op,
                    IconLayer2 = l2,
                    IconLayer2Opacity = l2op,
                    Source = s
                };
            }).ToList();
        }

        // Ícones duotone reais de design_handoff_notch (assets/icons-export/*.svg) — 2 camadas por ícone
        // (uma em opacidade cheia, outra em .5), na mesma ordem de pintura do SVG original.
        private static (string L1, double L1Op, string L2, double L2Op) GetShortcutIconLayers(ShortcutType type) => type switch
        {
            ShortcutType.Application => (
                "M2 6.21c0-1.984 0-2.977.659-3.593S4.379 2 6.5 2s3.182 0 3.841.617C11 3.233 11 4.226 11 6.21v11.58c0 1.984 0 2.977-.659 3.593S8.621 22 6.5 22s-3.182 0-3.841-.617C2 20.767 2 19.774 2 17.79z", 0.5,
                "M13 15.4c0-2.074 0-3.111.659-3.756S15.379 11 17.5 11s3.182 0 3.841.644C22 12.29 22 13.326 22 15.4v2.2c0 2.074 0 3.111-.659 3.756S19.621 22 17.5 22s-3.182 0-3.841-.644C13 20.71 13 19.674 13 17.6zm0-9.9c0-1.087 0-1.63.171-2.06a2.3 2.3 0 0 1 1.218-1.262C14.802 2 15.327 2 16.375 2h2.25c1.048 0 1.573 0 1.986.178c.551.236.99.69 1.218 1.262c.171.43.171.973.171 2.06s0 1.63-.171 2.06a2.3 2.3 0 0 1-1.218 1.262C20.198 9 19.673 9 18.625 9h-2.25c-1.048 0-1.573 0-1.986-.178a2.3 2.3 0 0 1-1.218-1.262C13 7.13 13 6.587 13 5.5", 1),
            ShortcutType.Folder => (
                "M22 14v-2.202c0-2.632 0-3.949-.77-4.804a3 3 0 0 0-.224-.225C20.151 6 18.834 6 16.202 6h-.374c-1.153 0-1.73 0-2.268-.153a4 4 0 0 1-.848-.352C12.224 5.224 11.816 4.815 11 4l-.55-.55c-.274-.274-.41-.41-.554-.53a4 4 0 0 0-2.18-.903C7.53 2 7.336 2 6.95 2c-.883 0-1.324 0-1.692.07A4 4 0 0 0 2.07 5.257C2 5.626 2 6.068 2 6.95V14c0 3.771 0 5.657 1.172 6.828S6.229 22 10 22h4c3.771 0 5.657 0 6.828-1.172S22 17.771 22 14", 0.5,
                "M12.25 10a.75.75 0 0 1 .75-.75h5a.75.75 0 0 1 0 1.5h-5a.75.75 0 0 1-.75-.75", 1),
            ShortcutType.Url => (
                "M8 2.25A6.75 6.75 0 0 0 2.969 13.5a.75.75 0 0 0 1.118-1A5.25 5.25 0 0 1 8 3.75h4a5.25 5.25 0 1 1 0 10.5h-2a.75.75 0 0 0 0 1.5h2a6.75 6.75 0 0 0 0-13.5z", 1,
                "M6.75 15c0-2.9 2.35-5.25 5.25-5.25h2a.75.75 0 0 0 0-1.5h-2a6.75 6.75 0 0 0 0 13.5h4a6.75 6.75 0 0 0 5.031-11.25a.75.75 0 0 0-1.118 1A5.25 5.25 0 0 1 16 20.25h-4A5.25 5.25 0 0 1 6.75 15", 0.5),
            ShortcutType.Command => (
                "M2 12c0-4.714 0-7.071 1.464-8.536C4.93 2 7.286 2 12 2s7.071 0 8.535 1.464C22 4.93 22 7.286 22 12s0 7.071-1.465 8.535C19.072 22 16.714 22 12 22s-7.071 0-8.536-1.465C2 19.072 2 16.714 2 12", 0.5,
                "M13.488 6.446a.75.75 0 0 1 .53.918l-2.588 9.66a.75.75 0 0 1-1.449-.389l2.589-9.659a.75.75 0 0 1 .918-.53M14.97 8.47a.75.75 0 0 1 1.06 0l.209.208c.635.635 1.165 1.165 1.529 1.642c.384.504.654 1.036.654 1.68s-.27 1.176-.654 1.68c-.364.477-.894 1.007-1.53 1.642l-.208.208a.75.75 0 1 1-1.06-1.06l.171-.172c.682-.682 1.139-1.14 1.434-1.528c.283-.37.347-.586.347-.77s-.064-.4-.347-.77c-.295-.387-.752-.846-1.434-1.528l-.171-.172a.75.75 0 0 1 0-1.06m-7 0a.75.75 0 0 1 1.06 1.06l-.171.172c-.682.682-1.138 1.14-1.434 1.528c-.283.37-.346.586-.346.77s.063.4.346.77c.296.387.752.846 1.434 1.528l.172.172a.75.75 0 1 1-1.061 1.06l-.208-.208c-.636-.635-1.166-1.165-1.53-1.642c-.384-.504-.653-1.036-.653-1.68s.27-1.176.653-1.68c.364-.477.894-1.007 1.53-1.642z", 1),
            _ => ("M2 6.21c0-1.984 0-2.977.659-3.593S4.379 2 6.5 2s3.182 0 3.841.617C11 3.233 11 4.226 11 6.21v11.58c0 1.984 0 2.977-.659 3.593S8.621 22 6.5 22s-3.182 0-3.841-.617C2 20.767 2 19.774 2 17.79z", 0.5, "", 0)
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
            fill.Background = ResourceBrush(percent > 85 ? "DangerBrush" : "AccentBrush");
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
            int previousCount = _notificationCount;

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

            _notificationCount = items.Count;
            UpdateNotificationSlot();

            // Notificação nova de verdade (contagem subiu), não uma removida — vira alert transitório na pill.
            if (_notificationCount > previousCount && items.Count > 0)
                ShowAlert(items[0].Title);
        }

        /// <summary>Slot 3 da pill compacta/peek — badge de contagem, só visível com notificações pendentes e painel fechado.</summary>
        private void UpdateNotificationSlot()
        {
            bool show = _notificationCount > 0 && !_isExpanded;
            NotificationSlot.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            NotificationBadge.Visibility = show ? Visibility.Visible : Visibility.Collapsed;

            if (show)
            {
                NotificationBadgeText.Text = _notificationCount.ToString();
                NotificationCaption.Text = _notificationCount == 1 ? "1 notificação" : $"{_notificationCount} notificações";
            }
            else
            {
                NotificationCaption.Visibility = Visibility.Collapsed;
            }

            SyncCompactSize();
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
            MicToggleLabel.Foreground = ResourceBrush(isMuted ? "DangerBrush" : "TextPrimaryBrush");
            MicToggleState.Text = isMuted ? "OFF" : "ON";

            TopMicOnIcon.Visibility = isMuted ? Visibility.Collapsed : Visibility.Visible;
            TopMicOffIcon.Visibility = isMuted ? Visibility.Visible : Visibility.Collapsed;
            TopMicCaption.Text = isMuted ? "Mic mutado" : "Mic ativo";
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
            _isPeeking = true;
            MicCaption.Visibility = Visibility.Visible;
            if (DiscordSlot.Visibility == Visibility.Visible) DiscordCaption.Visibility = Visibility.Visible;
            if (NotificationSlot.Visibility == Visibility.Visible) NotificationCaption.Visibility = Visibility.Visible;
            AnimateWidth(GetPeekTargetWidth());
        }

        private void NotchRoot_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_isExpanded || _isTransitioning) return;
            _isPeeking = false;
            MicCaption.Visibility = Visibility.Collapsed;
            DiscordCaption.Visibility = Visibility.Collapsed;
            NotificationCaption.Visibility = Visibility.Collapsed;
            AnimateWidth(GetCompactTargetWidth());
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
            // Status "error" só sai no clique (StatusLabel_Click) — não deixa isso virar drag/toggle.
            if (_statusKind == StatusKind.Error) return;
            double startLeft = Left;
            double startTop = Top;

            _isPeeking = false;
            MicCaption.Visibility = Visibility.Collapsed;
            DiscordCaption.Visibility = Visibility.Collapsed;
            NotificationCaption.Visibility = Visibility.Collapsed;
            Width = GetCompactTargetWidth();

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
            {
                // Largura do clique-pra-expandir já é a mesma do expandido — deixa pronta aqui
                // pra ToggleExpanded() só precisar animar a altura, sem salto de largura.
                Width = GetPeekTargetWidth();
                Left = GetLeftForAnchor(_anchor, Width);
                ToggleExpanded();
            }
        }

        private void ToggleExpanded()
        {
            _isExpanded = !_isExpanded;
            _isTransitioning = true;

            UpdateNotificationSlot(); // badge só existe com o painel fechado — reavalia antes de medir a largura alvo

            double targetWidth = _isExpanded ? ExpandedWidth : GetCompactTargetWidth();
            double targetHeight = _isExpanded ? ExpandedHeight : CompactHeight;
            double targetLeft = GetLeftForAnchor(_anchor, targetWidth);

            CompactContent.Visibility = _isExpanded ? Visibility.Collapsed : Visibility.Visible;
            ExpandedContent.Visibility = _isExpanded ? Visibility.Visible : Visibility.Collapsed;

            if (_isExpanded)
                AnimateHeightOnly(targetHeight); // largura já é a do peek/expandido — só a altura cresce
            else
                AnimateSize(targetWidth, targetHeight, targetLeft); // recolher volta largura+altura junto

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

        private void AnimateHeightOnly(double targetHeight)
        {
            if (!SystemParameters.ClientAreaAnimation)
            {
                Height = targetHeight;
                return;
            }

            BeginAnimation(HeightProperty, null);
            var ease = new CubicEase { EasingMode = EasingMode.EaseOut };
            var duration = new Duration(TimeSpan.FromMilliseconds(220));
            BeginAnimation(HeightProperty, new DoubleAnimation(targetHeight, duration) { EasingFunction = ease });
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
            double compactWidth = GetCompactTargetWidth();
            var candidates = new (NotchAnchor Anchor, double Left)[]
            {
        (NotchAnchor.Left, workArea.Left + 12),
        (NotchAnchor.Center, workArea.Left + (workArea.Width - compactWidth) / 2),
        (NotchAnchor.Right, workArea.Right - compactWidth - 12),
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