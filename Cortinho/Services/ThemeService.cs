using System.Windows;

namespace Cortinho.Services
{
    /// <summary>Troca Tokens.Dark.xaml ⇄ Tokens.Light.xaml em runtime (fase 6) — o dicionário de cor
    /// fica sempre no mesmo índice de Application.Resources.MergedDictionaries (ver App.xaml), então
    /// trocar tema é só substituir a entrada nesse índice, sem precisar recriar nada.</summary>
    public static class ThemeService
    {
        private const int ColorDictionaryIndex = 1; // App.xaml: [0]=Tokens.Shared, [1]=Tokens.Dark/Light, [2]=Controls

        /// <summary>MainWindow escuta isso pra sincronizar DWMWA_USE_IMMERSIVE_DARK_MODE — sem isso a
        /// Mica real (área central, ver ApplyMicaBackdrop) continua tingindo escuro mesmo no tema claro,
        /// já que esse flag do DWM é quem decide o tom da Mica, independente dos ResourceDictionary da app.</summary>
        public static event Action<string>? ThemeChanged;

        public static void Apply(string theme)
        {
            var uri = theme == "light"
                ? new Uri("Theme/Tokens.Light.xaml", UriKind.Relative)
                : new Uri("Theme/Tokens.Dark.xaml", UriKind.Relative);

            System.Windows.Application.Current.Resources.MergedDictionaries[ColorDictionaryIndex] = new ResourceDictionary { Source = uri };
            ThemeChanged?.Invoke(theme);
        }

        public static void ApplySaved()
        {
            var settings = AppSettingsStore.Load();
            Apply(settings.Theme);
        }

        public static void Save(string theme)
        {
            var settings = AppSettingsStore.Load();
            settings.Theme = theme;
            AppSettingsStore.Save(settings);
        }
    }
}
