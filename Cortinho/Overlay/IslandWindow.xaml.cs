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
        /// <summary>height = double.NaN pede altura automática (SizeToContent) — usado pelas ilhas
        /// "264×auto" do PHASE-OVERLAY.md §3, cuja âncora é sempre pelo topo (Top não depende da altura,
        /// então medir depois do primeiro layout não atrapalha o posicionamento). padding sobrescreve o
        /// 16 padrão — só a ilha Config (44×44) usa um valor menor pra sobrar espaço pro glifo.</summary>
        public IslandWindow(FrameworkElement content, double width, double height, double padding = 16)
        {
            InitializeComponent();
            Width = width;
            if (double.IsNaN(height))
                SizeToContent = SizeToContent.Height;
            else
                Height = height;
            IslandRoot.Padding = new Thickness(padding);
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

        /// <summary>Remove WS_EX_NOACTIVATE e força esta janela a virar foreground (PHASE-OVERLAY.md
        /// §6) — só a ilha de Busca chama isso, ao ser clicada. Deactivate() devolve o estado normal;
        /// o OverlayController liga isso ao evento Window.Deactivated. Nome não é "Activate" de
        /// propósito: Window já tem um Activate() herdado (bool, ativação "educada" via Win32) que não
        /// mexe em WS_EX_NOACTIVATE — reaproveitar o nome ia esconder esse método sem sobrescrever nada.</summary>
        public void ActivateForInput()
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            int style = NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE);
            NativeMethods.SetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE, style & ~NativeMethods.WS_EX_NOACTIVATE);
            NativeMethods.SetForegroundWindow(hwnd);
        }

        public void Deactivate()
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            int style = NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE);
            NativeMethods.SetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE, style | NativeMethods.WS_EX_NOACTIVATE);
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
