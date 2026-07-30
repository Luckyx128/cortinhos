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
using System.Linq;
using System.Windows.Threading;

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

        private enum NotchAnchor { Left, Center, Right }
        private NotchAnchor _anchor = NotchAnchor.Center;
        private bool _isTransitioning;

        private const double CompactHeight = 36;
        private const double ExpandedWidth = 400;
        private const double ExpandedHeight = 357;
        private bool _isExpanded;


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
            MicToggleButton.Content = isMuted ? "Ativar mic" : "Mutar mic";
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
            if (_isExpanded || _isTransitioning) return;
            MicCaption.Visibility = Visibility.Visible;
            AnimateWidth(PeekWidth);
        }

        private void NotchRoot_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_isExpanded || _isTransitioning) return;
            MicCaption.Visibility = Visibility.Collapsed;
            AnimateWidth(CompactWidth);
        }

        private void AnimateWidth(double targetWidth)
        {
            double targetLeft = GetLeftForAnchor(_anchor, targetWidth);

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

        private void NotchRoot_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is DependencyObject source && MicToggleButton.IsAncestorOf(source))
                return;
            if (_isTransitioning) return;
            double startLeft = Left;
            double startTop = Top;

            MicCaption.Visibility = Visibility.Collapsed;
            if (!_isExpanded) Width = CompactWidth;

            Cursor = System.Windows.Input.Cursors.SizeAll;
            NotchScale.ScaleX = 1.03;
            NotchScale.ScaleY = 1.03;
            NotchShadow.BlurRadius = 40;
            NotchShadow.ShadowDepth = 14;
            NotchShadow.Opacity = 0.5;

            BeginAnimation(LeftProperty, null);
            BeginAnimation(TopProperty, null);

            DragMove();

            Cursor = System.Windows.Input.Cursors.Arrow;
            NotchScale.ScaleX = 1;
            NotchScale.ScaleY = 1;
            NotchShadow.BlurRadius = 16;
            NotchShadow.ShadowDepth = 2;
            NotchShadow.Opacity = 0.34;

            bool wasDrag = Math.Abs(Left - startLeft) > 2 || Math.Abs(Top - startTop) > 2;

            if (wasDrag)
                SnapToNearestAnchor();
            else
                ToggleExpanded();
        }

        private void ToggleExpanded()
        {
            _isExpanded = !_isExpanded;
            _isTransitioning = true;

            double targetWidth = _isExpanded ? ExpandedWidth : CompactWidth;
            double targetHeight = _isExpanded ? ExpandedHeight : CompactHeight;
            double targetLeft = GetLeftForAnchor(_anchor, targetWidth);

            CompactContent.Visibility = _isExpanded ? Visibility.Collapsed : Visibility.Visible;
            ExpandedContent.Visibility = _isExpanded ? Visibility.Visible : Visibility.Collapsed;

            AnimateSize(targetWidth, targetHeight, targetLeft);

            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(240) };
            timer.Tick += (_, _) =>
            {
                _isTransitioning = false;
                timer.Stop();
            };
            timer.Start();
        }

        private void AnimateSize(double targetWidth, double targetHeight, double targetLeft)
        {
            if (!SystemParameters.ClientAreaAnimation)
            {
                Width = targetWidth;
                Height = targetHeight;
                Left = targetLeft;
                return;
            }

            BeginAnimation(WidthProperty, null);
            BeginAnimation(HeightProperty, null);

            var ease = new CubicEase { EasingMode = EasingMode.EaseOut };
            var duration = new Duration(TimeSpan.FromMilliseconds(220));

            BeginAnimation(WidthProperty, new DoubleAnimation(targetWidth, duration) { EasingFunction = ease });
            BeginAnimation(HeightProperty, new DoubleAnimation(targetHeight, duration) { EasingFunction = ease });
            BeginAnimation(LeftProperty, new DoubleAnimation(targetLeft, duration) { EasingFunction = ease });
        }

        private void MicToggleButton_Click(object sender, RoutedEventArgs e)
        {
            _micService.ToggleMute();
            UpdateMicIcon();
        }


        private void SnapToNearestAnchor()
        {
            var workArea = SystemParameters.WorkArea;
            var candidates = new (NotchAnchor Anchor, double Left)[]
            {
        (NotchAnchor.Left, workArea.Left + 12),
        (NotchAnchor.Center, workArea.Left + (workArea.Width - CompactWidth) / 2),
        (NotchAnchor.Right, workArea.Right - CompactWidth - 12),
            };

            var closest = candidates.OrderBy(c => Math.Abs(c.Left - Left)).First();
            _anchor = closest.Anchor;

            var ease = new BackEase { Amplitude = 0.35, EasingMode = EasingMode.EaseOut };
            var duration = new Duration(TimeSpan.FromMilliseconds(220));

            BeginAnimation(LeftProperty, new DoubleAnimation(closest.Left, duration) { EasingFunction = ease });
            BeginAnimation(TopProperty, new DoubleAnimation(workArea.Top, duration) { EasingFunction = ease });
        }

        private double GetLeftForAnchor(NotchAnchor anchor, double width)
        {
            var workArea = SystemParameters.WorkArea;
            return anchor switch
            {
                NotchAnchor.Left => workArea.Left + 12,
                NotchAnchor.Right => workArea.Right - width - 12,
                _ => workArea.Left + (workArea.Width - width) / 2,
            };
        }
    }
}