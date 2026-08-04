using System.Windows;

namespace Cortinho.Models
{
    /// <summary>Dados prontos pra exibir um ShortcutItem num ShortcutTile — usado tanto pelo grid da
    /// MainWindow quanto pela ilha do overlay (ShortcutGridIsland). Ver ShortcutTileDisplayFactory.</summary>
    public sealed class ShortcutTileDisplayItem
    {
        public string Name { get; init; } = "";
        public string TypeLabelUpper { get; init; } = "";
        public System.Windows.Media.Brush CoverBrush { get; init; } = System.Windows.Media.Brushes.Transparent;
        public System.Windows.Media.Color GlowColor { get; init; } = System.Windows.Media.Colors.Transparent;
        public string GhostLetter { get; init; } = "";
        public string IconLayer1 { get; init; } = "";
        public double IconLayer1Opacity { get; init; } = 1;
        public string IconLayer2 { get; init; } = "";
        public double IconLayer2Opacity { get; init; } = 1;
        public string PinIconData { get; init; } = "";
        public Visibility PinnedVisibility { get; init; } = Visibility.Collapsed;
        public string HotkeyLabel { get; init; } = "";
        public Visibility HotkeyVisibility { get; init; } = Visibility.Collapsed;
        public bool IsAddTile { get; init; }
        public ShortcutItem? Source { get; init; }
    }
}
