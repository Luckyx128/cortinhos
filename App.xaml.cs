using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace ShortcutLauncher;

public partial class App : Application
{
    private NotifyIcon? _trayIcon;
    private MainWindow? _mainWindow;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _mainWindow = new MainWindow();

        SetupTrayIcon();

        // Começa minimizado na bandeja, sem abrir a janela de cara
        _mainWindow.Hide();
    }

    private void SetupTrayIcon()
    {
        _trayIcon = new NotifyIcon
        {
            Icon = System.Drawing.SystemIcons.Application,
            Visible = true,
            Text = "Shortcut Launcher (Ctrl+Alt+L)"
        };

        var menu = new ContextMenuStrip();
        menu.Items.Add("Abrir launcher", null, (_, _) => ShowLauncher());
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Sair", null, (_, _) => ExitApp());
        _trayIcon.ContextMenuStrip = menu;

        _trayIcon.DoubleClick += (_, _) => ShowLauncher();
    }

    public void ShowLauncher()
    {
        if (_mainWindow is null) return;

        if (_mainWindow.IsVisible)
        {
            _mainWindow.Hide();
        }
        else
        {
            _mainWindow.Show();
            _mainWindow.Activate();
            _mainWindow.WindowState = WindowState.Normal;
        }
    }

    public void ShowNotification(string title, string message)
    {
        _trayIcon?.ShowBalloonTip(5000, title, message, ToolTipIcon.Warning);
    }

    private void ExitApp()
    {
        _mainWindow?.Cleanup();
        _trayIcon!.Visible = false;
        _trayIcon.Dispose();
        Shutdown();
    }
}
