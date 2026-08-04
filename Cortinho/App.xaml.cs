using System.IO;
using System.Linq;
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
        private MainWindow? _mainWindow;

        // Derivados do AppxManifest.xml (Package\AppxManifest.xml) — só mudam se Identity/Publisher ou o Application Id mudarem lá
        private const string PackageFamilyName = "Cortinho.Notch_7s34dc27ge8hm";
        private const string PackageAppId = "CortinhoNotch";

        protected override void OnStartup(StartupEventArgs e)
        {
            Services.ThemeService.ApplySaved(); // antes do base.OnStartup — StartupUri cria a NotchWindow já aqui dentro
            ApplySystemAccent();
            base.OnStartup(e);

            if (!EnsurePackageIdentity())
                return; // relançou como processo com identidade e está saindo — não cria UI nesse processo

            _mainWindow = new MainWindow(); // criada oculta — só aparece via tray, hotkey Ctrl+Alt+L (se a flag de overlay estiver off) ou clique no ícone
            _mainWindow.Overlay = new Overlay.OverlayController(); // design_handoff_overlay fase 1 — atrás da flag "OpenOverlayOnHotkey" em app-settings.json

            // Acrylic real num ContextMenuStrip (WinForms) não é viável sem trabalho Win32 bem maior
            // (SetWindowRgn pra cantos + composição própria) — divergência aceita do fase 4. Aproxima
            // a paleta dark (surface-3/border-subtle/hover) via ToolStripProfessionalRenderer.
            var menu = new ContextMenuStrip
            {
                Renderer = new TrayMenuRenderer(),
                ForeColor = System.Drawing.Color.FromArgb(0xEC, 0xEE, 0xF2),
                Font = new System.Drawing.Font("Segoe UI", 9f),
                ShowImageMargin = false,
            };
            menu.Items.Add("Abrir launcher", null, (_, _) => _mainWindow.ShowLauncher());
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("Sair", null, (_, _) =>
            {
                _mainWindow?.ForceClose();
                Shutdown();
            });

            _trayIcon = new NotifyIcon
            {
                Icon = CreateTrayIcon(),
                Visible = true,
                Text = "Cortinho",
                ContextMenuStrip = menu
            };
            _trayIcon.MouseClick += (_, args) =>
            {
                if (args.Button == System.Windows.Forms.MouseButtons.Left)
                    _mainWindow.ShowLauncher();
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

            if (!TryRegisterPackage())
                return true; // sem identidade mesmo — segue, módulos que dependem dela (ex: Notificações) devem se esconder sozinhos

            System.Diagnostics.Process.Start("explorer.exe", $"shell:appsFolder\\{PackageFamilyName}!{PackageAppId}");
            Environment.Exit(0);
            return false;
        }

        private static bool TryRegisterPackage()
        {
            var packageManager = new global::Windows.Management.Deployment.PackageManager();
            var msixUri = new Uri(Path.Combine(AppContext.BaseDirectory, "CortinhoSparse.msix"));
            var options = new global::Windows.Management.Deployment.AddPackageOptions
            {
                ExternalLocationUri = new Uri(AppContext.BaseDirectory),
                ForceUpdateFromAnyVersion = true
            };

            try
            {
                var result = packageManager.AddPackageByUriAsync(msixUri, options).AsTask().GetAwaiter().GetResult();
                if (result.ExtendedErrorCode != null) throw result.ExtendedErrorCode;
                return true;
            }
            catch (Exception ex) when ((uint)ex.HResult == 0x80073D0B)
            {
                // Pacote já registrado apontando pra outra pasta externa (ex: alternando entre build Debug e Release)
                // — remove o registro antigo e registra de novo apontando pra pasta atual.
                try
                {
                    var existing = packageManager.FindPackagesForUser(string.Empty, PackageFamilyName).FirstOrDefault();
                    if (existing == null) return false;

                    packageManager.RemovePackageAsync(existing.Id.FullName).AsTask().GetAwaiter().GetResult();

                    var retry = packageManager.AddPackageByUriAsync(msixUri, options).AsTask().GetAwaiter().GetResult();
                    return retry.ExtendedErrorCode == null;
                }
                catch
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
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

        /// <summary>
        /// Sobrescreve --accent (Theme/Tokens.xaml) com a cor de destaque real do Windows do usuário,
        /// em vez do azul --brand-accent fixo que fica só de fallback. Ver TOKENS-WPF.md: "Nunca hard-code
        /// o azul na UI — derive tudo de uma variável só".
        /// </summary>
        private void ApplySystemAccent()
        {
            try
            {
                var settings = new global::Windows.UI.ViewManagement.UISettings();
                var c = settings.GetColorValue(global::Windows.UI.ViewManagement.UIColorType.Accent);
                var accent = System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B);

                Resources["AccentColor"] = accent;
                Resources["AccentSoftColor"] = System.Windows.Media.Color.FromArgb(0x24, accent.R, accent.G, accent.B); // ~14%
                Resources["AccentHoverColor"] = Lighten(accent, 0.18);
            }
            catch
            {
                // UISettings indisponível — segue com o --brand-accent de fallback já definido em Tokens.xaml
            }
        }

        private static System.Windows.Media.Color Lighten(System.Windows.Media.Color c, double amount)
        {
            byte LightenChannel(byte channel) => (byte)Math.Clamp(channel + (255 - channel) * amount, 0, 255);
            return System.Windows.Media.Color.FromArgb(c.A, LightenChannel(c.R), LightenChannel(c.G), LightenChannel(c.B));
        }

        /// <summary>Aproxima --surface-3/--border-subtle/--hover-overlay num ContextMenuStrip do WinForms
        /// (menu da bandeja) — pinta direto (sem ToolStripProfessionalColorTable, que não está disponível
        /// nesse SDK de WinForms empacotado com o WPF) via ToolStripRenderer.</summary>
        private sealed class TrayMenuRenderer : ToolStripRenderer
        {
            private static readonly System.Drawing.Color Bg = System.Drawing.Color.FromArgb(0x2B, 0x33, 0x3D);
            private static readonly System.Drawing.Color BorderColor = System.Drawing.Color.FromArgb(0x3A, 0x42, 0x4C);
            private static readonly System.Drawing.Color Hover = System.Drawing.Color.FromArgb(0x39, 0x44, 0x50);
            private static readonly System.Drawing.Color Text = System.Drawing.Color.FromArgb(0xEC, 0xEE, 0xF2);

            protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
            {
                e.Graphics.Clear(Bg);
            }

            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
            {
                using var pen = new System.Drawing.Pen(BorderColor);
                e.Graphics.DrawRectangle(pen, 0, 0, e.ToolStrip.Width - 1, e.ToolStrip.Height - 1);
            }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                if (!e.Item.Selected) return;
                using var brush = new System.Drawing.SolidBrush(Hover);
                e.Graphics.FillRectangle(brush, new System.Drawing.Rectangle(System.Drawing.Point.Empty, e.Item.Size));
            }

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                e.TextColor = Text;
                base.OnRenderItemText(e);
            }

            protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
            {
                using var pen = new System.Drawing.Pen(BorderColor);
                int y = e.Item.Height / 2;
                e.Graphics.DrawLine(pen, 4, y, e.Item.Width - 4, y);
            }
        }
    }

}
