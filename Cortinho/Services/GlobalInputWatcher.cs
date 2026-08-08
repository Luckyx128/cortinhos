using System.Diagnostics;
using System.Runtime.InteropServices;
using Cortinho.Native;

namespace Cortinho.Services
{
    public class GlobalInputWatcher : IDisposable
    {
        public event Action<System.Windows.Point>? MouseDown;
        public event Action? EscapePressed;

        /// <summary>Qualquer tecla (vkCode). Existe porque as janelas do overlay são WS_EX_NOACTIVATE
        /// e portanto nunca recebem KeyDown do WPF — o hook global é o único caminho.</summary>
        public event Action<uint>? KeyPressed;

        private readonly NativeMethods.HookProc _mouseProc;
        private readonly NativeMethods.HookProc _keyboardProc;
        private IntPtr _mouseHook = IntPtr.Zero;
        private IntPtr _keyboardHook = IntPtr.Zero;

        public GlobalInputWatcher()
        {
            _mouseProc = MouseHookCallback;
            _keyboardProc = KeyboardHookCallback;

            using var curProcess = Process.GetCurrentProcess();
            using var curModule = curProcess.MainModule;

            _mouseHook = NativeMethods.SetWindowsHookEx(NativeMethods.WH_MOUSE_LL, _mouseProc, IntPtr.Zero, 0);
            _keyboardHook = NativeMethods.SetWindowsHookEx(NativeMethods.WH_KEYBOARD_LL, _keyboardProc, IntPtr.Zero, 0);
        }

        private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam.ToInt32() == NativeMethods.WM_LBUTTONDOWN)
            {
                var hookStruct = Marshal.PtrToStructure<NativeMethods.MSLLHOOKSTRUCT>(lParam);
                MouseDown?.Invoke(new System.Windows.Point(hookStruct.pt.X, hookStruct.pt.Y));
            }
            return NativeMethods.CallNextHookEx(_mouseHook, nCode, wParam, lParam);
        }

        private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam.ToInt32() == NativeMethods.WM_KEYDOWN)
            {
                var hookStruct = Marshal.PtrToStructure<NativeMethods.KBDLLHOOKSTRUCT>(lParam);
                if (hookStruct.vkCode == NativeMethods.VK_ESCAPE)
                    EscapePressed?.Invoke();
                KeyPressed?.Invoke(hookStruct.vkCode);
            }
            return NativeMethods.CallNextHookEx(_keyboardHook, nCode, wParam, lParam);
        }

        public void Dispose()
        {
            if (_mouseHook != IntPtr.Zero) NativeMethods.UnhookWindowsHookEx(_mouseHook);
            if (_keyboardHook != IntPtr.Zero) NativeMethods.UnhookWindowsHookEx(_keyboardHook);
        }
    }
}
