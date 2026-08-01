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

        // Derivados do AppxManifest.xml (Package\AppxManifest.xml) — só mudam se Identity/Publisher ou o Application Id mudarem lá
        private const string PackageFamilyName = "Cortinho.Notch_7s34dc27ge8hm";
        private const string PackageAppId = "CortinhoNotch";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (!EnsurePackageIdentity())
                return; // relançou como processo com identidade e está saindo — não cria UI nesse processo

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

        /// <returns>true se este processo já tem (ou passou a ter) identidade de pacote e deve seguir com a UI normal; false se relançou e este processo deve encerrar.</returns>
        private static bool EnsurePackageIdentity()
        {
            if (HasPackageIdentity())
                return true;

            try
            {
                var msixPath = Path.Combine(AppContext.BaseDirectory, "CortinhoSparse.msix");
                var packageManager = new global::Windows.Management.Deployment.PackageManager();
                var options = new global::Windows.Management.Deployment.AddPackageOptions
                {
                    ExternalLocationUri = new Uri(AppContext.BaseDirectory),
                    ForceUpdateFromAnyVersion = true
                };

                var result = packageManager.AddPackageByUriAsync(new Uri(msixPath), options)
                    .AsTask().GetAwaiter().GetResult();

                if (result.ExtendedErrorCode != null)
                    throw result.ExtendedErrorCode;
            }
            catch
            {
                // Sem identidade mesmo (certificado não confiável, .msix ausente, etc) — segue sem ela.
                // Módulos que dependem de identidade de pacote (ex: Notificações) devem se esconder sozinhos nesse caso.
                return true;
            }

            System.Diagnostics.Process.Start("explorer.exe", $"shell:appsFolder\\{PackageFamilyName}!{PackageAppId}");
            Environment.Exit(0);
            return false;
        }

        private static bool HasPackageIdentity()
        {
            try
            {
                _ = global::Windows.ApplicationModel.Package.Current;
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
    }

}
