using System.Diagnostics;
using System.Windows.Threading;

namespace ShortcutLauncher;

/// <summary>
/// Observa periodicamente se algum atalho marcado com TrackDiscordPresence está rodando
/// e atualiza a Rich Presence do Discord de acordo. Acompanha um processo por vez.
/// </summary>
public class GamePresenceWatcher : IDisposable
{
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(5) };
    private readonly DiscordPresenceService _presence;
    private readonly Func<List<ShortcutItem>> _getShortcuts;
    private Process? _activeProcess;

    public GamePresenceWatcher(DiscordPresenceService presence, Func<List<ShortcutItem>> getShortcuts)
    {
        _presence = presence;
        _getShortcuts = getShortcuts;
        _timer.Tick += (_, _) => Poll();
    }

    public void Start() => _timer.Start();

    private void Poll()
    {
        if (_activeProcess is { HasExited: true })
        {
            _presence.ClearPresence();
            _activeProcess = null;
        }

        if (_activeProcess is not null) return;

        foreach (var item in _getShortcuts().Where(s => s.Type == ShortcutType.Application && s.TrackDiscordPresence))
        {
            var exeName = System.IO.Path.GetFileNameWithoutExtension(
                Environment.ExpandEnvironmentVariables(item.Path));

            var match = Process.GetProcessesByName(exeName).FirstOrDefault();
            if (match is null) continue;

            _activeProcess = match;

            DateTime started;
            try { started = match.StartTime.ToUniversalTime(); }
            catch { started = DateTime.UtcNow; }

            _presence.UpdatePresence(
                details: item.Name,
                state: string.IsNullOrWhiteSpace(item.DiscordServerName) ? "Jogando" : item.DiscordServerName,
                startedAtUtc: started);
            break;
        }
    }

    public void Dispose() => _timer.Stop();
}
