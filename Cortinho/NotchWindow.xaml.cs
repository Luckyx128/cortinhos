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

namespace Cortinho
{
    public partial class NotchWindow : Window
    {
        private readonly MicService _micService = new();
        private HwndSource? _hwndSource;
        private const int GW_EXSTYLE = -20;
        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int WS_EX_TOOLWINDOW = 0x00000080;

        private const int WM_HOTKEY = 0x0312;
        private const int HOTKEY_ID_MUTE = 9000;
        private const uint MOD_CONTROL = 0x0002;
        private const uint MOD_ALT = 0x0001;
        private const uint VK_M = 0x4D;

        private const double CompactWidth = 92;
        private const double PeekWidth = 232;


        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        public NotchWindow()
        {
            InitializeComponent();
            Top = SystemParameters.WorkArea.Top;
            Left = (SystemParameters.WorkArea.Width - Width) / 2;

            UpdateMicIcon();
        }
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY && wParam.ToInt32() == HOTKEY_ID_MUTE)
            {
                _micService.ToggleMute();
                UpdateMicIcon();
                handled = true;
            }
            return IntPtr.Zero;
        }

        protected override void OnClosed(EventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            UnregisterHotKey(hwnd, HOTKEY_ID_MUTE);
            _hwndSource?.RemoveHook(WndProc);
            base.OnClosed(e);
        }

        private void UpdateMicIcon()
        {
            bool isMuted = _micService.IsMuted;
            MicOnIcon.Visibility = isMuted ? Visibility.Collapsed :
                Visibility.Visible;
            MicOffIcon.Visibility = isMuted ? Visibility.Visible : Visibility.Collapsed;
            MicCaption.Text = isMuted ? "Mic mutado" : "Mic ativo";
            MicUnderline.Visibility = isMuted ? Visibility.Visible : Visibility.Collapsed;
        }


        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var hwnd = new WindowInteropHelper(this).Handle;
            int currentStyle = GetWindowLong(hwnd, GW_EXSTYLE);
            SetWindowLong(hwnd, GW_EXSTYLE, currentStyle | WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW);
            _hwndSource = HwndSource.FromHwnd(hwnd);
            _hwndSource?.AddHook(WndProc);

            RegisterHotKey(hwnd, HOTKEY_ID_MUTE, MOD_CONTROL | MOD_ALT, VK_M);
        }

        private void NotchRoot_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MicCaption.Visibility = Visibility.Visible;
            AnimateWidth(PeekWidth);
        }

        private void NotchRoot_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MicCaption.Visibility = Visibility.Collapsed;
            AnimateWidth(CompactWidth);
        }

        private void AnimateWidth(double targetWidth)
        {
            double targetLeft = SystemParameters.WorkArea.Left + (SystemParameters.WorkArea.Width - targetWidth) / 2;

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
    }
}