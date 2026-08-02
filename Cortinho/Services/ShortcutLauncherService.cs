using System;
using System.Diagnostics;
using Cortinho.Models;

namespace Cortinho.Services
{
    public static class ShortcutLauncherService
    {
        /// <summary>Falha ao lançar um atalho — alimenta o status "error" do notch em vez de um MessageBox modal.</summary>
        public static event Action<string>? LaunchFailed;

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
            catch (Exception)
            {
                LaunchFailed?.Invoke($"Não consegui abrir '{item.Name}'");
            }
        }
    }
}
