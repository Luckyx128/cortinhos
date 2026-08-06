using Cortinho.Native;
using Cortinho.Overlay.Islands;
using System.Collections.Generic;
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

            bool animate = SystemParameters.ClientAreaAnimation;

            _scrim.Show();
            _scrim.FadeIn(animate);

            // Mostrar ANTES de posicionar: as ilhas de altura automática (SizeToContent) só têm
            // ActualHeight válido depois de entrarem na árvore visual, e o empilhamento da coluna
            // esquerda depende dessa altura. Opacity=0 no Prepare evita o flash de um frame na
            // posição antiga — quem devolve a opacidade é o AnimateIn logo abaixo.
            var visible = new List<IslandWindow> { _gridIslandWindow, _micIslandWindow, _notifIslandWindow, _favIslandWindow };
            if (discordAvailable) visible.Add(_discordIslandWindow);
            if (perfAvailable) visible.Add(_perfIslandWindow);
            visible.Add(_configIslandWindow);

            foreach (var window in visible)
            {
                window.Opacity = 0;
                window.Show();
                window.UpdateLayout();
            }

            PositionIslands();

            // Stagger de ~20ms, ordem grid → laterais (PHASE-OVERLAY.md §7) — busca ainda não existe.
            for (int i = 0; i < visible.Count; i++)
                visible[i].AnimateIn(TimeSpan.FromMilliseconds(20 * i), animate);
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

        private const double ColumnGap = 12;

        /// <summary>Arranjo A "Constelação" (PHASE-OVERLAY.md §3). Só o monitor primário por enquanto —
        /// mesma divergência aceita já registrada na fase 1 (settings de monitor do overlay é fase futura).
        ///
        /// Divergência dos offsets Y do §3: a tabela do spec (mic 206 / notif 286 / favoritos 502) assume
        /// uma ilha de mic de ~80 DIPs, mas com o cabeçalho comum do §5 e o padding de 16 ela sai com
        /// ~119 — com offsets fixos as ilhas da coluna esquerda se sobrepunham visivelmente. Aqui só o Y
        /// da PRIMEIRA ilha de cada coluna vem do spec; as de baixo empilham pela altura real medida.
        /// Isso deixa de importar na fase 3, quando o layout passa a ser arrastado e persistido.</summary>
        private void PositionIslands()
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;

            _gridIslandWindow.Left = (screenWidth - _gridIslandWindow.Width) / 2;
            _gridIslandWindow.Top = screenHeight - _gridIslandWindow.Height - 18;

            _configIslandWindow.Left = screenWidth - _configIslandWindow.Width - 24;
            _configIslandWindow.Top = 24;

            StackColumn(24, 206, _micIslandWindow, _notifIslandWindow, _favIslandWindow);

            double rightLeft = screenWidth - 264 - 24;
            StackColumn(rightLeft, 206, _discordIslandWindow, _perfIslandWindow);
        }

        /// <summary>Empilha as ilhas visíveis de uma coluna a partir de (left, firstTop), com ColumnGap
        /// entre elas. Ignora as escondidas (Discord/Desempenho somem quando não há provider).</summary>
        private static void StackColumn(double left, double firstTop, params IslandWindow[] windows)
        {
            double top = firstTop;
            foreach (var window in windows)
            {
                if (!window.IsVisible) continue;

                window.Left = left;
                window.Top = top;
                top += window.ActualHeight + ColumnGap;
            }
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
