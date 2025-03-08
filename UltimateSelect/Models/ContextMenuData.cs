// File: Models/ContextMenuData.cs
using System.Collections.Generic;
using System.Windows;

namespace UltimateSelect.Models
{
    public class ContextMenuData
    {
        public Rect SelectedRegion { get; set; }
        public List<WindowInfo> Windows { get; set; } = new List<WindowInfo>();
        public Dictionary<string, object> CustomData { get; set; } = new Dictionary<string, object>();
    }
}
