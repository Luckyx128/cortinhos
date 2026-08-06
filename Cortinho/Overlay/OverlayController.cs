using Cortinho.Native;
using Cortinho.Overlay.Islands;
using System.Windows;

namespace Cortinho.Overlay
{
    /// <summary>Abre/fecha o overlay: scrim + ilhas, foco (PHASE-OVERLAY.md §6), stagger de entrada.
    /// Fase 2 = grid + as 6 ilhas "simples" (Config/Favoritos/Mic/Discord/Desempenho/Notificações) — só
    /// a Busca (a única que tira foco do jogo) fica pra depois.</summary>
    public class OverlayController
    {
        private readonly MainWindow _mainWindow;

        private readonly OverlayScrimWindow _scrim = new();
        private readonly Services.GlobalInputWatcher _inputWatcher = new();

        private readonly ShortcutGridIsland _gridContent = new();
        private readonly IslandWindow _gridIslandWindow;

        private readonly MicIsland _micContent = new();
        private readonly IslandWindow _micIslandWindow;

        private readonly NotificationsIsland _notifContent = new();
        private readonly IslandWindow _notifIslandWindow;

        private readonly FavoritesIsland _favContent = new();
        private readonly IslandWindow _favIslandWindow;

        private readonly DiscordIsland _discordContent = new();
        private readonly IslandWindow _discordIslandWindow;

        private readonly PerfIsland _perfContent = new();
        private readonly IslandWindow _perfIslandWindow;

        private readonly ConfigIsland _configContent = new();
        private readonly IslandWindow _configIslandWindow;

        private bool _isOpen;
        private IntPtr _previousForegroundWindow;

        public OverlayController(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;

            _gridIslandWindow = new IslandWindow(_gridContent, 848, 364);
            _micIslandWindow = new IslandWindow(_micContent, 264, double.NaN);
            _notifIslandWindow = new IslandWindow(_notifContent, 264, 200);
            _favIslandWindow = new IslandWindow(_favContent, 264, double.NaN);
            _discordIslandWindow = new IslandWindow(_discordContent, 264, double.NaN);
            _perfIslandWindow = new IslandWindow(_perfContent, 264, double.NaN);
            _configIslandWindow = new IslandWindow(_configContent, 44, 44, padding: 6);

            _configContent.ConfigClicked += () =>
            {
                _mainWindow.ShowLauncher();
                Close();
            };

            _inputWatcher.EscapePressed += () => { if (_isOpen) Close(); };
        }

        public void Toggle()
        {
            if (_isOpen) Close();
            else Open();
        }

        private void Open()
        {
            if (_isOpen) return;
            _isOpen = true;

            _previousForegroundWindow = NativeMethods.GetForegroundWindow();

            _gridContent.Refresh();
            _favContent.Refresh();
            _micContent.Refresh();
            _notifContent.Refresh();

            bool discordAvailable = _discordContent.IsAvailable;
            if (discordAvailable) _discordContent.Refresh();

            bool perfAvailable = _perfContent.IsAvailable;
            if (perfAvailable) _perfContent.Start();

            PositionIslands();

            bool animate = SystemParameters.ClientAreaAnimation;

            _scrim.Show();
            _scrim.FadeIn(animate);

            // Stagger de ~20ms, ordem grid → laterais (PHASE-OVERLAY.md §7) — busca ainda não existe.
            int step = 0;
            TimeSpan NextDelay() => TimeSpan.FromMilliseconds(20 * step++);

            ShowIsland(_gridIslandWindow, NextDelay(), animate);
            ShowIsland(_micIslandWindow, NextDelay(), animate);
            ShowIsland(_notifIslandWindow, NextDelay(), animate);
            ShowIsland(_favIslandWindow, NextDelay(), animate);
            if (discordAvailable) ShowIsland(_discordIslandWindow, NextDelay(), animate);
            if (perfAvailable) ShowIsland(_perfIslandWindow, NextDelay(), animate);
            ShowIsland(_configIslandWindow, NextDelay(), animate);
        }

        private static void ShowIsland(IslandWindow window, TimeSpan delay, bool animate)
        {
            window.Show();
            window.AnimateIn(delay, animate);
        }

        private void Close()
        {
            if (!_isOpen) return;
            _isOpen = false;

            bool animate = SystemParameters.ClientAreaAnimation;

            _perfContent.Stop();
            _discordContent.Stop();

            HideIsland(_gridIslandWindow, animate);
            HideIsland(_micIslandWindow, animate);
            HideIsland(_notifIslandWindow, animate);
            HideIsland(_favIslandWindow, animate);
            if (_discordIslandWindow.IsVisible) HideIsland(_discordIslandWindow, animate);
            if (_perfIslandWindow.IsVisible) HideIsland(_perfIslandWindow, animate);
            HideIsland(_configIslandWindow, animate);

            _scrim.FadeOut(animate, () =>
            {
                _scrim.Hide();
                if (_previousForegroundWindow != IntPtr.Zero)
                    NativeMethods.SetForegroundWindow(_previousForegroundWindow);
            });
        }

        private static void HideIsland(IslandWindow window, bool animate) =>
            window.AnimateOut(animate, () => window.Hide());

        /// <summary>Arranjo A "Constelação" (PHASE-OVERLAY.md §3). Só o monitor primário por enquanto —
        /// mesma divergência aceita já registrada na fase 1 (settings de monitor do overlay é fase futura).</summary>
        private void PositionIslands()
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;

            _gridIslandWindow.Left = (screenWidth - _gridIslandWindow.Width) / 2;
            _gridIslandWindow.Top = screenHeight - _gridIslandWindow.Height - 18;

            _micIslandWindow.Left = 24;
            _micIslandWindow.Top = 206;

            _notifIslandWindow.Left = 24;
            _notifIslandWindow.Top = 286;

            _favIslandWindow.Left = 24;
            _favIslandWindow.Top = 502;

            _discordIslandWindow.Left = screenWidth - _discordIslandWindow.Width - 24;
            _discordIslandWindow.Top = 206;

            _perfIslandWindow.Left = screenWidth - _perfIslandWindow.Width - 24;
            _perfIslandWindow.Top = 336;

            _configIslandWindow.Left = screenWidth - _configIslandWindow.Width - 24;
            _configIslandWindow.Top = 24;
        }

        /// <summary>Chamado só de MainWindow.OnClosed (saída real do app pela bandeja) — libera o
        /// PerformanceCounter próprio do PerfIsland, mesma disciplina do NotchWindow.OnClosed.</summary>
        public void Dispose()
        {
            _perfContent.Dispose();
            _inputWatcher.Dispose();
        }
    }
}
