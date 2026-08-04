using System.IO;
using System.Text.Json;
using Cortinho.Models;

namespace Cortinho.Services
{
    public static class AppSettingsStore
    {
        private static readonly string ConfigPath = Path.Combine(AppContext.BaseDirectory, "app-settings.json");
        private static readonly string SamplePath = Path.Combine(AppContext.BaseDirectory, "app-settings.sample.json");

        private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

        public static AppSettings Load()
        {
            try
            {
                if (!File.Exists(ConfigPath))
                {
                    if (File.Exists(SamplePath))
                        File.Copy(SamplePath, ConfigPath);
                    else
                        return new AppSettings();
                }

                var json = File.ReadAllText(ConfigPath);
                return JsonSerializer.Deserialize<AppSettings>(json, Options) ?? new AppSettings();
            }
            catch
            {
                return new AppSettings();
            }
        }

        public static void Save(AppSettings settings)
        {
            var json = JsonSerializer.Serialize(settings, Options);
            File.WriteAllText(ConfigPath, json);
        }
    }
}
