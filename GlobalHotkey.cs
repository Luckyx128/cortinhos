using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace ShortcutLauncher;

/// <summary>
/// Registra um atalho global do Windows (funciona mesmo com o app em segundo plano
/// ou com um jogo em foco) usando a API nativa RegisterHotKey.
/// </summary>
public class GlobalHotkey : IDisposable
{
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private const int WM_HOTKEY = 0x0312;
    private const int HotkeyId = 9000;

    // Modificadores
    public const uint MOD_ALT = 0x0001;
    public const uint MOD_CONTROL = 0x0002;
    public const uint MOD_SHIFT = 0x0004;
    public const uint MOD_WIN = 0x0008;

    private readonly Window _window;
    private HwndSource? _source;
    private readonly Action _onPressed;

    public GlobalHotkey(Window window, uint modifiers, uint virtualKey, Action onPressed)
    {
        _window = window;
        _onPressed = onPressed;

        _window.SourceInitialized += (_, _) =>
        {
            var helper = new WindowInteropHelper(_window);
            _source = HwndSource.FromHwnd(helper.Handle);
            _source?.AddHook(HwndHook);
            RegisterHotKey(helper.Handle, HotkeyId, modifiers, virtualKey);
        };
    }

    private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == WM_HOTKEY && wParam.ToInt32() == HotkeyId)
        {
            _onPressed();
            handled = true;
        }
        return IntPtr.Zero;
    }

    public void Dispose()
    {
        var helper = new WindowInteropHelper(_window);
        UnregisterHotKey(helper.Handle, HotkeyId);
        _source?.RemoveHook(HwndHook);
    }
}
