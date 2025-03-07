using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace UltimateSelect.Models
{
    public class ContextMenuContext
    {
        public BitmapSource CapturedImage { get; set; }
        public List<WindowInfo> Windows { get; set; }
        public Rect SelectedRegion { get; set; }
        public Point MenuLocation { get; set; }
        public Action CloseFrameCallback { get; set; }
        // Здесь можно добавлять новые свойства по мере необходимости
    }
}
