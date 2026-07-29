using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Application = System.Windows.Application;

namespace Cortinho
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private NotifyIcon? _trayIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var menu = new ContextMenuStrip();
            menu.Items.Add("Sair", null, (_, _) => Shutdown());

            _trayIcon = new NotifyIcon
            {
                Icon = CreateTrayIcon(),
                Visible = true,
                Text = "Cortinho",
                ContextMenuStrip = menu
            };
        }

        private static System.Drawing.Icon CreateTrayIcon()
        {
            const int size = 64;

            var stripes = new DrawingGroup();
            using (var stripeDc = stripes.Open())
            {
                stripeDc.DrawRectangle(new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x3e, 0xa6, 0xff)), null, new Rect(0, 0, 14.08, 128));
                stripeDc.DrawRectangle(new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x2f, 0x8f, 0xe0)), null, new Rect(14.08, 0, 14.08, 128));
            }
            var stripeBrush = new DrawingBrush(stripes)
            {
                TileMode = TileMode.Tile,
                Viewport = new Rect(0, 0, 28.16, 128),
                ViewportUnits = BrushMappingMode.Absolute
            };

            var visual = new DrawingVisual();
            using (var dc = visual.RenderOpen())
            {
                dc.DrawRoundedRectangle(stripeBrush, null, new Rect(0, 0, 128, 128), 28, 28);

                var notchBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x14, 0x17, 0x1c));
                var notchGeometry = Geometry.Parse("M24 0 H104 V20 a26 26 0 0 1 -26 26 H50 a26 26 0 0 1 -26 -26 Z");
                dc.DrawGeometry(notchBrush, null, notchGeometry);
            }
            visual.Transform = new ScaleTransform((double)size / 128, (double)size / 128);

            var renderBitmap = new RenderTargetBitmap(size, size, 96, 96, PixelFormats.Pbgra32);
            renderBitmap.Render(visual);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
            using var stream = new MemoryStream();
            encoder.Save(stream);
            stream.Position = 0;

            using var bitmap = new System.Drawing.Bitmap(stream);
            return System.Drawing.Icon.FromHandle(bitmap.GetHicon());
        }
        protected override void OnExit(ExitEventArgs e)
        {
            _trayIcon?.Dispose();
            base.OnExit(e);
        }
    }

}
