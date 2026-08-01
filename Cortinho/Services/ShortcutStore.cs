using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Cortinho.Models;

namespace Cortinho.Services
{
    public static class ShortcutStore
    {
        private static readonly string ConfigPath = Path.Combine(AppContext.BaseDirectory, "shortcuts.json");
        private static readonly string SamplePath = Path.Combine(AppContext.BaseDirectory, "shortcuts.sample.json");

        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public static List<ShortcutItem> Load()
        {
            try
            {
                if (!File.Exists(ConfigPath))
                {
                    if (File.Exists(SamplePath))
                        File.Copy(SamplePath, ConfigPath);
                    else
                        return new List<ShortcutItem>();
                }

                var json = File.ReadAllText(ConfigPath);
                return JsonSerializer.Deserialize<List<ShortcutItem>>(json, Options) ?? new List<ShortcutItem>();
            }
            catch
            {
                return new List<ShortcutItem>();
            }
        }

        public static void Save(List<ShortcutItem> items)
        {
            var json = JsonSerializer.Serialize(items, Options);
            File.WriteAllText(ConfigPath, json);
        }
    }
}
