using System;
using System.Collections.Generic;
using System.Windows;
using UltimateSelect.Helpers;
using UltimateSelect.Models;

namespace UltimateSelect.Services
{
    public class WindowManagerService
    {
        public List<WindowInfo> GetWindowsInRegion(Rect selectionRegion)
        {
            List<WindowInfo> windows = new List<WindowInfo>();

            IntPtr hWnd = Win32Interop.GetTopWindow(IntPtr.Zero);
            while (hWnd != IntPtr.Zero)
            {
                if (Win32Interop.IsWindowVisible(hWnd))
                {
                    string title = Win32Interop.GetWindowTitle(hWnd);
                    if (!string.IsNullOrWhiteSpace(title))
                    {
                        int exStyle = Win32Interop.GetWindowLong(hWnd, Win32Interop.GWL_EXSTYLE);
                        if ((exStyle & Win32Interop.WS_EX_TOOLWINDOW) == 0)
                        {
                            Rect winRect = Win32Interop.GetWindowRect(hWnd);
                            if (selectionRegion.IntersectsWith(winRect))
                            {
                                windows.Add(new WindowInfo
                                {
                                    Handle = hWnd,
                                    Title = title,
                                    WindowRect = winRect
                                });
                            }
                        }
                    }
                }
                hWnd = Win32Interop.GetWindow(hWnd, Win32Interop.GW_HWNDNEXT);
            }
            // Порядок обхода соответствует Z-очереди – сортировка не нужна.
            return windows;
        }
    }
}
