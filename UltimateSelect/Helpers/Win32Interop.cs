using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace UltimateSelect.Helpers
{
    public static class Win32Interop
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        // Перечисление окон (можно использовать для обратной совместимости, но теперь мы будем обходить в Z-очереди)
        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        // Получение размера окна
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;  
            public int Top;   
            public int Right; 
            public int Bottom;
        }

        // Установка активного окна
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        // Функция получения прямоугольника окна
        public static Rect GetWindowRect(IntPtr hWnd)
        {
            if (GetWindowRect(hWnd, out RECT rect))
                return new Rect(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
            return Rect.Empty;
        }

        // Функция получения заголовка окна
        public static string GetWindowTitle(IntPtr hWnd)
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            if (GetWindowText(hWnd, Buff, nChars) > 0)
                return Buff.ToString();
            return string.Empty;
        }

        // Получение верхнего окна в Z-очереди
        [DllImport("user32.dll")]
        public static extern IntPtr GetTopWindow(IntPtr hWnd);

        // Получение следующего окна в Z-очереди
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
        public const uint GW_HWNDNEXT = 2;

        // Получение стиля окна
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_TOOLWINDOW = 0x00000080;

        // Для сворачивания окна
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        public const int SW_MINIMIZE = 6;
        public const int SW_RESTORE = 9;

        // Для закрытия окна
        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        public const uint WM_CLOSE = 0x0010;
    }
}
