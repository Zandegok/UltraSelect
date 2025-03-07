using System;
using UltimateSelect.Models;

namespace UltimateSelect.Services
{
    public class ApplicationStateService
    {
        private static readonly Lazy<ApplicationStateService> _instance =
            new Lazy<ApplicationStateService>(() => new ApplicationStateService());
        public static ApplicationStateService Instance => _instance.Value;

        public AppState CurrentState { get; private set; } = AppState.Idle;

        public event EventHandler<AppState> StateChanged;

        public void SetState(AppState newState)
        {
            if (CurrentState != newState)
            {
                CurrentState = newState;
                StateChanged?.Invoke(this, newState);
            }
        }

        // Optionally, add methods to trigger state changes:
        public void TriggerOverlay() => SetState(AppState.OverlayActive);
        public void TriggerContextMenu() => SetState(AppState.ContextMenuActive);
        public void TriggerTool() => SetState(AppState.ToolActive);
        public void Reset() => SetState(AppState.Idle);
    }
}
