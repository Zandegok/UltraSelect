using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UltimateSelect.Views
{
	public partial class OverlayWindow : Window
	{
		private Point? _startPoint = null;
		public event EventHandler<Rect> SelectionCompleted;

		public OverlayWindow()
		{
			InitializeComponent();
			this.MouseLeftButtonDown += OverlayWindow_MouseLeftButtonDown;
			this.MouseMove += OverlayWindow_MouseMove;
			this.MouseLeftButtonUp += OverlayWindow_MouseLeftButtonUp;
		}

		private void OverlayWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			_startPoint = e.GetPosition(SelectionCanvas);
			SelectionRectangle.Visibility = Visibility.Visible;
			Canvas.SetLeft(SelectionRectangle, _startPoint.Value.X);
			Canvas.SetTop(SelectionRectangle, _startPoint.Value.Y);
			SelectionRectangle.Width = 0;
			SelectionRectangle.Height = 0;
			CaptureMouse();
		}

		private void OverlayWindow_MouseMove(object sender, MouseEventArgs e)
		{
			if(_startPoint.HasValue)
			{
				var pos = e.GetPosition(SelectionCanvas);
				double x = Math.Min(pos.X, _startPoint.Value.X);
				double y = Math.Min(pos.Y, _startPoint.Value.Y);
				double width = Math.Abs(pos.X - _startPoint.Value.X);
				double height = Math.Abs(pos.Y - _startPoint.Value.Y);
				Canvas.SetLeft(SelectionRectangle, x);
				Canvas.SetTop(SelectionRectangle, y);
				SelectionRectangle.Width = width;
				SelectionRectangle.Height = height;
			}
		}

		private void OverlayWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if(_startPoint.HasValue)
			{
				var endPoint = e.GetPosition(SelectionCanvas);
				double x = Math.Min(endPoint.X, _startPoint.Value.X);
				double y = Math.Min(endPoint.Y, _startPoint.Value.Y);
				double width = Math.Abs(endPoint.X - _startPoint.Value.X);
				double height = Math.Abs(endPoint.Y - _startPoint.Value.Y);

				// The selection in overlay's local (device-independent) coordinates.
				Rect localRect = new Rect(x, y, width, height);

				// Convert the top-left point to screen coordinates.
				Point screenTopLeft = this.PointToScreen(new Point(localRect.X, localRect.Y));

				// Convert screen coordinates back to WPF coordinates.
				PresentationSource source = PresentationSource.FromVisual(this);
				if(source != null)
				{
					Matrix transform = source.CompositionTarget.TransformFromDevice;
					Point wpfTopLeft = transform.Transform(screenTopLeft);

					// Build a corrected rectangle using the converted top-left point.
					localRect = new Rect(wpfTopLeft, localRect.Size);

				}

				ReleaseMouseCapture();
				Close();
				SelectionCompleted?.Invoke(this, localRect);
			}
		}
	}
}
