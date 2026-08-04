using Cortinho.Native;
using Cortinho.Overlay.Islands;
using System.Windows;

namespace Cortinho.Overlay
{
    /// <summary>Abre/fecha o overlay: scrim + ilhas, foco (PHASE-OVERLAY.md §6), stagger de entrada.
    /// Fase 1 = só a ilha do grid; o resto (busca, mic, notificações...) entra na fase 2, mas o
    /// esqueleto de posicionamento por âncora já fica pronto pra elas.</summary>
    public class OverlayController
    {
        private readonly OverlayScrimWindow _scrim = new();
        private readonly IslandWindow _gridIslandWindow;
        private readonly ShortcutGridIsland _gridContent = new();
        private readonly Services.GlobalInputWatcher _inputWatcher = new();

        private bool _isOpen;
        private IntPtr _previousForegroundWindow;

        public OverlayController()
        {
            _gridIslandWindow = new IslandWindow(_gridContent, 848, 364);
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
            PositionIslands();

            bool animate = SystemParameters.ClientAreaAnimation;

            _scrim.Show();
            _scrim.FadeIn(animate);

            _gridIslandWindow.Show();
            _gridIslandWindow.AnimateIn(TimeSpan.Zero, animate); // ilha única na fase 1 — stagger real entra com mais ilhas
        }

        private void Close()
        {
            if (!_isOpen) return;
            _isOpen = false;

            bool animate = SystemParameters.ClientAreaAnimation;

            _gridIslandWindow.AnimateOut(animate, () => _gridIslandWindow.Hide());
            _scrim.FadeOut(animate, () =>
            {
                _scrim.Hide();
                if (_previousForegroundWindow != IntPtr.Zero)
                    NativeMethods.SetForegroundWindow(_previousForegroundWindow);
            });
        }

        /// <summary>Arranjo A "Constelação" (PHASE-OVERLAY.md §3) — só a âncora bottom-center (grid)
        /// está implementada; top-left/top-right/top-center entram junto das ilhas da fase 2.</summary>
        private void PositionIslands()
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;

            _gridIslandWindow.Left = (screenWidth - _gridIslandWindow.Width) / 2;
            _gridIslandWindow.Top = screenHeight - _gridIslandWindow.Height - 18;
        }
    }
}
