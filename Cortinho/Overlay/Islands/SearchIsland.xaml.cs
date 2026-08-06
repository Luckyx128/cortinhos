using Cortinho.Models;
using Cortinho.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cortinho.Overlay.Islands
{
    /// <summary>Ilha de Busca (PHASE-OVERLAY.md §5.2 + §6) — a única que tira foco do jogo: clicar na
    /// barra chama IslandWindow.Activate() (ver OverlayController, que também religa Deactivate() no
    /// Window.Deactivated). Filtra ShortcutItem por Contains case-insensitive sobre o nome; QueryChanged
    /// deixa o grid da ilha vizinha filtrar com a mesma consulta.</summary>
    public partial class SearchIsland : System.Windows.Controls.UserControl
    {
        private sealed class ResultItem
        {
            public string Name { get; init; } = "";
            public string IconLayer1 { get; init; } = "";
            public double IconLayer1Opacity { get; init; } = 1;
            public string IconLayer2 { get; init; } = "";
            public double IconLayer2Opacity { get; init; } = 1;
            public string HotkeyLabel { get; init; } = "";
            public Visibility HotkeyVisibility { get; init; } = Visibility.Collapsed;
            public bool IsSelected { get; set; }
            public ShortcutItem Source { get; init; } = null!;
        }

        public event Action<string>? QueryChanged;

        /// <summary>Ver ShortcutGridIsland.ShortcutLaunched — mesmo contrato, disparado no Enter.</summary>
        public event Action? ShortcutLaunched;

        private List<ResultItem> _results = new();
        private int _selectedIndex;

        public SearchIsland()
        {
            InitializeComponent();
        }

        /// <summary>Chamado no Open() do OverlayController — limpa a busca de uma sessão anterior.</summary>
        public void Reset() => SearchBox.Text = "";

        private void SearchBar_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Window.GetWindow(this) is IslandWindow window)
                window.ActivateForInput();
            SearchBox.Focus();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = SearchBox.Text?.Trim() ?? "";
            Placeholder.Visibility = string.IsNullOrEmpty(query) ? Visibility.Visible : Visibility.Collapsed;

            _results = string.IsNullOrEmpty(query)
                ? new List<ResultItem>()
                : ShortcutStore.Load()
                    .Where(s => s.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .Take(4)
                    .Select(ToResultItem)
                    .ToList();

            _selectedIndex = 0;
            ApplySelection();
            ResultsPanel.Visibility = _results.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

            QueryChanged?.Invoke(query);
        }

        private static ResultItem ToResultItem(ShortcutItem item)
        {
            var (l1, l1op, l2, l2op) = ShortcutIconProvider.GetIconLayers(item.Type);
            return new ResultItem
            {
                Name = item.Name,
                IconLayer1 = l1,
                IconLayer1Opacity = l1op,
                IconLayer2 = l2,
                IconLayer2Opacity = l2op,
                HotkeyLabel = ShortcutTileDisplayFactory.FormatHotkey(item.HotkeyModifiers, item.HotkeyKey),
                HotkeyVisibility = item.HasHotkey ? Visibility.Visible : Visibility.Collapsed,
                Source = item,
            };
        }

        private void SearchBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (_results.Count == 0) return;

            switch (e.Key)
            {
                case Key.Down:
                    _selectedIndex = Math.Min(_selectedIndex + 1, _results.Count - 1);
                    ApplySelection();
                    e.Handled = true;
                    break;
                case Key.Up:
                    _selectedIndex = Math.Max(_selectedIndex - 1, 0);
                    ApplySelection();
                    e.Handled = true;
                    break;
                case Key.Enter:
                    ShortcutLauncherService.Launch(_results[_selectedIndex].Source);
                    ShortcutLaunched?.Invoke();
                    e.Handled = true;
                    break;
            }
        }

        /// <summary>Reatribui ItemsSource (null, depois de volta) em vez de Items.Refresh() — o mesmo
        /// truque simples usado nas outras ilhas (nenhuma delas usa INotifyPropertyChanged), só que
        /// aqui precisa forçar a regeneração dos containers pra reavaliar o DataTrigger de IsSelected.</summary>
        private void ApplySelection()
        {
            for (int i = 0; i < _results.Count; i++)
                _results[i].IsSelected = i == _selectedIndex;
            ResultsHost.ItemsSource = null;
            ResultsHost.ItemsSource = _results;
        }
    }
}
