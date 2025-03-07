using System;
using System.Threading.Tasks;
using System.Windows;
using UltimateSelect.Models;               // Contains AppState and ContextMenuData.
using UltimateSelect.Services;             // Contains ApplicationStateService, HotKeyManager, TrayIconManager, ContextMenuService.
using UltimateSelect.Views;                // Contains HiddenWindow and OverlayWindow.
using SW = System.Windows;                // Alias for System.Windows.
using SWForms = System.Windows.Forms;     // Alias for System.Windows.Forms.

namespace UltimateSelect
{
    public partial class App : Application
    {
        private HotKeyManager _hotKeyManager;
        private TrayIconManager _trayIconManager;
        private HiddenWindow _hiddenWindow;
        private ContextMenuService _contextMenuService;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize global state to Idle.
            ApplicationStateService.Instance.SetState(AppState.Idle);

            // Create and initialize the system tray icon.
            _trayIconManager = new TrayIconManager();

            // Create the hidden window for hotkey messages.
            _hiddenWindow = new HiddenWindow();
            _hiddenWindow.Show();
            _hiddenWindow.Hide(); // Remains hidden but provides an Hwnd.

            // Initialize HotKeyManager using the hidden window.
            _hotKeyManager = new HotKeyManager(_hiddenWindow);
            _hotKeyManager.HotKeyPressed += OnHotKeyPressed;
            // Register a default hotkey (Ctrl+Alt+S). This can later be made user configurable.
            _hotKeyManager.RegisterHotKey(Modifiers.Control | Modifiers.Alt, SWForms.Keys.S, "ActivationHotkey");

            // Initialize the ContextMenuService and load available plugins.
            _contextMenuService = new ContextMenuService();
            _contextMenuService.ComposePlugins();

            // Optionally, subscribe to state changes to log or update UI.
            ApplicationStateService.Instance.StateChanged += (s, newState) =>
            {
                System.Diagnostics.Debug.WriteLine($"State changed to: {newState}");
            };
        }

        private void OnHotKeyPressed(object sender, HotKeyEventArgs e)
        {
            // Only trigger selection if the app is in the Idle state.
            if (ApplicationStateService.Instance.CurrentState == AppState.Idle)
            {
                ApplicationStateService.Instance.SetState(AppState.OverlayActive);
                ShowOverlay();
            }
        }

        private void ShowOverlay()
        {
            var overlay = new OverlayWindow();
            overlay.SelectionCompleted += async (s, selection) =>
            {
                // Once the user completes selection, transition to the context menu state.
                ApplicationStateService.Instance.SetState(AppState.ContextMenuActive);
                await ProcessSelectionAsync(selection);
            };
            overlay.Show();
        }

        private async Task ProcessSelectionAsync(SW.Rect selection)
        {
            // Prepare minimal context data for building the context menu.
            var contextData = new ContextMenuData
            {
                SelectedRegion = selection,
                ExtraData = null
            };

            // Asynchronously build the context menu from available plugin providers.
            var contextMenu = await _contextMenuService.BuildContextMenuAsync(contextData);

            // Position the context menu at the center of the selected region.
            contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Absolute;
            contextMenu.HorizontalOffset = selection.X + selection.Width / 2;
            contextMenu.VerticalOffset = selection.Y + selection.Height / 2;
            contextMenu.IsOpen = true;

            // When the context menu is closed, return to the Idle state.
            contextMenu.Closed += (s, e) =>
            {
                ApplicationStateService.Instance.Reset();
            };
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _hotKeyManager?.Dispose();
            _trayIconManager?.Dispose();
            base.OnExit(e);
        }
    }
}
