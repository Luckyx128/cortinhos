using System;
using System.Windows;
using System.Windows.Controls;

namespace Cortinho.Overlay.Islands
{
    /// <summary>Ilha Config (PHASE-OVERLAY.md §5.8) — botão único que abre a MainWindow gerenciadora
    /// e fecha o overlay. É a ponte entre as duas superfícies.</summary>
    public partial class ConfigIsland : System.Windows.Controls.UserControl
    {
        public event Action? ConfigClicked;

        public ConfigIsland()
        {
            InitializeComponent();
        }

        private void ConfigButton_Click(object sender, RoutedEventArgs e) => ConfigClicked?.Invoke();
    }
}
