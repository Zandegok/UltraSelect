using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;

namespace UltimateSelect.Views
{
    public partial class DrawingWindow : Window
    {
        private bool isDrawing = false;
        private Point startPoint;
        private Polyline currentLine;

        public DrawingWindow(BitmapSource image)
        {
            InitializeComponent();
            BaseImage.Source = image;
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DrawingCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDrawing = true;
            startPoint = e.GetPosition(DrawingCanvas);
            currentLine = new Polyline
            {
                Stroke = Brushes.Red,
                StrokeThickness = 2,
                StrokeLineJoin = PenLineJoin.Round
            };
            currentLine.Points.Add(startPoint);
            DrawingCanvas.Children.Add(currentLine);
            DrawingCanvas.CaptureMouse();
        }

        private void DrawingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing && currentLine != null)
            {
                Point pos = e.GetPosition(DrawingCanvas);
                currentLine.Points.Add(pos);
            }
        }

        private void DrawingCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDrawing = false;
            DrawingCanvas.ReleaseMouseCapture();
        }
    }
}
