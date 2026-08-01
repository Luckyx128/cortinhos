using System.IO;
using System.Text.Json;
using Cortinho.Models;

namespace Cortinho.Services
{
    public static class DiscordSettingsStore
    {
        private static readonly string ConfigPath = Path.Combine(AppContext.BaseDirectory, "discord-config.json");
        private static readonly string SamplePath = Path.Combine(AppContext.BaseDirectory, "discord-config.sample.json");

        public static DiscordSettings Load()
        {
            try
            {
                if (!File.Exists(ConfigPath))
                {
                    if (File.Exists(SamplePath))
                        File.Copy(SamplePath, ConfigPath);
                    else
                        return new DiscordSettings();
                }

                var json = File.ReadAllText(ConfigPath);
                return JsonSerializer.Deserialize<DiscordSettings>(json) ?? new DiscordSettings();
            }
            catch
            {
                return new DiscordSettings();
            }
        }

        public static void Save(DiscordSettings settings)
        {
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigPath, json);
        }
    }
}
