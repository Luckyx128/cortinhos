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
using ContextMenu = System.Windows.Controls.ContextMenu;
using MenuItem = System.Windows.Controls.MenuItem;
using Brush = System.Windows.Media.Brush;
using FontFamily = System.Windows.Media.FontFamily;

namespace Cortinho
{
    public partial class NotchWindow : Window
    {
        public static NotchWindow? Current => System.Windows.Application.Current?.Windows.OfType<NotchWindow>().FirstOrDefault();

        private readonly MicService _micService = new();
        private readonly NotificationService _notificationService = new();
        private readonly DiscordPresenceService _discordPresenceService = new();
        private GamePresenceWatcher? _discordWatcher;
        private DiscordSettings _discordSettings = new();
        private GlobalInputWatcher? _inputWatcher;
        private HwndSource? _hwndSource;
        private DispatcherTimer? _autoCollapseTimer;
        private ContextMenu? _micPickerMenu;
        private const int HOTKEY_ID_MUTE = 9000;
        private const int HOTKEY_ID_TOGGLE_NOTCH = 9001;

        // Auto-Hide no Desktop: recolhe em uma linha minimalista/discreta no topo (2-3px)
        private const double AutoHidePeekHeight = 3.0;
        private bool _isAutoHide;
        private bool _isManuallyHidden;
        private bool _isRevealedForOsd;
        private DispatcherTimer? _autoHideTimer;
        private DispatcherTimer? _edgeWatcherTimer;

        // Ícones de Mic para OSD
        private const string MicOnIconPath = "M17 7v4a5 5 0 0 1-10 0V7a5 5 0 0 1 10 0Z M17 7h-3m3 4h-3m6 0a8 8 0 0 1-8 8m0 0a8 8 0 0 1-8-8m8 8v3m0 0h3m-3 0H9";
        private const string MicOffIconPath = "m2 2l20 20M4 11a8 8 0 0 0 8 8m0 0c1.954 0 3.745-.7 5.135-1.865M12 19v3m0 0h3m-3 0H9m11-11c0 1.651-.5 3.186-1.358 4.46m-1.634-8.464c0-2.761-2.239-4.98-5-4.98c-1.869 0-3.47.965-4.328 2.484m9.328 2.496l-3.028.012m3.028-.012v4.008m-10-4.008v4.02a5 5 0 0 0 5 5c1.135 0 2.165-.39 3.004-1.028m1.435-1.728c.358-.69.56-1.413.56-2.244v-.012m-2.824 0h2.825";
        private const string SuccessIconPath = "M4 12.5 L9.5 17.5 L20 6.5";

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out NativeMethods.POINT lpPoint);

        // Largura base = só o slot do mic. Cada slot extra (Discord, notificações) visível soma mais espaço —
        // "34" é estimativa (slot 30 + gap 4) que pode precisar de ajuste visual fino.
        private const double BaseCompactWidth = 92;
        private const double ExtraSlotWidthCompact = 34;

        private enum NotchAnchor { Left, Center, Right }
        private NotchAnchor _anchor = NotchAnchor.Center;
        private bool _isTransitioning;
        private bool _isPeeking;
        private int _notificationCount;

        /// <summary>Exposto pra ilha Discord do overlay (design_handoff_overlay §5.6) reaproveitar o mesmo
        /// _discordPresenceService/_discordWatcher em vez de abrir uma segunda conexão RPC com o Discord.</summary>
        public bool DiscordConfigured => !string.IsNullOrWhiteSpace(_discordSettings.ClientId);
        public bool DiscordTrackingEnabled => _discordSettings.Enabled;
        public string? DiscordCurrentAppName => _discordWatcher?.CurrentAppName;
        public DateTime? DiscordCurrentAppStartedUtc => _discordWatcher?.CurrentAppStartedUtc;
        public event Action? DiscordStateChanged;

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
        private const double ExpandedHeight = 304;
        private bool _isExpanded;

        private const double IdentityWidth = 96; // logo 22 + gap 8 + ponto 7 + padding — calibrar visualmente
        private const double IdentityPeekWidth = 200; // + legenda "Overlay ativo" no hover — estimativa, calibrar visualmente
        private const double IdentityHeight = 48;
        private bool _isOverlayOpen;


        public NotchWindow()
        {
            InitializeComponent();
            LoadPlacementSettings();
            Top = GetTargetWorkArea().Top;

            _micService.MuteChanged += OnMicMuteChanged;
            MicService.DeviceChanged += OnGlobalMicDeviceChanged;
            UpdateMicIcon();
            UpdateActiveMicDeviceText();
            _ = InitializeNotificationsAsync();
            InitializeShortcuts();
            InitializeDiscord();

            ShortcutLauncherService.LaunchFailed += msg =>
                Dispatcher.Invoke(() => ShowError(msg));

            NotificationIpcService.NotificationReceived += OnIpcNotificationReceived;

            Width = GetCompactTargetWidth();
            Left = GetLeftForAnchor(_anchor, Width);

            // Inicia em modo auto-hide inteligente no desktop para não cobrir abas de navegadores e títulos
            CollapseToAutoHide(immediate: true);
        }

        private double GetNormalTop() => GetTargetWorkArea().Top;
        private double GetAutoHideTop() => GetTargetWorkArea().Top - (CompactHeight - AutoHidePeekHeight);

        private void AnimateTop(double targetTop, double durationMs = 200)
        {
            if (!SystemParameters.ClientAreaAnimation)
            {
                Top = targetTop;
                return;
            }

            BeginAnimation(TopProperty, null);
            var ease = new CubicEase { EasingMode = EasingMode.EaseOut };
            var animation = new DoubleAnimation(targetTop, new Duration(TimeSpan.FromMilliseconds(durationMs)))
            {
                EasingFunction = ease
            };
            BeginAnimation(TopProperty, animation);
        }

        private void AnimateOpacity(double targetOpacity, double durationMs = 180)
        {
            if (!SystemParameters.ClientAreaAnimation)
            {
                Opacity = targetOpacity;
                return;
            }

            BeginAnimation(OpacityProperty, null);
            var animation = new DoubleAnimation(targetOpacity, new Duration(TimeSpan.FromMilliseconds(durationMs)));
            BeginAnimation(OpacityProperty, animation);
        }

        private void AnimateFadeOut()
        {
            AnimateTop(GetAutoHideTop() - 20, 200);
            AnimateOpacity(0, 180);
        }

        private void RevealFromAutoHide(bool forOsd = false)
        {
            if (_isManuallyHidden && !forOsd) return;
            if (_isExpanded || _isOverlayOpen) return;

            _autoHideTimer?.Stop();
            _isAutoHide = false;

            if (Opacity < 1)
            {
                AnimateOpacity(1, 160);
            }

            AnimateTop(GetNormalTop(), 200);

            var duration = new Duration(TimeSpan.FromMilliseconds(180));
            CompactContent.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(1.0, duration));
            AutoHideIndicator.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(0.0, duration));
        }

        private void CollapseToAutoHide(bool immediate = false)
        {
            if (_isExpanded || _isOverlayOpen || _isTransitioning || _isManuallyHidden || _statusKind != StatusKind.None) return;
            if (NotchRoot.IsMouseOver && !immediate) return;

            _isAutoHide = true;
            double targetTop = GetAutoHideTop();

            if (immediate || !SystemParameters.ClientAreaAnimation)
            {
                BeginAnimation(TopProperty, null);
                Top = targetTop;
                CompactContent.Opacity = 0;
                AutoHideIndicator.Opacity = 0.8;
                return;
            }

            AnimateTop(targetTop, 220);

            var duration = new Duration(TimeSpan.FromMilliseconds(200));
            CompactContent.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(0.0, duration));
            AutoHideIndicator.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(0.8, duration));
        }

        private void ResetAutoHideTimer(double seconds = 1.2)
        {
            if (_isExpanded || _isOverlayOpen || _isManuallyHidden || _statusKind != StatusKind.None) return;

            if (_autoHideTimer == null)
            {
                _autoHideTimer = new DispatcherTimer();
                _autoHideTimer.Tick += (_, _) =>
                {
                    _autoHideTimer.Stop();
                    if (!NotchRoot.IsMouseOver && !_isExpanded && !_isOverlayOpen && _statusKind == StatusKind.None)
                    {
                        CollapseToAutoHide();
                    }
                };
            }
            _autoHideTimer.Stop();
            _autoHideTimer.Interval = TimeSpan.FromSeconds(seconds);
            _autoHideTimer.Start();
        }

        private void InitializeAutoHideWatcher()
        {
            _edgeWatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
            _edgeWatcherTimer.Tick += (_, _) => CheckMouseEdgeProximity();
            _edgeWatcherTimer.Start();
        }

        private void CheckMouseEdgeProximity()
        {
            if (!IsLoaded || _isTransitioning || _isExpanded || _isOverlayOpen) return;
            if (_isManuallyHidden && !_isRevealedForOsd) return;

            if (!GetCursorPos(out var pt)) return;

            var dpi = VisualTreeHelper.GetDpi(this);
            double cursorX = pt.X / dpi.DpiScaleX;
            double cursorY = pt.Y / dpi.DpiScaleY;

            var workArea = GetTargetWorkArea();

            // Detecta aproximação na borda superior da tela onde a Notch está ancorada (0-10px)
            bool inTopTrigger = cursorY >= workArea.Top && cursorY <= (workArea.Top + 10) &&
                               cursorX >= (Left - 30) && cursorX <= (Left + Width + 30);

            // Ou quando o cursor está fisicamente sobre o retângulo visível da Notch
            bool overWindow = cursorX >= Left && cursorX <= (Left + Width) &&
                              cursorY >= Top && cursorY <= (Top + Height);

            bool isHovering = inTopTrigger || overWindow || NotchRoot.IsMouseOver;

            if (isHovering)
            {
                _autoHideTimer?.Stop();
                if (_isAutoHide)
                {
                    RevealFromAutoHide();
                }
            }
            else
            {
                if (!_isAutoHide && _statusKind == StatusKind.None && !_isRevealedForOsd && (_autoHideTimer == null || !_autoHideTimer.IsEnabled))
                {
                    ResetAutoHideTimer(1.2);
                }
            }
        }

        public void ToggleNotchVisibility()
        {
            if (_isManuallyHidden)
            {
                _isManuallyHidden = false;
                AnimateOpacity(1, 160);
                RevealFromAutoHide();
                ResetAutoHideTimer(3.5);
            }
            else
            {
                _isManuallyHidden = true;
                _autoHideTimer?.Stop();

                if (_isExpanded)
                {
                    ToggleExpanded();
                }

                AnimateFadeOut();
            }
        }

        private void OnMicMuteChanged(bool isMuted)
        {
            Dispatcher.Invoke(() =>
            {
                UpdateMicIcon();
                PlayMicSound(isMuted);
                if (!_isExpanded)
                {
                    ShowMicAlert(isMuted);
                }
            });
        }

        // ---- Sistema de Status e OSD (alert/progress/error/mic)
        // Prioridade: error > alert > progress, um por vez (PHASE-1-NOTCH.md §2).

        public void ShowMicAlert(bool isMuted)
        {
            if (_statusKind == StatusKind.Error || _isExpanded) return;

            string label = isMuted ? "Microfone mutado" : "Microfone ativo";
            string icon = isMuted ? MicOffIconPath : MicOnIconPath;
            var fg = ResourceBrush(isMuted ? "DangerBrush" : "SuccessBrush");
            var bg = isMuted
                ? ResourceBrush("DangerSoftBrush")
                : new SolidColorBrush(System.Windows.Media.Color.FromArgb(0x26, 0x22, 0xC5, 0x5E));

            SetStatus(StatusKind.Alert, label, icon, 1.0, "", 0.0, fg, bg, isStroke: true);

            _isRevealedForOsd = true;
            RevealFromAutoHide(forOsd: true);

            double targetWidth = 176;
            AnimateWidth(targetWidth);

            StartStatusAutoRecede(2.4);
        }

        private void StartStatusAutoRecede(double seconds)
        {
            _statusAutoRecedeTimer?.Stop();
            _statusAutoRecedeTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(seconds > 0 ? seconds : 3.5) };
            _statusAutoRecedeTimer.Tick += (_, _) =>
            {
                _statusAutoRecedeTimer?.Stop();
                ClearStatus();
                _isRevealedForOsd = false;
                SyncCompactSize();

                if (_isManuallyHidden)
                {
                    AnimateFadeOut();
                }
                else if (!NotchRoot.IsMouseOver)
                {
                    ResetAutoHideTimer(0.8);
                }
            };
            _statusAutoRecedeTimer.Start();
        }

        public void ShowAlert(string label)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => ShowAlert(label));
                return;
            }

            if (_statusKind == StatusKind.Error || _isExpanded) return; // error tem prioridade, ignora

            SetStatus(StatusKind.Alert, label, AlertIconL1, 0.5, AlertIconL2, 1);

            _isRevealedForOsd = true;
            RevealFromAutoHide(forOsd: true);

            double targetWidth = Math.Max(GetCompactTargetWidth(), Math.Min(320, Math.Max(160, label.Length * 7.5 + 46)));
            AnimateWidth(targetWidth);

            StartStatusAutoRecede(2.6);
        }

        public void ShowNotificationAlert(string label, string kind, double durationSeconds)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => ShowNotificationAlert(label, kind, durationSeconds));
                return;
            }

            // Regra do Design System: se a Notch estiver expandida (_isExpanded == true), NÃO encolher a largura de 400px!
            if (_isExpanded) return;

            if (_statusKind == StatusKind.Error && !kind.Equals("error", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (kind.Equals("success", StringComparison.OrdinalIgnoreCase))
            {
                var fg = ResourceBrush("SuccessBrush");
                var scb = fg as SolidColorBrush;
                var bg = scb != null
                    ? new SolidColorBrush(System.Windows.Media.Color.FromArgb(0x26, scb.Color.R, scb.Color.G, scb.Color.B))
                    : new SolidColorBrush(System.Windows.Media.Color.FromArgb(0x26, 0x22, 0xC5, 0x5E));

                SetStatus(StatusKind.Alert, label, SuccessIconPath, 1.0, "", 0.0, fg, bg, isStroke: true);
            }
            else if (kind.Equals("error", StringComparison.OrdinalIgnoreCase))
            {
                var fg = ResourceBrush("DangerBrush");
                var bg = ResourceBrush("DangerSoftBrush");
                SetStatus(StatusKind.Error, label, ErrorIconL1, 0.5, ErrorIconL2, 1, fg, bg, isStroke: false);
            }
            else
            {
                var fg = ResourceBrush("AccentBrush");
                var bg = ResourceBrush("AccentSoftBrush");
                SetStatus(StatusKind.Alert, label, AlertIconL1, 0.5, AlertIconL2, 1, fg, bg, isStroke: false);
            }

            _isRevealedForOsd = true;
            RevealFromAutoHide(forOsd: true);

            double targetWidth = Math.Max(GetCompactTargetWidth(), Math.Min(320, Math.Max(160, label.Length * 7.5 + 46)));
            AnimateWidth(targetWidth);

            StartStatusAutoRecede(durationSeconds);
        }

        public void ShowError(string label)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => ShowError(label));
                return;
            }

            // Regra do Design System: se a Notch estiver expandida (_isExpanded == true), NÃO encolher a largura de 400px!
            if (_isExpanded) return;

            _statusAutoRecedeTimer?.Stop(); // error nunca auto-esconde — só no clique (StatusLabel_Click)
            SetStatus(StatusKind.Error, label, ErrorIconL1, 0.5, ErrorIconL2, 1);
            _isRevealedForOsd = true;
            RevealFromAutoHide(forOsd: true);
            double targetWidth = Math.Max(GetCompactTargetWidth(), Math.Min(320, Math.Max(160, label.Length * 7.5 + 46)));
            AnimateWidth(targetWidth);
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

        private void SetStatus(StatusKind kind, string label, string iconL1, double iconL1Opacity, string iconL2, double iconL2Opacity,
            System.Windows.Media.Brush? customFg = null, System.Windows.Media.Brush? customBg = null, bool isStroke = false)
        {
            _statusKind = kind;
            SlotsRow.Visibility = Visibility.Collapsed;
            StatusProgressBar.Visibility = Visibility.Collapsed;
            StatusLabel.Visibility = Visibility.Visible;
            StatusLabelText.Text = label;

            StatusIconPath.Data = string.IsNullOrEmpty(iconL1) ? null : Geometry.Parse(iconL1);
            StatusIconPath.Opacity = iconL1Opacity;
            StatusIconPathSecondary.Data = string.IsNullOrEmpty(iconL2) ? null : Geometry.Parse(iconL2);
            StatusIconPathSecondary.Opacity = iconL2Opacity;

            bool isError = kind == StatusKind.Error;
            StatusLabel.Background = customBg ?? ResourceBrush(isError ? "DangerSoftBrush" : "AccentSoftBrush");
            StatusLabel.BorderBrush = isError ? ResourceBrush("DangerBrush") : null;
            StatusLabel.BorderThickness = new Thickness(isError ? 1 : 0);

            var fg = customFg ?? ResourceBrush(isError ? "DangerBrush" : "TextPrimaryBrush");
            if (isStroke)
            {
                StatusIconPath.Fill = null;
                StatusIconPath.Stroke = fg;
                StatusIconPath.StrokeThickness = 1.5;
                StatusIconPath.StrokeStartLineCap = PenLineCap.Round;
                StatusIconPath.StrokeEndLineCap = PenLineCap.Round;
                StatusIconPathSecondary.Fill = null;
                StatusIconPathSecondary.Stroke = fg;
                StatusIconPathSecondary.StrokeThickness = 1.5;
            }
            else
            {
                StatusIconPath.Stroke = null;
                StatusIconPath.Fill = fg;
                StatusIconPathSecondary.Stroke = null;
                StatusIconPathSecondary.Fill = fg;
            }
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
            if (_isExpanded || _isTransitioning || _isOverlayOpen) return;
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
                        DiscordStateChanged?.Invoke();
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

        private void DiscordToggle_Click(object sender, MouseButtonEventArgs e) => ToggleDiscordTracking();

        public void ToggleDiscordTracking()
        {
            _discordSettings.Enabled = !_discordSettings.Enabled;
            DiscordSettingsStore.Save(_discordSettings);
            SetDiscordToggleVisual(_discordSettings.Enabled);

            if (_discordSettings.Enabled)
                StartDiscordWatcher();
            else
                StopDiscordWatcher();

            UpdateDiscordSlot();
            DiscordStateChanged?.Invoke();
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
                var (l1, l1op, l2, l2op) = ShortcutIconProvider.GetIconLayers(s.Type);
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

        private void ShortcutButton_Click(object sender, MouseButtonEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is ShortcutDisplayItem item)
                ShortcutLauncherService.Launch(item.Source);
        }

        // Módulo de desempenho (CPU/GPU) removido daqui de propósito: o DispatcherTimer rodava desde o
        // startup até o fim do app, mesmo com a pill recolhida e ninguém vendo os números, e cada ciclo
        // lia centenas de contadores PDH "GPU Engine" (~1s de CPU por leitura, a cada 1.5s → mais de
        // meio núcleo queimado continuamente, medido). Agora o perf vive só no PerfIsland do overlay,
        // que só liga o timer enquanto o overlay está aberto. Ver Overlay/Islands/PerfIsland.xaml.cs.

        private sealed class NotificationDisplayItem
        {
            public string Title { get; init; } = "";
            public string Body { get; init; } = "";
            public string TimeLabel { get; init; } = "";
            public BitmapImage? Icon { get; init; }
            public string? VectorIcon { get; init; }
            public Brush? VectorBrush { get; init; }
        }

        private readonly List<NotificationDisplayItem> _customNotificationHistory = new();
        private List<NotificationDisplayItem> _lastSystemNotificationItems = new();

        private void OnIpcNotificationReceived(NotificationPayload payload)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => OnIpcNotificationReceived(payload));
                return;
            }

            // Tocar feedback sonoro suave do sistema (ex: SystemSounds.Asterisk)
            try
            {
                if (payload.Kind.Equals("error", StringComparison.OrdinalIgnoreCase))
                    System.Media.SystemSounds.Hand.Play();
                else
                    System.Media.SystemSounds.Asterisk.Play();
            }
            catch { }

            // Adicionar o item recebido à lista do NotificationsPanel da Notch para que o usuário possa visualizá-lo no histórico
            AddNotificationToHistory(payload);

            // Respeitar rigorosamente a regra do Design System: se a Notch estiver expandida (_isExpanded == true), NÃO encolher a largura de 400px!
            if (_isExpanded)
            {
                return;
            }

            // Se a Notch estiver recolhida/em auto-hide: revelar suavemente, permanecer pelo tempo especificado em payload.DurationSeconds (padrão 3.5s) e recolher automaticamente
            double duration = payload.DurationSeconds ?? 3.5;

            if (payload.Kind.Equals("error", StringComparison.OrdinalIgnoreCase))
            {
                ShowError(payload.Message);
                if (payload.DurationSeconds.HasValue && payload.DurationSeconds.Value > 0)
                {
                    StartStatusAutoRecede(payload.DurationSeconds.Value);
                }
            }
            else if (payload.Kind.Equals("progress", StringComparison.OrdinalIgnoreCase) && payload.Progress.HasValue)
            {
                ShowProgress(payload.Progress.Value);
            }
            else
            {
                ShowNotificationAlert(payload.Message, payload.Kind, duration);
            }
        }

        private void AddNotificationToHistory(NotificationPayload payload)
        {
            string title = !string.IsNullOrWhiteSpace(payload.Title)
                ? payload.Title
                : (!string.IsNullOrWhiteSpace(payload.Source) ? payload.Source : (payload.Kind.Equals("error", StringComparison.OrdinalIgnoreCase) ? "Erro" : "Notificação"));

            string vectorIcon;
            Brush vectorBrush;

            if (payload.Kind.Equals("error", StringComparison.OrdinalIgnoreCase))
            {
                vectorIcon = "M18 6L6 18M6 6l12 12";
                vectorBrush = ResourceBrush("DangerBrush");
            }
            else if (payload.Kind.Equals("success", StringComparison.OrdinalIgnoreCase))
            {
                vectorIcon = SuccessIconPath;
                vectorBrush = ResourceBrush("SuccessBrush");
            }
            else
            {
                vectorIcon = "M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9 M13.73 21a2 2 0 0 1-3.46 0";
                vectorBrush = ResourceBrush("AccentBrush");
            }

            var item = new NotificationDisplayItem
            {
                Title = title,
                Body = payload.Message,
                TimeLabel = DateTime.Now.ToString("HH:mm"),
                VectorIcon = vectorIcon,
                VectorBrush = vectorBrush
            };

            _customNotificationHistory.Insert(0, item);
            if (_customNotificationHistory.Count > 20)
            {
                _customNotificationHistory.RemoveAt(_customNotificationHistory.Count - 1);
            }

            UpdateNotificationsDisplay();
        }

        private void UpdateNotificationsDisplay(List<NotificationDisplayItem>? systemItems = null)
        {
            if (systemItems != null)
            {
                _lastSystemNotificationItems = systemItems;
            }

            var combined = new List<NotificationDisplayItem>();
            combined.AddRange(_customNotificationHistory);
            combined.AddRange(_lastSystemNotificationItems);

            var displayItems = combined.Take(3).ToList();
            NotificationsItems.ItemsSource = displayItems;
            NoNotificationsLabel.Visibility = displayItems.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

            _notificationCount = combined.Count;
            UpdateNotificationSlot();

            NotificationsPanel.Visibility = Visibility.Visible;
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

            var systemItems = new List<NotificationDisplayItem>();
            foreach (var item in items.Take(5))
            {
                systemItems.Add(new NotificationDisplayItem
                {
                    Title = item.Title,
                    Body = item.Body,
                    TimeLabel = item.Time.ToLocalTime().ToString("HH:mm"),
                    Icon = item.LogoRef != null ? await LoadIconAsync(item.LogoRef) : null
                });
            }

            UpdateNotificationsDisplay(systemItems);

            // Notificação nova de verdade (contagem subiu), não uma removida — vira alert transitório na pill.
            if (_notificationCount > previousCount && items.Count > 0 && !_isExpanded)
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
            _customNotificationHistory.Clear();
            _notificationService.ClearAll();
            _ = RefreshNotificationsAsync();
        }
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == NativeMethods.WM_HOTKEY)
            {
                int id = wParam.ToInt32();
                if (id == HOTKEY_ID_MUTE)
                {
                    _micService.ToggleMute();
                    handled = true;
                }
                else if (id == HOTKEY_ID_TOGGLE_NOTCH)
                {
                    ToggleNotchVisibility();
                    handled = true;
                }
            }
            return IntPtr.Zero;
        }

        protected override void OnClosed(EventArgs e)
        {
            NotificationIpcService.NotificationReceived -= OnIpcNotificationReceived;
            var hwnd = new WindowInteropHelper(this).Handle;
            NativeMethods.UnregisterHotKey(hwnd, HOTKEY_ID_MUTE);
            NativeMethods.UnregisterHotKey(hwnd, HOTKEY_ID_TOGGLE_NOTCH);
            _hwndSource?.RemoveHook(WndProc);
            _inputWatcher?.Dispose();
            _discordWatcher?.Dispose();
            _discordPresenceService.Dispose();
            _micService.MuteChanged -= OnMicMuteChanged;
            MicService.DeviceChanged -= OnGlobalMicDeviceChanged;
            _micService.Dispose();
            _edgeWatcherTimer?.Stop();
            _autoHideTimer?.Stop();
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

            if (_isOverlayOpen) UpdateIdentityStatusDot();
        }

        /// <summary>Pílula "identidade" (PHASE-OVERLAY.md §4) — a NotchWindow encolhe pra isso enquanto o
        /// overlay está aberto, já que as ilhas (mic, Discord, contagem) dizem o que a pílula normalmente diria.
        /// Ligado pelo OverlayController.Open()/Close() via lookup lazy, mesmo padrão do DiscordIsland.Notch.</summary>
        public void SetOverlayOpen(bool open)
        {
            if (_isOverlayOpen == open) return;
            _isOverlayOpen = open;

            if (open)
            {
                _autoCollapseTimer?.Stop();
                _autoHideTimer?.Stop();
                _isAutoHide = false;
                _isExpanded = false;
                _isPeeking = false;
                CompactContent.Visibility = Visibility.Collapsed;
                ExpandedContent.Visibility = Visibility.Collapsed;
                IdentityContent.Visibility = Visibility.Visible;
                AutoHideIndicator.Opacity = 0;
                UpdateIdentityStatusDot();
                Top = GetNormalTop();
                AnimateSize(IdentityWidth, IdentityHeight, GetLeftForAnchor(_anchor, IdentityWidth));
            }
            else
            {
                IdentityContent.Visibility = Visibility.Collapsed;
                CompactContent.Visibility = Visibility.Visible;
                CompactContent.Opacity = 1;
                double width = GetCompactTargetWidth();
                AnimateSize(width, CompactHeight, GetLeftForAnchor(_anchor, width));
                ResetAutoHideTimer(2.0);
            }
        }

        private void UpdateIdentityStatusDot() =>
            IdentityStatusDot.Fill = ResourceBrush(_micService.IsMuted ? "WarningBrush" : "SuccessBrush");

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
            NativeMethods.RegisterHotKey(hwnd, HOTKEY_ID_TOGGLE_NOTCH, NativeMethods.MOD_CONTROL | NativeMethods.MOD_ALT, 0x48); // Ctrl + Alt + H

            _inputWatcher = new GlobalInputWatcher();
            _inputWatcher.MouseDown += OnGlobalMouseDown;
            _inputWatcher.EscapePressed += OnGlobalEscapePressed;

            InitializeAutoHideWatcher();
        }

        private void OnGlobalMouseDown(System.Windows.Point screenPoint)
        {
            if (_micPickerMenu?.IsOpen == true) return;
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
            if (_isOverlayOpen) { IdentityCaption.Visibility = Visibility.Visible; AnimateWidth(IdentityPeekWidth); return; }
            if (_isExpanded || _isTransitioning) return;

            _autoHideTimer?.Stop();
            if (_isAutoHide)
            {
                RevealFromAutoHide();
            }

            _isPeeking = true;
            MicCaption.Visibility = Visibility.Visible;
            if (DiscordSlot.Visibility == Visibility.Visible) DiscordCaption.Visibility = Visibility.Visible;
            if (NotificationSlot.Visibility == Visibility.Visible) NotificationCaption.Visibility = Visibility.Visible;
            AnimateWidth(GetPeekTargetWidth());
        }

        private void NotchRoot_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_isOverlayOpen) { IdentityCaption.Visibility = Visibility.Collapsed; AnimateWidth(IdentityWidth); return; }
            if (_isExpanded || _isTransitioning) return;
            _isPeeking = false;
            MicCaption.Visibility = Visibility.Collapsed;
            DiscordCaption.Visibility = Visibility.Collapsed;
            NotificationCaption.Visibility = Visibility.Collapsed;
            AnimateWidth(GetCompactTargetWidth());

            if (!_isAutoHide && _statusKind == StatusKind.None)
            {
                ResetAutoHideTimer(1.2);
            }
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
            if (_isOverlayOpen) return;
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
            double targetHeight = _isExpanded ? GetExpandedTargetHeight() : CompactHeight;
            double targetLeft = GetLeftForAnchor(_anchor, targetWidth);

            CompactContent.Visibility = _isExpanded ? Visibility.Collapsed : Visibility.Visible;
            ExpandedContent.Visibility = _isExpanded ? Visibility.Visible : Visibility.Collapsed;

            if (_isExpanded)
            {
                _autoHideTimer?.Stop();
                _isAutoHide = false;
                AutoHideIndicator.Opacity = 0;
                Top = GetNormalTop();
                AnimateHeightOnly(targetHeight); // largura já é a do peek/expandido — só a altura cresce
                ResetAutoCollapseTimer();
            }
            else
            {
                CompactContent.Opacity = 1;
                AnimateSize(targetWidth, targetHeight, targetLeft); // recolher volta largura+altura junto
                _autoCollapseTimer?.Stop();
                ResetAutoHideTimer(1.5);
            }

            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(240) };
            timer.Tick += (_, _) =>
            {
                _isTransitioning = false;
                timer.Stop();
            };
            timer.Start();
        }

        private double GetExpandedTargetHeight()
        {
            // Altura base com margens internas e TopStatusRow
            double height = 260;

            // Se o DiscordPanel estiver visível, precisa de mais espaço vertical para acomodá-lo
            if (DiscordPanel.Visibility == Visibility.Visible)
            {
                height += 56;
            }

            // Se existirem atalhos rápidos ou notificações ocupando altura vertical
            if (ShortcutsPanel.Visibility == Visibility.Visible)
            {
                height += 24;
            }

            return Math.Max(ExpandedHeight, height);
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
        }

        private void OnGlobalMicDeviceChanged(string deviceId)
        {
            Dispatcher.Invoke(UpdateActiveMicDeviceText);
        }

        private void CompactMicSlot_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            OpenMicPicker(CompactMicSlot);
        }

        private void MicToggleButton_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            OpenMicPicker(MicToggleButton);
        }

        private void MicDeviceSelector_Click(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            OpenMicPicker(MicDeviceSelectorButton);
        }

        private void MicDeviceSelector_RightClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            OpenMicPicker(MicDeviceSelectorButton);
        }

        public void OpenMicPicker(FrameworkElement target)
        {
            _autoCollapseTimer?.Stop();
            _autoHideTimer?.Stop();

            var menu = CreateMicPickerMenu((selectedId, selectedName) =>
            {
                SelectMicrophone(selectedId, selectedName);
            });

            menu.PlacementTarget = target;
            menu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            menu.Closed += (_, _) =>
            {
                _micPickerMenu = null;
                if (_isExpanded)
                    ResetAutoCollapseTimer();
                else if (!NotchRoot.IsMouseOver)
                    ResetAutoHideTimer(1.2);
            };

            _micPickerMenu = menu;
            menu.IsOpen = true;
        }

        public void SelectMicrophone(string selectedId, string selectedName)
        {
            var s = AppSettingsStore.Load();
            s.MicDeviceId = selectedId;
            AppSettingsStore.Save(s);
            MicService.NotifyGlobalDeviceChanged(selectedId);
            ShowAlert($"Mic: {selectedName}");
            UpdateActiveMicDeviceText();
        }

        public void UpdateActiveMicDeviceText()
        {
            var settings = AppSettingsStore.Load();
            string currentId = settings.MicDeviceId ?? "";
            string displayName = "Padrão do sistema";

            if (!string.IsNullOrEmpty(currentId))
            {
                var devices = MicService.GetCaptureDevices();
                var match = devices.FirstOrDefault(d => d.Id == currentId);
                if (!string.IsNullOrEmpty(match.Name))
                {
                    displayName = match.Name;
                }
            }

            MicActiveDeviceText.Text = displayName;
            MicActiveDeviceText.ToolTip = string.IsNullOrEmpty(currentId) && !string.IsNullOrEmpty(_micService.DeviceName)
                ? $"Padrão do sistema ({_micService.DeviceName})"
                : displayName;
        }

        public static ContextMenu CreateMicPickerMenu(Action<string, string> onDeviceSelected)
        {
            var menu = new ContextMenu
            {
                Style = (Style)System.Windows.Application.Current.FindResource("FormContextMenuStyle")
            };

            menu.Items.Add(new TextBlock
            {
                Text = "SELECIONAR MICROFONE",
                FontFamily = (FontFamily)System.Windows.Application.Current.FindResource("FontMono"),
                FontSize = 9.5,
                FontWeight = FontWeights.SemiBold,
                Foreground = (Brush)System.Windows.Application.Current.FindResource("TextTertiaryBrush"),
                Margin = new Thickness(10, 4, 10, 6)
            });

            var settings = AppSettingsStore.Load();
            string configuredId = settings.MicDeviceId ?? "";

            var devices = MicService.GetCaptureDevices().ToList();
            bool isDefaultActive = string.IsNullOrEmpty(configuredId) || devices.All(d => d.Id != configuredId);

            var menuItemStyle = (Style)System.Windows.Application.Current.FindResource("FormMenuItemStyle");

            // Item: Padrão do sistema
            var defaultItem = new MenuItem
            {
                Header = new TextBlock
                {
                    Text = "Padrão do sistema",
                    VerticalAlignment = VerticalAlignment.Center,
                    TextTrimming = TextTrimming.CharacterEllipsis,
                    MaxWidth = 260
                },
                Style = menuItemStyle,
                Icon = isDefaultActive ? CreateCheckmarkIcon() : CreateEmptyIcon(),
                ToolTip = "Usar microfone padrão configurado no Windows"
            };
            defaultItem.Click += (_, _) => onDeviceSelected("", "Padrão do sistema");
            menu.Items.Add(defaultItem);

            if (devices.Count > 0)
            {
                menu.Items.Add(new Separator
                {
                    Background = (Brush)System.Windows.Application.Current.FindResource("BorderSubtleBrush"),
                    Height = 1,
                    Margin = new Thickness(6, 4, 6, 4)
                });

                foreach (var device in devices)
                {
                    bool isSelected = !isDefaultActive && device.Id == configuredId;
                    var item = new MenuItem
                    {
                        Header = new TextBlock
                        {
                            Text = device.Name,
                            VerticalAlignment = VerticalAlignment.Center,
                            TextTrimming = TextTrimming.CharacterEllipsis,
                            MaxWidth = 260
                        },
                        Style = menuItemStyle,
                        Icon = isSelected ? CreateCheckmarkIcon() : CreateEmptyIcon(),
                        ToolTip = device.Name
                    };
                    string devId = device.Id;
                    string devName = device.Name;
                    item.Click += (_, _) => onDeviceSelected(devId, devName);
                    menu.Items.Add(item);
                }
            }

            return menu;
        }

        private static FrameworkElement CreateCheckmarkIcon()
        {
            var path = new Path
            {
                Data = Geometry.Parse("M3 7.5 L6.5 11 L12 4"),
                Stroke = (Brush)System.Windows.Application.Current.FindResource("AccentBrush"),
                StrokeThickness = 2,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeLineJoin = PenLineJoin.Round
            };
            var canvas = new Canvas { Width = 15, Height = 15 };
            canvas.Children.Add(path);
            return canvas;
        }

        private static FrameworkElement CreateEmptyIcon()
        {
            return new Canvas { Width = 15, Height = 15 };
        }


        private void SnapToNearestAnchor()
        {
            var workArea = GetTargetWorkArea();
            double compactWidth = GetCompactTargetWidth();
            var candidates = new (NotchAnchor Anchor, double Left)[]
            {
                (NotchAnchor.Left, workArea.Left + 12),
                (NotchAnchor.Center, workArea.Left + (workArea.Width - compactWidth) / 2),
                (NotchAnchor.Right, workArea.Right - compactWidth - 12),
            };

            var closest = candidates.OrderBy(c => Math.Abs(c.Left - Left)).First();
            _anchor = closest.Anchor;
            SaveAnchorSetting(closest.Anchor);

            var ease = new BackEase { Amplitude = 0.35, EasingMode = EasingMode.EaseOut };
            var duration = new Duration(TimeSpan.FromMilliseconds(220));

            BeginAnimation(LeftProperty, new DoubleAnimation(closest.Left, duration) { EasingFunction = ease });
            BeginAnimation(TopProperty, new DoubleAnimation(workArea.Top, duration) { EasingFunction = ease });
            _isAutoHide = false;
            ResetAutoHideTimer(2.0);
        }

        private double GetLeftForAnchor(NotchAnchor anchor, double width)
        {
            var workArea = GetTargetWorkArea();
            return anchor switch
            {
                NotchAnchor.Left => workArea.Left + 12,
                NotchAnchor.Right => workArea.Right - width - 12,
                _ => workArea.Left + (workArea.Width - width) / 2,
            };
        }

        // ---- Âncora e monitor (Config, fase 5) — NotchWindow não tem WS_EX_NOACTIVATE-related
        // restriction pra isso, é só matemática de posicionamento; ver MainWindow.xaml.cs (ConfigTab).

        private void LoadPlacementSettings()
        {
            var settings = AppSettingsStore.Load();
            _anchor = settings.NotchAnchor switch
            {
                "Left" => NotchAnchor.Left,
                "Right" => NotchAnchor.Right,
                _ => NotchAnchor.Center,
            };
        }

        private static void SaveAnchorSetting(NotchAnchor anchor)
        {
            var settings = AppSettingsStore.Load();
            settings.NotchAnchor = anchor.ToString();
            AppSettingsStore.Save(settings);
        }

        /// <summary>Converte o WorkingArea (px físicos) do monitor escolhido em Config pra DIPs.
        /// Assume a mesma escala de DPI do monitor primário pra todos — cobre o caso comum (DPI uniforme
        /// entre monitores); setups com DPI por-monitor diferente podem posicionar levemente errado.</summary>
        private static System.Windows.Rect GetTargetWorkArea()
        {
            var settings = AppSettingsStore.Load();
            if (string.IsNullOrEmpty(settings.NotchMonitorDeviceName))
                return SystemParameters.WorkArea;

            var screen = System.Windows.Forms.Screen.AllScreens.FirstOrDefault(s => s.DeviceName == settings.NotchMonitorDeviceName);
            var primary = System.Windows.Forms.Screen.PrimaryScreen;
            if (screen == null || primary == null) return SystemParameters.WorkArea;

            double scale = SystemParameters.WorkArea.Width / primary.WorkingArea.Width;
            var wa = screen.WorkingArea;
            return new System.Windows.Rect(wa.X * scale, wa.Y * scale, wa.Width * scale, wa.Height * scale);
        }

        /// <summary>Chamado pela Config (MainWindow) depois de salvar uma nova âncora/monitor —
        /// reposiciona sem esperar o próximo drag/hover.</summary>
        public void ApplyPlacementSettings()
        {
            LoadPlacementSettings();
            if (_isExpanded || _isTransitioning) return;
            Top = _isAutoHide ? GetAutoHideTop() : GetNormalTop();
            SyncCompactSize();
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