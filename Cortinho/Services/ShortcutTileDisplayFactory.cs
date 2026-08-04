using Cortinho.Models;

namespace Cortinho.Services
{
    /// <summary>Monta um ShortcutTileDisplayItem a partir de um ShortcutItem — mesma lógica pra
    /// MainWindow (grid da janela) e ShortcutGridIsland (grid do overlay), fonte única.</summary>
    public static class ShortcutTileDisplayFactory
    {
        private const string PinIconPath = "m16.219 4.838l2.964 2.967c2.012 2.014 3.018 3.021 2.784 4.107c-.235 1.085-1.567 1.585-4.23 2.586l-1.845.693c-.713.268-1.07.402-1.345.64q-.181.158-.322.352c-.212.297-.313.664-.515 1.4c-.46 1.672-.69 2.508-1.239 2.821c-.23.132-.492.2-.758.2c-.63 0-1.243-.614-2.469-1.84l-1.466-1.468l-1.079-1.08L5.285 14.8c-1.218-1.219-1.827-1.828-1.83-2.455a1.53 1.53 0 0 1 .203-.773c.313-.543 1.143-.772 2.803-1.23c.737-.203 1.105-.304 1.402-.517q.199-.144.36-.332c.236-.278.368-.637.63-1.355l.669-1.823c.987-2.693 1.48-4.04 2.568-4.28s2.102.774 4.129 2.803";

        public static ShortcutTileDisplayItem Create(ShortcutItem item)
        {
            var (l1, l1op, l2, l2op) = ShortcutIconProvider.GetIconLayers(item.Type);
            var (coverKey, glow) = item.Type switch
            {
                ShortcutType.Application => ("TileGradientAppBrush", System.Windows.Media.Color.FromRgb(0x3E, 0xA6, 0xFF)),
                ShortcutType.Folder => ("TileGradientFolderBrush", System.Windows.Media.Color.FromRgb(0xF0, 0xA9, 0x2C)),
                ShortcutType.Url => ("TileGradientUrlBrush", System.Windows.Media.Color.FromRgb(0x22, 0xC5, 0x5E)),
                ShortcutType.Command => ("TileGradientCommandBrush", System.Windows.Media.Color.FromRgb(0x8B, 0x5C, 0xF6)),
                _ => ("TileGradientAppBrush", System.Windows.Media.Color.FromRgb(0x3E, 0xA6, 0xFF)),
            };

            return new ShortcutTileDisplayItem
            {
                Name = item.Name,
                TypeLabelUpper = item.TypeLabel.ToUpperInvariant(),
                CoverBrush = (System.Windows.Media.Brush)System.Windows.Application.Current.FindResource(coverKey),
                GlowColor = glow,
                GhostLetter = item.Name.Length > 0 ? item.Name[0].ToString().ToUpperInvariant() : "?",
                IconLayer1 = l1,
                IconLayer1Opacity = l1op,
                IconLayer2 = l2,
                IconLayer2Opacity = l2op,
                PinIconData = PinIconPath,
                PinnedVisibility = item.Pinned ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed,
                HotkeyLabel = FormatHotkey(item.HotkeyModifiers, item.HotkeyKey),
                HotkeyVisibility = item.HasHotkey ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed,
                Source = item,
            };
        }

        /// <summary>Usado no rótulo de hotkey do tile e no badge da Config (regravar hotkey, fase 5).</summary>
        public static string FormatHotkey(uint modifiers, uint key)
        {
            if (modifiers == 0 || key == 0) return "";

            var parts = new System.Collections.Generic.List<string>();
            if ((modifiers & Native.NativeMethods.MOD_CONTROL) != 0) parts.Add("Ctrl");
            if ((modifiers & Native.NativeMethods.MOD_ALT) != 0) parts.Add("Alt");
            if ((modifiers & Native.NativeMethods.MOD_SHIFT) != 0) parts.Add("Shift");

            // VK codes de dígitos/letras batem com o ASCII de '0'-'9'/'A'-'Z' — cobre o caso comum
            // (é tudo que o rebind de hotkey grava — MainWindow.xaml.cs, RegravarButton_Click).
            if (key is >= 0x30 and <= 0x5A) parts.Add(((char)key).ToString());

            return string.Join("+", parts);
        }
    }
}
