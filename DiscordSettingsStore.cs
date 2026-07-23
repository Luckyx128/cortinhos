using System.IO;
using System.Text.Json;

namespace ShortcutLauncher;

public class DiscordSettings
{
    public string ClientId { get; set; } = string.Empty;
    public bool Enabled { get; set; } = false;
}

public static class DiscordSettingsStore
{
    private static readonly string ConfigPath = System.IO.Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "discord-config.json");

    public static DiscordSettings Load()
    {
        try
        {
            if (!File.Exists(ConfigPath))
                return new DiscordSettings();

            var json = File.ReadAllText(ConfigPath);
            return JsonSerializer.Deserialize<DiscordSettings>(json) ?? new DiscordSettings();
        }
        catch
        {
            return new DiscordSettings();
        }
    }
}
