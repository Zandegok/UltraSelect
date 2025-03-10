using System.Windows;
using System.Windows.Media;

namespace UltimateSelect.Views
{
    public class FrameWindow : BaseAppWindow
    {
        /// <summary>
        /// Creates a FrameWindow whose client area (ContentPanel) exactly matches the selected region,
        /// and makes the entire content fully transparent.
        /// </summary>
        /// <param name="selectedRegion">Selected region in screen coordinates.</param>
        public FrameWindow(Rect selectedRegion) : base(selectedRegion)
        {
            // Apply unique FrameWindow properties for full transparency.
            this.AllowsTransparency = true;
            this.Background = Brushes.Transparent;
            this.WindowStyle = WindowStyle.None;
            this.Topmost = true;
            this.ShowInTaskbar = false;

            // Make the inherited content panel fully transparent.
            ContentPanel.Background = Brushes.Transparent;
            // Do not add any extra UI elements.
        }
    }
}
