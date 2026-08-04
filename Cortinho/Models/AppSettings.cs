namespace Cortinho.Models
{
    public class AppSettings
    {
        // Flag de dev pro pacote design_handoff_overlay (fase 1) — README recomenda deixar atrás
        // de flag até o visual ser aprovado; a MainWindow continua sendo o padrão do Ctrl+Alt+L até então.
        public bool OpenOverlayOnHotkey { get; set; } = false;

        // Config (fase 5) — âncora/monitor do notch. NotchMonitorDeviceName vazio = monitor primário.
        public string NotchAnchor { get; set; } = "Center";
        public string NotchMonitorDeviceName { get; set; } = "";

        // Config (fase 5) — hotkey regravável do launcher/overlay (Ctrl+Alt+<Letra> por enquanto,
        // mesma limitação de FormatHotkey em ShortcutTileDisplayFactory: só dígitos/letras).
        public uint LauncherHotkeyModifiers { get; set; } = 0x0002 | 0x0001; // MOD_CONTROL | MOD_ALT
        public uint LauncherHotkeyKey { get; set; } = 0x4C; // VK_L

        // Fase 6 — "dark" ou "light". High contrast fica fora de escopo por decisão do design system.
        public string Theme { get; set; } = "dark";
    }
}
