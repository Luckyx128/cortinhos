using System;
using DiscordRPC;

namespace Cortinho.Services
{
    /// <summary>
    /// Wrapper fino sobre o cliente IPC do Discord (DiscordRPC). Fica inerte se nunca
    /// for inicializado (ex: sem Client ID configurado em discord-config.json).
    /// </summary>
    public class DiscordPresenceService : IDisposable
    {
        private DiscordRpcClient? _client;

        public bool IsInitialized => _client != null;

        public void Initialize(string clientId)
        {
            _client = new DiscordRpcClient(clientId);
            _client.Initialize();
        }

        public void UpdatePresence(string details, string state, DateTime startedAtUtc)
        {
            _client?.SetPresence(new RichPresence
            {
                Details = details,
                State = state,
                Timestamps = new Timestamps { Start = startedAtUtc }
            });
        }

        public void ClearPresence()
        {
            _client?.ClearPresence();
        }

        public void Shutdown()
        {
            _client?.Dispose();
            _client = null;
        }

        public void Dispose() => Shutdown();
    }
}
