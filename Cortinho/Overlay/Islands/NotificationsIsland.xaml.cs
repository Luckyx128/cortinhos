using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Cortinho.Services;

namespace Cortinho.Overlay.Islands
{
    /// <summary>Ilha Notificações (PHASE-OVERLAY.md §5.4) — instância própria de NotificationService
    /// (mesmo raciocínio do PerfIsland: cada superfície pede acesso e escuta mudanças por conta própria;
    /// UserNotificationListener.Current é da própria API do Windows, então duas instâncias de serviço não
    /// conflitam). Sem allow-list — toda notificação do Windows chega aqui, como já decidido no notch.</summary>
    public partial class NotificationsIsland : System.Windows.Controls.UserControl
    {
        private sealed class NotificationDisplayItem
        {
            public string Title { get; init; } = "";
            public string Body { get; init; } = "";
            public string TimeLabel { get; init; } = "";
            public BitmapImage? Icon { get; init; }
        }

        private readonly NotificationService _notificationService = new();
        private bool _initialized;

        public NotificationsIsland()
        {
            InitializeComponent();
        }

        /// <summary>Chamado toda vez que o overlay abre — inicializa (uma vez) e sempre busca o estado atual.</summary>
        public async void Refresh()
        {
            if (!_initialized)
            {
                _initialized = true;
                bool available = await _notificationService.InitializeAsync();
                if (!available) return;

                // A subscrição vive pelo resto do processo, mas o overlay passa quase todo o tempo
                // fechado — sem esse gate a ilha recarregaria (e redecodificaria ícones) a cada
                // notificação do Windows com ninguém olhando. Ao reabrir, o Refresh() busca tudo de novo.
                _notificationService.NotificationsChanged += (_, _) =>
                    Dispatcher.Invoke(() => { if (IsVisible) _ = RefreshAsync(); });
            }

            await RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            var items = await _notificationService.GetNotificationsAsync();

            // Teto de 8: decodificar o logo de cada notificação é uma ida ao WinRT + decode de bitmap,
            // e sem limite uma caixa de notificações cheia fazia dezenas disso a cada mudança. A ilha
            // rola, mas 8 já enche a área visível de 168 DIPs com folga.
            var displayItems = new List<NotificationDisplayItem>();
            foreach (var item in items.Take(8))
            {
                displayItems.Add(new NotificationDisplayItem
                {
                    Title = item.Title,
                    Body = item.Body,
                    TimeLabel = item.Time.ToLocalTime().ToString("HH:mm"),
                    Icon = item.LogoRef != null ? await LoadIconAsync(item.LogoRef) : null
                });
            }

            HeaderText.Text = items.Count == 0 ? "NOTIFICAÇÕES" : $"NOTIFICAÇÕES · {items.Count}";
            ItemsHost.ItemsSource = displayItems;
            EmptyLabel.Visibility = displayItems.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private static async Task<BitmapImage?> LoadIconAsync(global::Windows.Storage.Streams.RandomAccessStreamReference streamRef)
        {
            try
            {
                using var stream = await streamRef.OpenReadAsync();
                var reader = new global::Windows.Storage.Streams.DataReader(stream);
                await reader.LoadAsync((uint)stream.Size);
                var bytes = new byte[stream.Size];
                reader.ReadBytes(bytes);

                using var ms = new System.IO.MemoryStream(bytes);
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = ms;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        private void ClearNotifications_Click(object sender, MouseButtonEventArgs e)
        {
            _notificationService.ClearAll();
            _ = RefreshAsync();
        }
    }
}
