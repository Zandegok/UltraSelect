using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace UltimateSelect.Services
{
    public class HotKeyManager : IDisposable
    {
        private HwndSource _source;
        private IntPtr _windowHandle;
        private const int WM_HOTKEY = 0x0312;

        public event EventHandler<HotKeyEventArgs> HotKeyPressed;

        public HotKeyManager(Window hostWindow)
        {
            var helper = new WindowInteropHelper(hostWindow);
            _windowHandle = helper.Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source.AddHook(WndProc);
        }

        public bool RegisterHotKey(Modifiers modifiers, System.Windows.Forms.Keys key, string id)
        {
            // Use id.GetHashCode() as an identifier.
            int hotkeyId = id.GetHashCode();
            bool registered = NativeMethods.RegisterHotKey(_windowHandle, hotkeyId, (uint)modifiers, (uint)key);
            return registered;
        }

        public void UnregisterHotKey(string id)
        {
            int hotkeyId = id.GetHashCode();
            NativeMethods.UnregisterHotKey(_windowHandle, hotkeyId);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY)
            {
                int id = wParam.ToInt32();
                HotKeyPressed?.Invoke(this, new HotKeyEventArgs(id));
                handled = true;
            }
            return IntPtr.Zero;
        }

        public void Dispose()
        {
            if (_source != null)
            {
                _source.RemoveHook(WndProc);
                _source = null;
            }
        }
    }

    // Simple event args for hotkey events.
    public class HotKeyEventArgs : EventArgs
    {
        public int HotKeyId { get; }
        public HotKeyEventArgs(int id)
        {
            HotKeyId = id;
        }
    }

    // Modifier keys enum.
    [Flags]
    public enum Modifiers
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        Win = 8
    }

    internal static class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }
}
