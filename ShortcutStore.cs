using System.IO;
using System.Text.Json;

namespace ShortcutLauncher;

public static class ShortcutStore
{
    private static readonly string ConfigPath = System.IO.Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "shortcuts.json");

    public static List<ShortcutItem> Load()
    {
        try
        {
            if (!File.Exists(ConfigPath))
            {
                var sample = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "shortcuts.sample.json");
                if (File.Exists(sample))
                    File.Copy(sample, ConfigPath);
                else
                    return new List<ShortcutItem>();
            }

            var json = File.ReadAllText(ConfigPath);
            return JsonSerializer.Deserialize<List<ShortcutItem>>(json) ?? new List<ShortcutItem>();
        }
        catch
        {
            return new List<ShortcutItem>();
        }
    }

    public static void Save(List<ShortcutItem> items)
    {
        var json = JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ConfigPath, json);
    }
}
