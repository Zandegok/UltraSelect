using SWForms = System.Windows.Forms;
using SD = System.Drawing;
using System;
using System.Windows;
using UltimateSelect.Services;

namespace UltimateSelect.Services
{
    public class TrayIconManager : IDisposable
    {
        private SWForms.NotifyIcon _notifyIcon;

        public TrayIconManager()
        {
            _notifyIcon = new SWForms.NotifyIcon();
            _notifyIcon.Icon = LoadAppIcon();
            _notifyIcon.Visible = true;
            _notifyIcon.Text = "UltimateSelect - " + ApplicationStateService.Instance.CurrentState.ToString();
            _notifyIcon.ContextMenuStrip = BuildTrayContextMenu();

            // Subscribe to state changes to update the tooltip text.
            ApplicationStateService.Instance.StateChanged += (s, state) =>
            {
                _notifyIcon.Text = "UltimateSelect - " + state.ToString();
            };
        }

        private SD.Icon LoadAppIcon()
        {
            try
            {
                // Load icon from embedded resource.
                var uri = new Uri("pack://application:,,,/Resources/app.ico", UriKind.Absolute);
                var streamInfo = Application.GetResourceStream(uri);
                if (streamInfo != null)
                    return new SD.Icon(streamInfo.Stream);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading app icon: " + ex);
            }
            return null;
        }

        private SWForms.ContextMenuStrip BuildTrayContextMenu()
        {
            var menu = new SWForms.ContextMenuStrip();
            var exitItem = new SWForms.ToolStripMenuItem("Exit");
            exitItem.Click += (s, e) => Application.Current.Shutdown();
            menu.Items.Add(exitItem);
            return menu;
        }

        public void Dispose()
        {
            _notifyIcon?.Dispose();
        }
    }
}
