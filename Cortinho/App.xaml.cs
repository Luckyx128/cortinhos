using System.Windows;
using System.Windows.Forms;
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
                Icon = System.Drawing.SystemIcons.Application,
                Visible = true,
                Text = "Cortinho",
                ContextMenuStrip = menu
            };
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _trayIcon?.Dispose();
            base.OnExit(e);
        }
    }

}
