using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ShortcutLauncher;

public partial class MainWindow : Window
{
    private const int ToggleHotkeyId = 9000;
    private const int ShortcutHotkeyIdBase = 9001;

    private List<ShortcutItem> _shortcuts = new();
    private readonly HotkeyManager _hotkeyManager;
    private readonly Dictionary<ShortcutItem, int> _shortcutHotkeyIds = new();

    private readonly DiscordPresenceService _discordPresence = new();
    private readonly GamePresenceWatcher _presenceWatcher;

    public MainWindow()
    {
        InitializeComponent();

        _shortcuts = ShortcutStore.Load();
        RenderShortcuts();

        _hotkeyManager = new HotkeyManager(this);
        _presenceWatcher = new GamePresenceWatcher(_discordPresence, () => _shortcuts);

        SourceInitialized += (_, _) =>
        {
            _hotkeyManager.Register(
                ToggleHotkeyId,
                HotkeyManager.MOD_CONTROL | HotkeyManager.MOD_ALT,
                (uint)KeyInterop.VirtualKeyFromKey(Key.L),
                () => ((App)System.Windows.Application.Current).ShowLauncher());

            ReloadShortcutHotkeys();
            StartDiscordPresence();
        };

        // Fecha a janela real só esconde ela (o app continua na bandeja)
        Closing += (_, e) =>
        {
            e.Cancel = true;
            Hide();
        };
    }

    private void StartDiscordPresence()
    {
        var settings = DiscordSettingsStore.Load();
        if (!settings.Enabled || string.IsNullOrWhiteSpace(settings.ClientId)) return;

        _discordPresence.Initialize(settings.ClientId);
        _presenceWatcher.Start();
    }

    private void ReloadShortcutHotkeys()
    {
        _hotkeyManager.UnregisterAll(except: new[] { ToggleHotkeyId });
        _shortcutHotkeyIds.Clear();

        var failures = new List<string>();
        int nextId = ShortcutHotkeyIdBase;

        foreach (var item in _shortcuts.Where(s => s.HasHotkey))
        {
            int id = nextId++;
            if (_hotkeyManager.Register(id, item.HotkeyModifiers, item.HotkeyKey, () => Launch(item)))
                _shortcutHotkeyIds[item] = id;
            else
                failures.Add($"{item.Name} ({item.HotkeyLabel})");
        }

        if (failures.Count > 0)
        {
            var message = "Não consegui registrar o atalho global de: " + string.Join(", ", failures) +
                           " (combinação já em uso por outro programa).";

            if (IsVisible)
                System.Windows.MessageBox.Show(message, "Atalho indisponível", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
                ((App)System.Windows.Application.Current).ShowNotification("Atalho indisponível", message);
        }
    }

    private void RenderShortcuts()
    {
        ShortcutsPanel.Children.Clear();

        foreach (var item in _shortcuts)
        {
            var btn = new System.Windows.Controls.Button
            {
                Style = (Style)FindResource("TileButton"),
                Content = BuildTileContent(item),
                Tag = item
            };
            btn.Click += (_, _) => Launch(item);

            var contextMenu = new ContextMenu();
            var editItem = new MenuItem { Header = "Editar" };
            editItem.Click += (_, _) => EditShortcut(item);
            var removeItem = new MenuItem { Header = "Remover" };
            removeItem.Click += (_, _) => RemoveShortcut(item);
            contextMenu.Items.Add(editItem);
            contextMenu.Items.Add(removeItem);
            btn.ContextMenu = contextMenu;

            ShortcutsPanel.Children.Add(btn);
        }

        var addBtn = new System.Windows.Controls.Button
        {
            Style = (Style)FindResource("TileButton"),
            Content = "+ Adicionar"
        };
        addBtn.Click += (_, _) => AddShortcut();
        ShortcutsPanel.Children.Add(addBtn);
    }

    private static StackPanel BuildTileContent(ShortcutItem item)
    {
        var panel = new StackPanel();
        panel.Children.Add(new TextBlock
        {
            Text = item.Name,
            FontWeight = FontWeights.SemiBold,
            TextWrapping = TextWrapping.Wrap,
            TextAlignment = TextAlignment.Center
        });
        panel.Children.Add(new TextBlock
        {
            Text = item.TypeLabel,
            Foreground = System.Windows.Media.Brushes.Gray,
            FontSize = 11,
            HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
            Margin = new Thickness(0, 4, 0, 0)
        });

        if (item.HasHotkey)
        {
            panel.Children.Add(new TextBlock
            {
                Text = item.HotkeyLabel,
                Foreground = System.Windows.Media.Brushes.Gray,
                FontSize = 11,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                Margin = new Thickness(0, 2, 0, 0)
            });
        }

        return panel;
    }

    private void Launch(ShortcutItem item)
    {
        var path = Environment.ExpandEnvironmentVariables(item.Path);

        try
        {
            switch (item.Type)
            {
                case ShortcutType.Application:
                    Process.Start(new ProcessStartInfo(path, item.Arguments) { UseShellExecute = true });
                    break;
                case ShortcutType.Folder:
                    Process.Start(new ProcessStartInfo("explorer.exe", path) { UseShellExecute = true });
                    break;
                case ShortcutType.Url:
                    Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                    break;
                case ShortcutType.Command:
                    Process.Start(new ProcessStartInfo("cmd.exe", $"/c {path} {item.Arguments}")
                    {
                        UseShellExecute = true,
                        CreateNoWindow = true
                    });
                    break;
            }
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Não consegui executar '{item.Name}':\n{ex.Message}",
                "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void AddShortcut()
    {
        var dialog = new AddShortcutWindow(null, _shortcuts);
        if (dialog.ShowDialog() == true && dialog.Result is not null)
        {
            _shortcuts.Add(dialog.Result);
            ShortcutStore.Save(_shortcuts);
            RenderShortcuts();
            ReloadShortcutHotkeys();
        }
    }

    private void EditShortcut(ShortcutItem item)
    {
        var dialog = new AddShortcutWindow(item, _shortcuts);
        if (dialog.ShowDialog() == true && dialog.Result is not null)
        {
            var index = _shortcuts.IndexOf(item);
            _shortcuts[index] = dialog.Result;
            ShortcutStore.Save(_shortcuts);
            RenderShortcuts();
            ReloadShortcutHotkeys();
        }
    }

    private void RemoveShortcut(ShortcutItem item)
    {
        _shortcuts.Remove(item);
        ShortcutStore.Save(_shortcuts);
        RenderShortcuts();
        ReloadShortcutHotkeys();
    }

    public void Cleanup()
    {
        _hotkeyManager.Dispose();
        _presenceWatcher.Dispose();
        _discordPresence.Dispose();
    }
}
