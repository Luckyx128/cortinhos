using Cortinho.Native;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Cortinho.Overlay
{
    /// <summary>Casca genérica de uma ilha do overlay — acrylic/borda/raio/sombra ficam aqui, o
    /// conteúdo (grid, busca, mic...) é injetado como um UserControl. Uma classe, várias instâncias
    /// (PHASE-OVERLAY.md §2/§10).</summary>
    public partial class IslandWindow : Window
    {
        public IslandWindow(FrameworkElement content, double width, double height)
        {
            InitializeComponent();
            Width = width;
            Height = height;
            ContentHost.Content = content;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var hwnd = new WindowInteropHelper(this).Handle;
            int style = NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE);
            NativeMethods.SetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE,
                style | NativeMethods.WS_EX_TOOLWINDOW | NativeMethods.WS_EX_NOACTIVATE);
        }

        public void AnimateIn(TimeSpan delay, bool animate)
        {
            if (!animate)
            {
                Opacity = 1;
                IslandTranslate.Y = 0;
                return;
            }

            Opacity = 0;
            IslandTranslate.Y = 8;

            var duration = new Duration(TimeSpan.FromMilliseconds(220));
            var opacityAnim = new DoubleAnimation(1, duration) { BeginTime = delay, EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut } };
            var moveAnim = new DoubleAnimation(0, duration) { BeginTime = delay, EasingFunction = new BackEase { Amplitude = 0.35, EasingMode = EasingMode.EaseOut } };

            BeginAnimation(OpacityProperty, opacityAnim);
            IslandTranslate.BeginAnimation(TranslateTransform.YProperty, moveAnim);
        }

        public void AnimateOut(bool animate, Action onComplete)
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
