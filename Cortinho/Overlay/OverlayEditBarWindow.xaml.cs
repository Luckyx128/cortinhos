using Cortinho.Native;
using System.Windows;
using System.Windows.Interop;

namespace Cortinho.Overlay
{
    /// <summary>Barra do rodapé do modo de edição de layout (PHASE-OVERLAY.md §9).</summary>
    public partial class OverlayEditBarWindow : Window
    {
        public event Action? SaveRequested;
        public event Action? RestoreRequested;

        public OverlayEditBarWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var hwnd = new WindowInteropHelper(this).Handle;
            int style = NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE);
            NativeMethods.SetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE,
                style | NativeMethods.WS_EX_TOOLWINDOW | NativeMethods.WS_EX_NOACTIVATE);
        }

        /// <summary>Centraliza no rodapé. Chamado depois do Show() — SizeToContent só resolve a
        /// largura real depois do primeiro layout.</summary>
        public void CenterAtBottom()
        {
            UpdateLayout();
            Left = (SystemParameters.PrimaryScreenWidth - ActualWidth) / 2;
            Top = SystemParameters.PrimaryScreenHeight - ActualHeight - 8;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e) => SaveRequested?.Invoke();
        private void RestoreButton_Click(object sender, RoutedEventArgs e) => RestoreRequested?.Invoke();
    }
}
