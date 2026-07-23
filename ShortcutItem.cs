using System.Text.Json.Serialization;

namespace ShortcutLauncher;

public enum ShortcutType
{
    Application,
    Folder,
    Url,
    Command
}

public class ShortcutItem
{
    public string Name { get; set; } = "Novo atalho";
    public ShortcutType Type { get; set; } = ShortcutType.Application;
    public string Path { get; set; } = string.Empty;
    public string Arguments { get; set; } = string.Empty;

    public uint HotkeyModifiers { get; set; } = 0;
    public uint HotkeyKey { get; set; } = 0;

    public bool TrackDiscordPresence { get; set; } = false;
    public string DiscordServerName { get; set; } = string.Empty;

    [JsonIgnore]
    public string TypeLabel => Type switch
    {
        ShortcutType.Application => "App",
        ShortcutType.Folder => "Pasta",
        ShortcutType.Url => "Link",
        ShortcutType.Command => "Comando",
        _ => "?"
    };

    [JsonIgnore]
    public bool HasHotkey => HotkeyModifiers != 0 && HotkeyKey != 0;

    [JsonIgnore]
    public string HotkeyLabel => HotkeyFormat.Format(HotkeyModifiers, HotkeyKey);
}
