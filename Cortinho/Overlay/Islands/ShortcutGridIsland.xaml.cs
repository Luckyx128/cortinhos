using Cortinho.Models;
using Cortinho.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Cortinho.Overlay.Islands
{
    /// <summary>Ilha do grid de atalhos (PHASE-OVERLAY.md §5.1) — única ilha da fase 1. Sem tile
    /// "Adicionar" e sem menu de contexto aqui (editar/remover dependem do diálogo da MainWindow,
    /// que ainda não existe — fase 4 do pacote do notch); só abrir por clique, igual ao grid da janela.</summary>
    public partial class ShortcutGridIsland : System.Windows.Controls.UserControl
    {
        /// <summary>Disparado depois de um Launch bem-sucedido — o OverlayController fecha o overlay
        /// nesse momento pra devolver o foco pro app recém-aberto (pedido do usuário em teste ao vivo:
        /// antes o overlay ficava aberto por cima, competindo pelo foco com o app que acabou de abrir).</summary>
        public event Action? ShortcutLaunched;

        public ShortcutGridIsland()
        {
            InitializeComponent();
            Refresh();
        }

        public void Refresh() => Filter("");

        /// <summary>Mesma consulta da SearchIsland (PHASE-OVERLAY.md §5.2: "o grid filtra junto") —
        /// Contains case-insensitive sobre o nome, igual à busca da MainWindow.</summary>
        public void Filter(string query)
        {
            var items = ShortcutStore.Load();
            CountLabel.Text = $" · {items.Count}";

            var filtered = string.IsNullOrEmpty(query)
                ? items
                : items.Where(s => s.Name.Contains(query, System.StringComparison.OrdinalIgnoreCase)).ToList();

            TilesGrid.ItemsSource = filtered.Select(ShortcutTileDisplayFactory.Create).ToList();
        }

        private void ShortcutTile_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is ShortcutTileDisplayItem item && item.Source != null)
            {
                ShortcutLauncherService.Launch(item.Source);
                ShortcutLaunched?.Invoke();
            }
        }
    }
}
