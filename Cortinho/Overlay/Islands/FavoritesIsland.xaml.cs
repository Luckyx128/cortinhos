using Cortinho.Models;
using Cortinho.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cortinho.Overlay.Islands
{
    /// <summary>Ilha Favoritos (PHASE-OVERLAY.md §5.5) — até 5 ShortcutItem com Pinned == true.</summary>
    public partial class FavoritesIsland : System.Windows.Controls.UserControl
    {
        /// <summary>Ver ShortcutGridIsland.ShortcutLaunched — mesmo contrato, o OverlayController
        /// fecha o overlay ao lançar um atalho pra devolver o foco pro app recém-aberto.</summary>
        public event Action? ShortcutLaunched;

        private sealed class FavoriteDisplayItem
        {
            public string Name { get; init; } = "";
            public string IconLayer1 { get; init; } = "";
            public double IconLayer1Opacity { get; init; } = 1;
            public string IconLayer2 { get; init; } = "";
            public double IconLayer2Opacity { get; init; } = 1;
            public ShortcutItem Source { get; init; } = null!;
        }

        public FavoritesIsland()
        {
            InitializeComponent();
            Refresh();
        }

        public void Refresh()
        {
            var favorites = ShortcutStore.Load().Where(s => s.Pinned).Take(5).Select(s =>
            {
                var (l1, l1op, l2, l2op) = ShortcutIconProvider.GetIconLayers(s.Type);
                return new FavoriteDisplayItem
                {
                    Name = s.Name,
                    IconLayer1 = l1,
                    IconLayer1Opacity = l1op,
                    IconLayer2 = l2,
                    IconLayer2Opacity = l2op,
                    Source = s
                };
            }).ToList();

            ItemsHost.ItemsSource = favorites;
            EmptyLabel.Visibility = favorites.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void FavoriteButton_Click(object sender, MouseButtonEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is FavoriteDisplayItem item)
            {
                ShortcutLauncherService.Launch(item.Source);
                ShortcutLaunched?.Invoke();
            }
        }
    }
}
