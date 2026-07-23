using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace ShortcutLauncher;

public partial class AddShortcutWindow : Window
{
    public ShortcutItem? Result { get; private set; }

    private readonly ShortcutItem? _editing;
    private readonly List<ShortcutItem> _otherShortcuts;

    private uint _capturedModifiers;
    private uint _capturedKey;

    public AddShortcutWindow(ShortcutItem? existing = null, List<ShortcutItem>? allShortcuts = null)
    {
        InitializeComponent();
        TypeBox.SelectedIndex = 0;

        _editing = existing;
        _otherShortcuts = (allShortcuts ?? new List<ShortcutItem>())
            .Where(s => !ReferenceEquals(s, existing))
            .ToList();

        if (existing is not null)
        {
            NameBox.Text = existing.Name;
            PathBox.Text = existing.Path;
            ArgsBox.Text = existing.Arguments;
            foreach (var it in TypeBox.Items)
            {
                if (it is System.Windows.Controls.ComboBoxItem cbi &&
                    (string)cbi.Tag == existing.Type.ToString())
                {
                    TypeBox.SelectedItem = cbi;
                    break;
                }
            }

            _capturedModifiers = existing.HotkeyModifiers;
            _capturedKey = existing.HotkeyKey;
            HotkeyBox.Text = existing.HasHotkey ? existing.HotkeyLabel : "Nenhum";

            DiscordTrackBox.IsChecked = existing.TrackDiscordPresence;
            DiscordServerBox.Text = existing.DiscordServerName;
        }

        UpdateDiscordPanelVisibility();
    }

    private void BrowseClick(object sender, RoutedEventArgs e)
    {
        var selectedType = ((System.Windows.Controls.ComboBoxItem)TypeBox.SelectedItem).Tag as string;

        if (selectedType == "Folder")
        {
            var dialog = new OpenFolderDialog();
            if (dialog.ShowDialog() == true)
                PathBox.Text = dialog.FolderName;
        }
        else
        {
            var dialog = new Microsoft.Win32.OpenFileDialog { Filter = "Executáveis (*.exe)|*.exe|Todos os arquivos (*.*)|*.*" };
            if (dialog.ShowDialog() == true)
                PathBox.Text = dialog.FileName;
        }
    }

    private void TypeBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        UpdateDiscordPanelVisibility();
    }

    private void UpdateDiscordPanelVisibility()
    {
        if (DiscordPanel is null || TypeBox.SelectedItem is null) return;

        var selectedType = ((System.Windows.Controls.ComboBoxItem)TypeBox.SelectedItem).Tag as string;
        var isApplication = selectedType == "Application";

        DiscordPanel.Visibility = isApplication ? Visibility.Visible : Visibility.Collapsed;
        if (!isApplication)
        {
            DiscordTrackBox.IsChecked = false;
        }
    }

    private void DiscordTrackBox_CheckedChanged(object sender, RoutedEventArgs e)
    {
        DiscordServerBox.IsEnabled = DiscordTrackBox.IsChecked == true;
    }

    private void HotkeyBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        e.Handled = true;

        var key = e.Key == Key.System ? e.SystemKey : e.Key;

        if (key == Key.Escape)
        {
            _capturedModifiers = 0;
            _capturedKey = 0;
            HotkeyBox.Text = "Nenhum";
            return;
        }

        if (IsModifierKey(key)) return;

        uint modifiers = 0;
        if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) modifiers |= HotkeyManager.MOD_CONTROL;
        if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt)) modifiers |= HotkeyManager.MOD_ALT;
        if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)) modifiers |= HotkeyManager.MOD_SHIFT;
        if (Keyboard.Modifiers.HasFlag(ModifierKeys.Windows)) modifiers |= HotkeyManager.MOD_WIN;

        if (modifiers == 0)
        {
            HotkeyBox.Text = "Use ao menos um modificador (Ctrl/Alt/Shift/Win)";
            return;
        }

        var vk = (uint)KeyInterop.VirtualKeyFromKey(key);
        _capturedModifiers = modifiers;
        _capturedKey = vk;
        HotkeyBox.Text = HotkeyFormat.Format(modifiers, vk);
    }

    private static bool IsModifierKey(Key key) => key is
        Key.LeftCtrl or Key.RightCtrl or
        Key.LeftAlt or Key.RightAlt or
        Key.LeftShift or Key.RightShift or
        Key.LWin or Key.RWin;

    private void ClearHotkeyClick(object sender, RoutedEventArgs e)
    {
        _capturedModifiers = 0;
        _capturedKey = 0;
        HotkeyBox.Text = "Nenhum";
    }

    private void SaveClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameBox.Text) || string.IsNullOrWhiteSpace(PathBox.Text))
        {
            System.Windows.MessageBox.Show("Preencha nome e caminho.", "Faltou algo",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (_capturedModifiers != 0 && _capturedKey != 0)
        {
            const uint reservedModifiers = HotkeyManager.MOD_CONTROL | HotkeyManager.MOD_ALT;
            var reservedKey = (uint)KeyInterop.VirtualKeyFromKey(Key.L);
            if (_capturedModifiers == reservedModifiers && _capturedKey == reservedKey)
            {
                System.Windows.MessageBox.Show(
                    "Ctrl+Alt+L é reservado pra abrir/fechar o launcher. Escolha outra combinação.",
                    "Atalho reservado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var conflict = _otherShortcuts.FirstOrDefault(s =>
                s.HasHotkey && s.HotkeyModifiers == _capturedModifiers && s.HotkeyKey == _capturedKey);
            if (conflict is not null)
            {
                System.Windows.MessageBox.Show(
                    $"Essa combinação já está em uso pelo atalho \"{conflict.Name}\". Escolha outra.",
                    "Atalho em uso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        var typeTag = ((System.Windows.Controls.ComboBoxItem)TypeBox.SelectedItem).Tag as string;
        Enum.TryParse<ShortcutType>(typeTag, out var type);

        Result = new ShortcutItem
        {
            Name = NameBox.Text.Trim(),
            Type = type,
            Path = PathBox.Text.Trim(),
            Arguments = ArgsBox.Text.Trim(),
            HotkeyModifiers = _capturedModifiers,
            HotkeyKey = _capturedKey,
            TrackDiscordPresence = type == ShortcutType.Application && DiscordTrackBox.IsChecked == true,
            DiscordServerName = DiscordServerBox.Text.Trim()
        };

        DialogResult = true;
    }

    private void CancelClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
