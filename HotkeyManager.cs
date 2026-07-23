using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace ShortcutLauncher;

/// <summary>
/// Gerencia múltiplos atalhos globais do Windows (funcionam mesmo com o app em segundo plano
/// ou com um jogo em foco) usando a API nativa RegisterHotKey. Cada combinação é identificada
/// por um id escolhido pelo chamador (ver MainWindow para o esquema de ids usado).
/// </summary>
public class HotkeyManager : IDisposable
{
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private const int WM_HOTKEY = 0x0312;

    // Modificadores
    public const uint MOD_ALT = 0x0001;
    public const uint MOD_CONTROL = 0x0002;
    public const uint MOD_SHIFT = 0x0004;
    public const uint MOD_WIN = 0x0008;

    private readonly Window _window;
    private HwndSource? _source;
    private IntPtr _hwnd;
    private readonly Dictionary<int, Action> _callbacks = new();

    public HotkeyManager(Window window)
    {
        _window = window;

        _window.SourceInitialized += (_, _) =>
        {
            var helper = new WindowInteropHelper(_window);
            _hwnd = helper.Handle;
            _source = HwndSource.FromHwnd(_hwnd);
            _source?.AddHook(HwndHook);
        };
    }

    /// <summary>Tenta registrar a combinação. Retorna false se o Windows recusou (já em uso por outro app).</summary>
    public bool Register(int id, uint modifiers, uint virtualKey, Action onPressed)
    {
        if (_hwnd == IntPtr.Zero) return false;

        if (!RegisterHotKey(_hwnd, id, modifiers, virtualKey))
            return false;

        _callbacks[id] = onPressed;
        return true;
    }

    public void Unregister(int id)
    {
        if (_hwnd == IntPtr.Zero) return;
        if (!_callbacks.Remove(id)) return;
        UnregisterHotKey(_hwnd, id);
    }

    public void UnregisterAll(IEnumerable<int>? except = null)
    {
        var keep = except is null ? new HashSet<int>() : new HashSet<int>(except);
        foreach (var id in _callbacks.Keys.Where(id => !keep.Contains(id)).ToList())
            Unregister(id);
    }

    private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == WM_HOTKEY && _callbacks.TryGetValue(wParam.ToInt32(), out var callback))
        {
            callback();
            handled = true;
        }
        return IntPtr.Zero;
    }

    public void Dispose()
    {
        UnregisterAll();
        _source?.RemoveHook(HwndHook);
    }
}

public static class HotkeyFormat
{
    public static string Format(uint modifiers, uint virtualKey)
    {
        if (modifiers == 0 || virtualKey == 0) return "Nenhum";

        var parts = new List<string>();
        if ((modifiers & HotkeyManager.MOD_CONTROL) != 0) parts.Add("Ctrl");
        if ((modifiers & HotkeyManager.MOD_ALT) != 0) parts.Add("Alt");
        if ((modifiers & HotkeyManager.MOD_SHIFT) != 0) parts.Add("Shift");
        if ((modifiers & HotkeyManager.MOD_WIN) != 0) parts.Add("Win");

        var key = KeyInterop.KeyFromVirtualKey((int)virtualKey);
        parts.Add(key.ToString());

        return string.Join("+", parts);
    }
}
