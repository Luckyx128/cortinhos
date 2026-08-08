using Cortinho.Native;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;

namespace Cortinho.Overlay
{
    /// <summary>Vinheta em tela cheia — nunca captura clique (WS_EX_TRANSPARENT permanente, ver
    /// PHASE-OVERLAY.md §2). Só pinta; toda a interação do overlay vive nas IslandWindows.</summary>
    public partial class OverlayScrimWindow : Window
    {
        public OverlayScrimWindow()
        {
            InitializeComponent();

            Left = 0;
            Top = 0;
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var hwnd = new WindowInteropHelper(this).Handle;
            int style = NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE);
            NativeMethods.SetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE,
                style | NativeMethods.WS_EX_LAYERED | NativeMethods.WS_EX_TRANSPARENT
                      | NativeMethods.WS_EX_TOOLWINDOW | NativeMethods.WS_EX_NOACTIVATE);
        }

        /// <summary>Modo de edição (§9) — troca a vinheta pelo escurecimento uniforme.</summary>
        public void SetEditMode(bool on, bool animate)
        {
            double target = on ? 1 : 0;

            if (!animate)
            {
                UniformLayer.Opacity = target;
                return;
            }

            UniformLayer.BeginAnimation(OpacityProperty, new DoubleAnimation(target, new Duration(TimeSpan.FromMilliseconds(220)))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            });
        }

        public void FadeIn(bool animate)
        {
            if (!animate)
            {
                Opacity = 1;
                return;
            }
            BeginAnimation(OpacityProperty, new DoubleAnimation(1, new Duration(TimeSpan.FromMilliseconds(220)))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            });
        }

        public void FadeOut(bool animate, Action onComplete)
        {
            if (!animate)
            {
                Opacity = 0;
                onComplete();
                return;
            }

            var animation = new DoubleAnimation(0, new Duration(TimeSpan.FromMilliseconds(220)))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            animation.Completed += (_, _) => onComplete();
            BeginAnimation(OpacityProperty, animation);
        }
    }
}
