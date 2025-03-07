using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Input;

namespace UltimateSelect.Views
{
    public partial class PinWindow : Window
    {
        /// <summary>
        /// Конструктор принимает захваченное изображение и выбранную область.
        /// Окно имеет точно те же размеры, что и выбранная область, и не сдвигает изображение.
        /// Заголовок с кнопками накладывается поверх изображения.
        /// </summary>
        /// <param name="image">Захваченное изображение</param>
        /// <param name="selectedRegion">Выбранная область экрана</param>
        public PinWindow(BitmapSource image, Rect selectedRegion)
        {
            InitializeComponent();
            CapturedImage.Source = image;
            // Окно занимает ровно выбранную область
            this.Left = selectedRegion.X;
            this.Top = selectedRegion.Y;
            this.Width = selectedRegion.Width;
            this.Height = selectedRegion.Height;
        }

        private void CaptionArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                MaximizeButton_Click(sender, e);
            else
                this.DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Normal
                ? WindowState.Maximized
                : WindowState.Normal;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
