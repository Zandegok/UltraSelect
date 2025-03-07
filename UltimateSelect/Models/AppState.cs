namespace UltimateSelect.Models
{
    public enum AppState
    {
        Idle,             // Waiting in the background; only the hotkey manager, hidden window, and tray icon are active.
        OverlayActive,    // The overlay window is displayed for region selection.
        ContextMenuActive,// The context menu is being built and shown (asynchronously loading plugins, etc.).
        ToolActive        // A tool (drawing, pinning, etc.) is active; after the action completes, the app returns to Idle.
    }
}
