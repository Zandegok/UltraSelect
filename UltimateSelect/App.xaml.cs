using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Configuration;
using UltimateSelect.Models;
using UltimateSelect.Services;
using UltimateSelect.Views;
using SW = System.Windows;
using SWForms = System.Windows.Forms;

namespace UltimateSelect
{
	public partial class App : Application
	{
		private HotKeyManager _hotKeyManager;
		private TrayIconManager _trayIconManager;
		private HiddenWindow _hiddenWindow;
		private ContextMenuService _contextMenuService;

		internal static IConfiguration Config { get; private set; }

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			Config = new ConfigurationBuilder()
				 .AddJsonFile("appsettings.json")
				 .Build();

			ApplicationStateService.Instance.SetState(AppState.Idle);

			_trayIconManager = new TrayIconManager();

			_hiddenWindow = new HiddenWindow();
			_hiddenWindow.Show();
			_hiddenWindow.Hide(); // Hidden but provides an HWND.

			_hotKeyManager = new HotKeyManager(_hiddenWindow);
			_hotKeyManager.HotKeyPressed += OnHotKeyPressed;
			_hotKeyManager.RegisterHotKey(Modifiers.Control | Modifiers.Alt, SWForms.Keys.S, "ActivationHotkey");

			_contextMenuService = new ContextMenuService();
			_contextMenuService.ComposePlugins();

			ApplicationStateService.Instance.StateChanged += (s, newState) =>
			{
				System.Diagnostics.Debug.WriteLine($"State changed to: {newState}");
			};
		}

		private void OnHotKeyPressed(object sender, HotKeyEventArgs e)
		{
			if(ApplicationStateService.Instance.CurrentState == AppState.Idle)
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
				ApplicationStateService.Instance.SetState(AppState.ContextMenuActive);
				await ProcessSelectionAsync(selection);
			};
			overlay.Show();
		}

		private async Task ProcessSelectionAsync(SW.Rect selection)
		{
			// Prepare context data for building the context menu.
			var contextData = new ContextMenuData
			{
				SelectedRegion = selection,
				Windows = new WindowManagerService().GetWindowsInRegion(selection)
			};

			// Build the context menu asynchronously.
			var contextMenu = await _contextMenuService.BuildContextMenuAsync(contextData);
			contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Absolute;
			// Place the context menu at the end of the selection (bottom-right corner).
			contextMenu.HorizontalOffset = selection.X + selection.Width;
			contextMenu.VerticalOffset = selection.Y + selection.Height;
			contextMenu.IsOpen = true;

			// Create the frame window that encloses the selected region.
			var frameWindow = new FrameWindow(selection);
			frameWindow.Show();

			// When the context menu is closed, close the frame window.
			contextMenu.Closed += (s, e) =>
			{
				if(frameWindow.IsVisible)
				{
					frameWindow.Close();
				}
				ApplicationStateService.Instance.Reset();
			};

			await Task.CompletedTask;
		}

		protected override void OnExit(ExitEventArgs e)
		{
			_hotKeyManager?.Dispose();
			_trayIconManager?.Dispose();
			base.OnExit(e);
		}
	}
}
