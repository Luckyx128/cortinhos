using Cortinho.Models;
using Cortinho.Services;
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
        public ShortcutGridIsland()
        {
            InitializeComponent();
            Refresh();
        }

        public void Refresh()
        {
            var items = ShortcutStore.Load();
            TilesGrid.ItemsSource = items.Select(ShortcutTileDisplayFactory.Create).ToList();
            CountLabel.Text = $" · {items.Count}";
        }

        private void ShortcutTile_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is ShortcutTileDisplayItem item && item.Source != null)
                ShortcutLauncherService.Launch(item.Source);
        }
    }
}
