// File: Views/BaseAppWindow.xaml.cs
using System.Windows;
using System.Windows.Input;

namespace UltimateSelect.Views
{
    public partial class BaseAppWindow : Window
    {
        public BaseAppWindow()
        {
            InitializeComponent();
            // Enable window dragging via the title bar.
            TitleBar.MouseLeftButtonDown += TitleBar_MouseLeftButtonDown;
            // Set default button behavior.
            MinimizeButton.Click += (s, e) => this.WindowState = WindowState.Minimized;
            MaximizeButton.Click += (s, e) => ToggleMaximize();
            CloseButton.Click += (s, e) => this.Close();
            // PinButton's behavior should be provided by the consuming window.
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                ToggleMaximize();
            else
                DragMove();
        }

        private void ToggleMaximize()
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }

        // Expose properties to enable/disable buttons.
        public bool IsPinButtonEnabled
        {
            get => PinButton.IsEnabled;
            set => PinButton.IsEnabled = value;
        }

        public bool IsMinimizeButtonEnabled
        {
            get => MinimizeButton.IsEnabled;
            set => MinimizeButton.IsEnabled = value;
        }

        public bool IsMaximizeButtonEnabled
        {
            get => MaximizeButton.IsEnabled;
            set => MaximizeButton.IsEnabled = value;
        }

        public bool IsCloseButtonEnabled
        {
            get => CloseButton.IsEnabled;
            set => CloseButton.IsEnabled = value;
        }
    }
}
