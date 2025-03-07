using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UltimateSelect.Views;

namespace UltimateSelect
{
	public partial class OverlayWindow : Window
	{
		private Point? _startPoint;
		public bool IsSelectionStarted { get; private set; } = false;

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
			IsSelectionStarted = true;
			SelectionRectangle.Visibility = Visibility.Visible;
			Canvas.SetLeft(SelectionRectangle, _startPoint.Value.X);
			Canvas.SetTop(SelectionRectangle, _startPoint.Value.Y);
			SelectionRectangle.Width = 0;
			SelectionRectangle.Height = 0;
			CaptureMouse();
		}

		private void OverlayWindow_MouseMove(object sender, MouseEventArgs e)
		{
			if(_startPoint.HasValue && IsSelectionStarted)
			{
				var currentPoint = e.GetPosition(SelectionCanvas);
				double x = Math.Min(currentPoint.X, _startPoint.Value.X);
				double y = Math.Min(currentPoint.Y, _startPoint.Value.Y);
				double width = Math.Abs(currentPoint.X - _startPoint.Value.X);
				double height = Math.Abs(currentPoint.Y - _startPoint.Value.Y);
				Canvas.SetLeft(SelectionRectangle, x);
				Canvas.SetTop(SelectionRectangle, y);
				SelectionRectangle.Width = width;
				SelectionRectangle.Height = height;
			}
		}

		private void OverlayWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if(_startPoint.HasValue && IsSelectionStarted)
			{
				ReleaseMouseCapture();
				var endPoint = e.GetPosition(SelectionCanvas);
				double x = Math.Min(endPoint.X, _startPoint.Value.X);
				double y = Math.Min(endPoint.Y, _startPoint.Value.Y);
				double width = Math.Abs(endPoint.X - _startPoint.Value.X);
				double height = Math.Abs(endPoint.Y - _startPoint.Value.Y);
				var selectedRegion = new Rect(x, y, width, height);

				// После завершения выделения – закрываем оверлей и обрабатываем выделенную область
				ProcessSelection(selectedRegion);

				Close();
			}
		}

		private void ProcessSelection(Rect selectedRegion)
		{
			// Захват изображения выделенной области экрана
			var capturedImage = UltimateSelect.Helpers.ScreenCaptureHelper.CaptureScreen(selectedRegion);

			// Получение списка окон, пересекающихся с выделенной областью
			var windows = new UltimateSelect.Services.WindowManagerService().GetWindowsInRegion(selectedRegion);

			// Определяем позицию для показа контекстного меню (например, центр выделения)
			var menuLocation = new Point(selectedRegion.X + selectedRegion.Width / 2, selectedRegion.Y + selectedRegion.Height / 2);

			// Формирование контекста для контекстного меню

			// Создание окна FrameWindow с тонкой чёрной рамкой (реализацию FrameWindow нужно определить отдельно)
			var frameWindow = new FrameWindow();
			frameWindow.Left = selectedRegion.X-6;
			frameWindow.Top = selectedRegion.Y-6;
			frameWindow.Width = selectedRegion.Width;
			frameWindow.Height = selectedRegion.Height;
			frameWindow.Show();

			var context = new UltimateSelect.Models.ContextMenuContext
			{
				CapturedImage = capturedImage,
				Windows = windows,
				SelectedRegion = selectedRegion,
				MenuLocation = menuLocation,
				CloseFrameCallback = () =>
					 {
						 frameWindow.Close(); 
                }
			};
			// Построение и показ контекстного меню
			var menuBuilder = new UltimateSelect.Services.ContextMenuBuilder();
			var menu = menuBuilder.BuildMenu(context);
			((App)Application.Current).IsContextMenuOpen = true;
			menu.Closed += (s, args) =>
				{
					((App)Application.Current).IsContextMenuOpen = false;
					((App)Application.Current).TryToOpenOverlay();
				};
			menu.IsOpen = true;
			menu.Focus();

		}
	}
}
