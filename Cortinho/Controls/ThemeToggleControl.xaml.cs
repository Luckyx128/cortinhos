using Cortinho.Services;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Cortinho.Controls
{
    /// <summary>ThemeToggle.jsx — pill sol/lua. Fase 6: única superfície que troca o tema (Config
    /// tem a mesma ação escondida atrás daqui — não duplicado, é o próprio componente do handoff).</summary>
    public partial class ThemeToggleControl : System.Windows.Controls.UserControl
    {
        public ThemeToggleControl()
        {
            InitializeComponent();
            SetVisual(AppSettingsStore.Load().Theme);
        }

        private void LightButton_Click(object sender, MouseButtonEventArgs e) => SetTheme("light");
        private void DarkButton_Click(object sender, MouseButtonEventArgs e) => SetTheme("dark");

        private void SetTheme(string theme)
        {
            ThemeService.Save(theme);
            ThemeService.Apply(theme);
            SetVisual(theme);
        }

        private void SetVisual(string theme)
        {
            bool light = theme == "light";

            var accent = ResourceBrush("AccentBrush");
            var onAccent = ResourceBrush("TextOnAccentBrush");
            var tertiary = ResourceBrush("TextTertiaryBrush");

            LightBg.Background = light ? accent : System.Windows.Media.Brushes.Transparent;
            DarkBg.Background = !light ? accent : System.Windows.Media.Brushes.Transparent;

            var sunColor = light ? onAccent : tertiary;
            SunIcon1.Fill = sunColor;
            SunIcon2.Fill = sunColor;
            SunIcon3.Fill = sunColor;

            var moonColor = !light ? onAccent : tertiary;
            MoonIcon1.Fill = moonColor;
            MoonIcon2.Fill = moonColor;
        }

        private static System.Windows.Media.Brush ResourceBrush(string key) => (System.Windows.Media.Brush)System.Windows.Application.Current.FindResource(key);
    }
}
