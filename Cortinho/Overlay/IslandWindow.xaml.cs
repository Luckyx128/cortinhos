using Cortinho.Native;
using System.Windows;
using System.Windows.Input;
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
        // Faixa transparente reservada em volta da ilha pro chrome do modo de edição — ver comentário
        // no XAML. Toda a API pública desta classe fala em coordenadas da ILHA, não da janela.
        private const double RingSide = 12;
        private const double RingTop = 40;

        private readonly double _islandWidth;
        private readonly double? _islandFixedHeight;

        private bool _isEditMode;
        private bool _isDragging;

        /// <summary>Disparado ao soltar a ilha depois de arrastar — o OverlayController persiste a
        /// nova âncora/offset a partir daí.</summary>
        public event Action<IslandWindow>? Dragged;

        public string IslandId { get; }

        public double IslandWidth => _islandWidth;

        /// <summary>Altura lógica da ilha. Nas ilhas "auto" (SizeToContent), só é confiável depois do
        /// primeiro layout — mesma ressalva que o OverlayController já respeita ao posicionar.</summary>
        public double IslandHeight => _islandFixedHeight ?? Math.Max(0, ActualHeight - RingTop - RingSide);

        public double IslandLeft => Left + RingSide;
        public double IslandTop => Top + RingTop;

        /// <summary>height = double.NaN pede altura automática (SizeToContent) — usado pelas ilhas
        /// "264×auto" do PHASE-OVERLAY.md §3, cuja âncora é sempre pelo topo (Top não depende da altura,
        /// então medir depois do primeiro layout não atrapalha o posicionamento). padding sobrescreve o
        /// 16 padrão — só a ilha Config (44×44) usa um valor menor pra sobrar espaço pro glifo.</summary>
        public IslandWindow(FrameworkElement content, string islandId, string label, double width, double height, double padding = 16)
        {
            InitializeComponent();

            IslandId = islandId;
            LabelChipText.Text = label;

            _islandWidth = width;
            Width = width + RingSide * 2;

            if (double.IsNaN(height))
            {
                _islandFixedHeight = null;
                SizeToContent = SizeToContent.Height;
            }
            else
            {
                _islandFixedHeight = height;
                Height = height + RingTop + RingSide;
            }

            IslandRoot.Padding = new Thickness(padding);
            ContentHost.Content = content;

            MouseLeftButtonDown += IslandWindow_MouseLeftButtonDown;
        }

        /// <summary>Posiciona pela coordenada da ILHA (o controller nunca vê a folga do chrome).</summary>
        public void SetIslandPosition(double islandLeft, double islandTop)
        {
            // Solta os clocks de animação antes de escrever: um DoubleAnimation com FillBehavior.HoldEnd
            // (o encaixe do §7) continua sobrescrevendo atribuições diretas em Left/Top depois de
            // terminar — mesmo gotcha que já mordeu o drag do notch.
            BeginAnimation(LeftProperty, null);
            BeginAnimation(TopProperty, null);

            Left = islandLeft - RingSide;
            Top = islandTop - RingTop;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var hwnd = new WindowInteropHelper(this).Handle;
            int style = NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE);
            NativeMethods.SetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE,
                style | NativeMethods.WS_EX_TOOLWINDOW | NativeMethods.WS_EX_NOACTIVATE);
        }

        // ---- Modo de edição de layout (PHASE-OVERLAY.md §9)

        public void SetEditMode(bool on)
        {
            _isEditMode = on;

            EditRing.Visibility = on ? Visibility.Visible : Visibility.Collapsed;
            LabelChip.Visibility = on ? Visibility.Visible : Visibility.Collapsed;
            Cursor = on ? System.Windows.Input.Cursors.Hand : System.Windows.Input.Cursors.Arrow;

            // Enquanto arruma o layout, o conteúdo não recebe clique — senão arrastar o grid dispararia
            // o atalho embaixo do cursor. Fora do modo de edição a ilha volta a ser só conteúdo.
            ContentHost.IsHitTestVisible = !on;
        }

        private void IslandWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_isEditMode || _isDragging) return;

            _isDragging = true;
            EditRing.Stroke = (System.Windows.Media.Brush)FindResource("AccentBrush");
            IslandScale.ScaleX = 1.02;
            IslandScale.ScaleY = 1.02;
            Cursor = System.Windows.Input.Cursors.SizeAll;

            BeginAnimation(LeftProperty, null);
            BeginAnimation(TopProperty, null);

            try { DragMove(); }
            catch { /* DragMove joga se o botão já subiu entre o evento e a chamada */ }

            _isDragging = false;
            EditRing.Stroke = (System.Windows.Media.Brush)FindResource("BorderStrongBrush");
            IslandScale.ScaleX = 1;
            IslandScale.ScaleY = 1;
            Cursor = System.Windows.Input.Cursors.Hand;

            SnapIntoPlace();
            Dragged?.Invoke(this);
        }

        /// <summary>Encaixe do §3 (≤14 DIPs das bordas/centro), animado em 220ms spring como pede o §7.</summary>
        private void SnapIntoPlace()
        {
            var (snappedLeft, snappedTop) = OverlayAnchors.Snap(
                IslandLeft, IslandTop, IslandWidth, IslandHeight,
                SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);

            double targetWindowLeft = snappedLeft - RingSide;
            double targetWindowTop = snappedTop - RingTop;

            if (!SystemParameters.ClientAreaAnimation)
            {
                SetIslandPosition(snappedLeft, snappedTop);
                return;
            }

            var ease = new BackEase { Amplitude = 0.35, EasingMode = EasingMode.EaseOut };
            var duration = new Duration(TimeSpan.FromMilliseconds(220));

            BeginAnimation(LeftProperty, new DoubleAnimation(targetWindowLeft, duration) { EasingFunction = ease });
            BeginAnimation(TopProperty, new DoubleAnimation(targetWindowTop, duration) { EasingFunction = ease });
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
