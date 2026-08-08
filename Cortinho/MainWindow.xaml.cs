using Cortinho.Models;
using Cortinho.Native;
using Cortinho.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace Cortinho
{
    public partial class MainWindow : Window
    {
        private const int HOTKEY_ID_LAUNCHER = 9001;

        private IntPtr _hwnd;
        private HwndSource? _hwndSource;
        private bool _reallyClosing;
        private List<ShortcutItem> _shortcuts = new();
        private DispatcherTimer? _statusResetTimer;

        /// <summary>design_handoff_overlay — setado de fora (App.xaml.cs) logo após a construção.
        /// Desde a fase 5 é ele que o Ctrl+Alt+L abre; esta janela virou a superfície de gerenciamento,
        /// alcançável só pela bandeja e pela ilha de engrenagem.</summary>
        public Overlay.OverlayController? Overlay { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            // Cria o HWND sem mostrar a janela — precisamos dele já no construtor pra registrar a
            // hotkey global Ctrl+Alt+L mesmo com o launcher ainda escondido (App.xaml.cs cria isso oculto).
            new WindowInteropHelper(this).EnsureHandle();

            AtalhosTab.IsChecked = true;
            LoadShortcuts();
            LoadConfig();

            Services.ThemeService.ThemeChanged += OnThemeChanged;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            _hwnd = new WindowInteropHelper(this).Handle;
            _hwndSource = HwndSource.FromHwnd(_hwnd);
            _hwndSource?.AddHook(WndProc);

            var settings = AppSettingsStore.Load();
            NativeMethods.RegisterHotKey(_hwnd, HOTKEY_ID_LAUNCHER, settings.LauncherHotkeyModifiers, settings.LauncherHotkeyKey);

            ApplyMicaBackdrop();
        }

        /// <summary>DWMWA_USE_IMMERSIVE_DARK_MODE decide o tom da Mica real (área central da janela,
        /// que fica transparente de propósito pra deixar a Mica aparecer) — nada a ver com os
        /// ResourceDictionary da app, então trocar tema sem reaplicar isso deixa o meio da janela sempre
        /// escuro mesmo no tema claro. Só reafirma o flag; não recria o backdrop inteiro.</summary>
        private void OnThemeChanged(string theme)
        {
            if (_hwnd == IntPtr.Zero) return;
            try
            {
                int dark = theme == "light" ? 0 : 1;
                NativeMethods.DwmSetWindowAttribute(_hwnd, NativeMethods.DWMWA_USE_IMMERSIVE_DARK_MODE, ref dark, sizeof(int));
            }
            catch { /* DWM indisponível — mesma tolerância de ApplyMicaBackdrop */ }
        }

        /// <summary>Mica real via DWM — só funciona porque esta janela, ao contrário do notch, é ativável
        /// (sem WS_EX_NOACTIVATE). Se o Windows não suportar (build &lt; 11 22621), aborta cedo e mantém
        /// o fundo sólido --bg-window já setado no XAML — nunca tenta a composição transparente sem
        /// confirmar que o backdrop pegou, pra não repetir o bug de "janela preta" do notch.</summary>
        private void ApplyMicaBackdrop()
        {
            try
            {
                int dark = AppSettingsStore.Load().Theme == "light" ? 0 : 1;
                NativeMethods.DwmSetWindowAttribute(_hwnd, NativeMethods.DWMWA_USE_IMMERSIVE_DARK_MODE, ref dark, sizeof(int));

                int corner = (int)NativeMethods.DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;
                NativeMethods.DwmSetWindowAttribute(_hwnd, NativeMethods.DWMWA_WINDOW_CORNER_PREFERENCE, ref corner, sizeof(int));

                int backdrop = (int)NativeMethods.DWM_SYSTEMBACKDROP_TYPE.DWMSBT_MAINWINDOW;
                int hr = NativeMethods.DwmSetWindowAttribute(_hwnd, NativeMethods.DWMWA_SYSTEMBACKDROP_TYPE, ref backdrop, sizeof(int));
                if (hr != 0) return;

                // A extensão de frame (glass) fica a cargo do WindowChrome (GlassFrameThickness="-1" no XAML) —
                // chamar DwmExtendFrameIntoClientArea manualmente aqui entra em conflito: o WindowChrome reaplica
                // a própria gestão da non-client area em várias mensagens de janela e desfaz a chamada manual.
                if (_hwndSource != null)
                    _hwndSource.CompositionTarget.BackgroundColor = Colors.Transparent;

                WindowBorder.Background = System.Windows.Media.Brushes.Transparent;
            }
            catch
            {
                // DWM indisponível — mantém o fallback sólido opaco já setado no XAML
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == NativeMethods.WM_HOTKEY && wParam.ToInt32() == HOTKEY_ID_LAUNCHER)
            {
                Overlay?.Toggle();
                handled = true;
            }
            return IntPtr.Zero;
        }

        public void ShowLauncher()
        {
            if (!IsVisible) Show();
            if (WindowState == WindowState.Minimized) WindowState = WindowState.Normal;
            Activate();
        }

        public void ToggleLauncher()
        {
            if (IsVisible && WindowState != WindowState.Minimized) Hide();
            else ShowLauncher();
        }

        public void ForceClose()
        {
            _reallyClosing = true;
            Close();
        }

        private void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            if (_reallyClosing) return;
            e.Cancel = true;
            Hide();
        }

        protected override void OnClosed(EventArgs e)
        {
            Services.ThemeService.ThemeChanged -= OnThemeChanged;
            NativeMethods.UnregisterHotKey(_hwnd, HOTKEY_ID_LAUNCHER);
            _hwndSource?.RemoveHook(WndProc);
            Overlay?.Dispose();
            base.OnClosed(e);
        }

        // ---- Foco de janela — esmaece só a title/tab row, nunca a janela toda (PHASE-1-NOTCH.md mesma regra)

        private void MainWindow_Activated(object sender, EventArgs e)
        {
            TitleTabWrapper.Opacity = 1;
            WindowBorder.BorderBrush = ResourceBrush("BorderStrongBrush");
        }

        private void MainWindow_Deactivated(object sender, EventArgs e)
        {
            TitleTabWrapper.Opacity = 0.62;
            WindowBorder.BorderBrush = ResourceBrush("BorderSubtleBrush");
        }

        // ---- Chrome — botões de caption + correção de padding no maximizado (clássico gotcha de
        // WindowStyle=None + WindowChrome: sem isso, o conteúdo maximizado vaza um pouco pra fora da tela)

        private void MinimizeButton_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void MaximizeButton_Click(object sender, RoutedEventArgs e) =>
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            bool maximized = WindowState == WindowState.Maximized;
            MaximizePad.Padding = maximized ? new Thickness(7) : new Thickness(0);
            MaximizeGlyph.Visibility = maximized ? Visibility.Collapsed : Visibility.Visible;
            RestoreGlyph.Visibility = maximized ? Visibility.Visible : Visibility.Collapsed;
        }

        // ---- Abas

        private void AtalhosTab_Checked(object sender, RoutedEventArgs e)
        {
            GridScrollViewer.Visibility = Visibility.Visible;
            ConfigPanel.Visibility = Visibility.Collapsed;
            SearchBoxWrapper.Visibility = Visibility.Visible;
        }

        private void ConfigTab_Checked(object sender, RoutedEventArgs e)
        {
            GridScrollViewer.Visibility = Visibility.Collapsed;
            ConfigPanel.Visibility = Visibility.Visible;
            SearchBoxWrapper.Visibility = Visibility.Collapsed;
        }

        // ---- Busca

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchPlaceholder.Visibility = string.IsNullOrEmpty(SearchBox.Text) ? Visibility.Visible : Visibility.Collapsed;
            RefreshGrid();
        }

        // ---- Grid de atalhos

        private void LoadShortcuts()
        {
            _shortcuts = ShortcutStore.Load();
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            string query = SearchBox.Text?.Trim() ?? "";
            var filtered = string.IsNullOrEmpty(query)
                ? _shortcuts
                : _shortcuts.Where(s => s.Name.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();

            var display = filtered.Select(ShortcutTileDisplayFactory.Create).ToList();
            display.Add(new ShortcutTileDisplayItem { IsAddTile = true });

            ShortcutsGrid.ItemsSource = display;
            CountBadgeText.Text = _shortcuts.Count == 1 ? "1 atalho" : $"{_shortcuts.Count} atalhos";
        }

        private void ShortcutTile_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is not ShortcutTileDisplayItem item) return;

            if (item.IsAddTile)
            {
                OpenDialog(null);
                return;
            }

            LaunchAndShowStatus(item);
        }

        private void LaunchAndShowStatus(ShortcutTileDisplayItem item)
        {
            StatusText.Text = $"Abrindo {item.Name}…";
            ShortcutLauncherService.Launch(item.Source!);

            _statusResetTimer?.Stop();
            _statusResetTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1400) };
            _statusResetTimer.Tick += (_, _) =>
            {
                StatusText.Text = "Pronto";
                _statusResetTimer!.Stop();
            };
            _statusResetTimer.Start();
        }

        private void OpenDialog(ShortcutItem? editing)
        {
            var dialog = new ShortcutDialog(this, editing);
            if (dialog.ShowDialog() == true)
                LoadShortcuts();
        }

        // ---- Menu de contexto do tile (Abrir/Editar/Fixar/Remover — ContextMenu.jsx + a coluna "Pinned"
        // que o diálogo de adicionar/editar não expõe, ver ShortcutDialog.xaml.cs)

        private void TileContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            var menu = (System.Windows.Controls.ContextMenu)sender;
            if (menu.DataContext is not ShortcutTileDisplayItem item) return;

            var pinItem = menu.Items.OfType<System.Windows.Controls.MenuItem>().FirstOrDefault(m => (string?)m.Tag == "pin");
            if (pinItem != null) pinItem.Header = item.Source?.Pinned == true ? "Desfixar" : "Fixar";
        }

        private void ShortcutTile_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is ShortcutTileDisplayItem { IsAddTile: true })
                e.Handled = true; // sem menu no tile "Adicionar"
        }

        private void TileMenu_Open_Click(object sender, RoutedEventArgs e)
        {
            if (GetMenuTargetItem(sender) is { } item) LaunchAndShowStatus(item);
        }

        private void TileMenu_Edit_Click(object sender, RoutedEventArgs e)
        {
            if (GetMenuTargetItem(sender)?.Source is { } source) OpenDialog(source);
        }

        private void TileMenu_TogglePin_Click(object sender, RoutedEventArgs e)
        {
            if (GetMenuTargetItem(sender)?.Source is not { } source) return;
            source.Pinned = !source.Pinned;
            ShortcutStore.Save(_shortcuts);
            RefreshGrid();
        }

        private void TileMenu_Remove_Click(object sender, RoutedEventArgs e)
        {
            if (GetMenuTargetItem(sender)?.Source is not { } source) return;
            _shortcuts.Remove(source);
            ShortcutStore.Save(_shortcuts);
            RefreshGrid();
        }

        private static ShortcutTileDisplayItem? GetMenuTargetItem(object sender) =>
            ((System.Windows.Controls.MenuItem)sender).DataContext as ShortcutTileDisplayItem;

        private static System.Windows.Media.Brush ResourceBrush(string key) => (System.Windows.Media.Brush)System.Windows.Application.Current.FindResource(key);

        // ==================== Config (fase 5) ====================

        private bool _loadingConfig;
        private bool _capturingHotkey;

        private void LoadConfig()
        {
            _loadingConfig = true;

            var discord = DiscordSettingsStore.Load();
            DiscordEnabledCheck.IsChecked = discord.Enabled;
            DiscordClientIdField.Text = discord.ClientId;
            DiscordClientIdField.IsEnabled = discord.Enabled;
            DiscordClientIdField.TextEdited += (_, _) => SaveDiscordSettings();

            var settings = AppSettingsStore.Load();
            UpdateHotkeyBadges(settings.LauncherHotkeyModifiers, settings.LauncherHotkeyKey);

            NotchAnchorField.Options = new List<Controls.FormSelectOption>
            {
                new() { Value = "Left", Label = "Esquerda" },
                new() { Value = "Center", Label = "Centro" },
                new() { Value = "Right", Label = "Direita" },
            };
            NotchAnchorField.SelectedValue = settings.NotchAnchor;

            var screens = System.Windows.Forms.Screen.AllScreens;
            var monitorOptions = new List<Controls.FormSelectOption> { new() { Value = "", Label = "Monitor principal" } };
            monitorOptions.AddRange(screens.Select((s, i) =>
                new Controls.FormSelectOption { Value = s.DeviceName, Label = $"Monitor {i + 1} ({s.Bounds.Width}×{s.Bounds.Height}){(s.Primary ? " — principal" : "")}" }));
            NotchMonitorField.Options = monitorOptions;
            NotchMonitorField.SelectedValue = settings.NotchMonitorDeviceName;

            _loadingConfig = false;
        }

        private void UpdateHotkeyBadges(uint modifiers, uint key)
        {
            string label = ShortcutTileDisplayFactory.FormatHotkey(modifiers, key);
            TitleHotkeyBadge.Text = label;
            ConfigHotkeyBadge.Text = label;
        }

        private void SaveDiscordSettings()
        {
            if (_loadingConfig) return;
            DiscordSettingsStore.Save(new DiscordSettings
            {
                Enabled = DiscordEnabledCheck.IsChecked == true,
                ClientId = DiscordClientIdField.Text?.Trim() ?? "",
            });
        }

        private void DiscordEnabledCheck_Changed(object sender, RoutedEventArgs e)
        {
            DiscordClientIdField.IsEnabled = DiscordEnabledCheck.IsChecked == true;
            SaveDiscordSettings();
        }

        private void NotchAnchorField_SelectionChanged(object sender, EventArgs e)
        {
            if (_loadingConfig) return;
            var settings = AppSettingsStore.Load();
            settings.NotchAnchor = (string)NotchAnchorField.SelectedValue;
            AppSettingsStore.Save(settings);
            ApplyNotchPlacement();
        }

        private void NotchMonitorField_SelectionChanged(object sender, EventArgs e)
        {
            if (_loadingConfig) return;
            var settings = AppSettingsStore.Load();
            settings.NotchMonitorDeviceName = (string)NotchMonitorField.SelectedValue;
            AppSettingsStore.Save(settings);
            ApplyNotchPlacement();
        }

        private static void ApplyNotchPlacement() =>
            System.Windows.Application.Current.Windows.OfType<NotchWindow>().FirstOrDefault()?.ApplyPlacementSettings();

        private void RegravarButton_Click(object sender, RoutedEventArgs e)
        {
            if (_capturingHotkey) return;
            _capturingHotkey = true;
            RegravarButton.Content = "Pressione uma combinação (Esc cancela)…";
            PreviewKeyDown += Config_CaptureHotkeyKeyDown;
        }

        private void Config_CaptureHotkeyKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = true;
            var key = e.Key == Key.System ? e.SystemKey : e.Key;

            if (key == Key.Escape) { EndHotkeyCapture(); return; }
            if (key is Key.LeftCtrl or Key.RightCtrl or Key.LeftAlt or Key.RightAlt or Key.LeftShift or Key.RightShift)
                return; // ainda esperando uma tecla não-modificadora

            uint modifiers = 0;
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) modifiers |= NativeMethods.MOD_CONTROL;
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt)) modifiers |= NativeMethods.MOD_ALT;
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)) modifiers |= NativeMethods.MOD_SHIFT;

            if (modifiers == 0) return; // exige ao menos um modificador — evita registrar hotkey de tecla única

            uint vk = (uint)KeyInterop.VirtualKeyFromKey(key);
            if (vk is < 0x30 or > 0x5A) return; // FormatHotkey só sabe mostrar dígito/letra — mesma limitação de sempre

            var settings = AppSettingsStore.Load();
            settings.LauncherHotkeyModifiers = modifiers;
            settings.LauncherHotkeyKey = vk;
            AppSettingsStore.Save(settings);

            NativeMethods.UnregisterHotKey(_hwnd, HOTKEY_ID_LAUNCHER);
            NativeMethods.RegisterHotKey(_hwnd, HOTKEY_ID_LAUNCHER, modifiers, vk);
            UpdateHotkeyBadges(modifiers, vk);

            EndHotkeyCapture();
        }

        private void EndHotkeyCapture()
        {
            _capturingHotkey = false;
            PreviewKeyDown -= Config_CaptureHotkeyKeyDown;
            RegravarButton.Content = "Regravar";
        }
    }
}
