using System;
using System.Diagnostics;
using Cortinho.Models;

namespace Cortinho.Services
{
    public static class ShortcutLauncherService
    {
        public static void Launch(ShortcutItem item)
        {
            var path = Environment.ExpandEnvironmentVariables(item.Path);

            try
            {
                switch (item.Type)
                {
                    case ShortcutType.Application:
                        Process.Start(new ProcessStartInfo(path, item.Arguments) { UseShellExecute = true });
                        break;
                    case ShortcutType.Folder:
                        Process.Start(new ProcessStartInfo("explorer.exe", path) { UseShellExecute = true });
                        break;
                    case ShortcutType.Url:
                        Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                        break;
                    case ShortcutType.Command:
                        Process.Start(new ProcessStartInfo("cmd.exe", $"/c {path} {item.Arguments}")
                        {
                            UseShellExecute = true,
                            CreateNoWindow = true
                        });
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Não consegui executar '{item.Name}':\n{ex.Message}",
                    "Erro", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            }
        }
    }
}
