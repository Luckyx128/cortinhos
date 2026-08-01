using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cortinho.Services
{
    public sealed record NotificationItem(
        uint Id,
        string Title,
        string Body,
        DateTimeOffset Time,
        global::Windows.Storage.Streams.RandomAccessStreamReference? LogoRef);

    public sealed class NotificationService
    {
        private global::Windows.UI.Notifications.Management.UserNotificationListener? _listener;

        public bool IsAvailable { get; private set; }

        public event EventHandler? NotificationsChanged;

        public async Task<bool> InitializeAsync()
        {
            try
            {
                _listener = global::Windows.UI.Notifications.Management.UserNotificationListener.Current;
                var status = await _listener.RequestAccessAsync();
                IsAvailable = status == global::Windows.UI.Notifications.Management.UserNotificationListenerAccessStatus.Allowed;
            }
            catch
            {
                IsAvailable = false;
            }

            if (IsAvailable && _listener != null)
                _listener.NotificationChanged += (_, _) => NotificationsChanged?.Invoke(this, EventArgs.Empty);

            return IsAvailable;
        }

        public async Task<IReadOnlyList<NotificationItem>> GetNotificationsAsync()
        {
            if (!IsAvailable || _listener == null)
                return Array.Empty<NotificationItem>();

            var notifications = await _listener.GetNotificationsAsync(global::Windows.UI.Notifications.NotificationKinds.Toast);

            var items = new List<NotificationItem>();
            foreach (var n in notifications.OrderByDescending(n => n.CreationTime))
            {
                var binding = n.Notification.Visual.GetBinding(global::Windows.UI.Notifications.KnownNotificationBindings.ToastGeneric);
                var texts = binding?.GetTextElements();

                string title = texts is { Count: > 0 } ? texts[0].Text : n.AppInfo.DisplayInfo.DisplayName;
                string body = texts is { Count: > 1 } ? texts[1].Text : "";

                global::Windows.Storage.Streams.RandomAccessStreamReference? logoRef = null;
                try { logoRef = n.AppInfo.DisplayInfo.GetLogo(new global::Windows.Foundation.Size(16, 16)); }
                catch { /* alguns apps não expõem logo — item fica sem ícone */ }

                items.Add(new NotificationItem(n.Id, title, body, n.CreationTime, logoRef));
            }

            return items;
        }

        public void ClearAll()
        {
            try { _listener?.ClearNotifications(); }
            catch { /* sem identidade/acesso — não há o que limpar */ }
        }
    }
}
