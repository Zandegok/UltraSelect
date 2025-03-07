using System;

namespace UltimateSelect.Models
{
    public class WindowInfo
    {
        public IntPtr Handle { get; set; }
        public string Title { get; set; }
        public System.Windows.Rect WindowRect { get; set; }
    }
}
