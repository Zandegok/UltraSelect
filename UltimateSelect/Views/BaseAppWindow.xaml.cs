using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UltimateSelect.Views
{
    public partial class BaseAppWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of BaseAppWindow.
        /// If a selected region (in screen coordinates) is provided, positions the window so that
        /// the client area (ContentPanel) exactly matches that region.
        /// </summary>
        /// <param name="selectedRegion">Optional selected region in screen coordinates.</param>
        public BaseAppWindow(Rect? selectedRegion = null)
        {
            InitializeComponent();

            if (selectedRegion.HasValue)
            {
                // Assume TitleBar height = 30 and Border thickness = 1 on all sides.
                const double titleBarHeight = 30;
                const double borderThickness = 1;

                // The ContentPanel is inside the window's ContentCanvas (which fills the window below the title bar).
                // We want the ContentPanel (client area) to be exactly the same as the selected region.
                // Therefore, position the window such that:
                // Window.Left = selectedRegion.X - borderThickness
                // Window.Top = selectedRegion.Y - titleBarHeight - borderThickness
                // Window.Width = selectedRegion.Width + 2*borderThickness
                // Window.Height = selectedRegion.Height + titleBarHeight + 2*borderThickness
                this.Left = selectedRegion.Value.X - borderThickness;
                this.Top = selectedRegion.Value.Y - titleBarHeight - borderThickness;
                this.Width = selectedRegion.Value.Width + 2 * borderThickness;
                this.Height = selectedRegion.Value.Height + titleBarHeight + 2 * borderThickness;

                // Within the ContentCanvas, position the ContentPanel at (0,0) and size it exactly.
                Canvas.SetLeft(ContentPanel, 0);
                Canvas.SetTop(ContentPanel, 0);
                ContentPanel.Width = selectedRegion.Value.Width;
                ContentPanel.Height = selectedRegion.Value.Height;
            }
        }

        private void PinButton_Click(object sender, RoutedEventArgs e)
        {
            // For demonstration, toggle the Topmost property.
            this.Topmost = !this.Topmost;
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                MaximizeButton_Click(sender, e);
            }
            else
            {
                this.DragMove();
            }
        }
    }
}
