using System;
using System.Threading;
using System.Windows;
using System.Diagnostics;
using System.Drawing;
using Gma.System.MouseKeyHook;
using Forms = System.Windows.Forms;

namespace UltimateSelect
{
	public partial class App : Application
	{
		private Mutex _mutex;
		private IKeyboardMouseEvents _globalHook;
		private bool _ctrlPressed = false;
		private bool _altPressed = false;
		private OverlayWindow _overlayWindow;

		public bool IsContextMenuOpen { get; internal set; }

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			// Проверка на дубликаты через Mutex
			bool createdNew;
			_mutex = new Mutex(true, "UltimateSelectAppMutex", out createdNew);
			if(!createdNew)
			{
				Shutdown();
				return;
			}

			// Приложение не закрывается автоматически при отсутствии окон
			this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

			// Создание иконки в системном трее
			Forms.NotifyIcon notifyIcon = new Forms.NotifyIcon();
			notifyIcon.Icon = new Icon("app.ico");
			notifyIcon.Visible = true;
			notifyIcon.Text = "UltimateSelect";

			// Подписка на глобальные события клавиатуры через Gma.System.MouseKeyHook
			_globalHook = Hook.GlobalEvents();
			_globalHook.KeyDown += GlobalHook_KeyDown;
			_globalHook.KeyUp += GlobalHook_KeyUp;
		}

		private void GlobalHook_KeyDown(object sender, Forms.KeyEventArgs e)
		{
			// Обработка нажатия клавиш Ctrl
			if(e.KeyCode == Forms.Keys.ControlKey || e.KeyCode == Forms.Keys.LControlKey || e.KeyCode == Forms.Keys.RControlKey)
				_ctrlPressed = true;

			// Обработка нажатия клавиши Alt (Menu)
			if(e.KeyCode == Forms.Keys.Menu || e.KeyCode == Forms.Keys.LMenu || e.KeyCode == Forms.Keys.RMenu)
				_altPressed = true;

			// Если одновременно нажаты Ctrl и Alt – показываем оверлей, если он ещё не открыт
			TryToOpenOverlay();
		}

		public void TryToOpenOverlay()
		{
			if(_ctrlPressed && _altPressed && _overlayWindow == null && IsContextMenuOpen == false)
			{
				_overlayWindow = new OverlayWindow();
				_overlayWindow.Closed += (s, args) => { _overlayWindow = null; };
				_overlayWindow.Show();
			}
		}

		private void GlobalHook_KeyUp(object sender, Forms.KeyEventArgs e)
		{
			if(e.KeyCode == Forms.Keys.ControlKey || e.KeyCode == Forms.Keys.LControlKey || e.KeyCode == Forms.Keys.RControlKey)
				_ctrlPressed = false;
			if(e.KeyCode == Forms.Keys.Menu || e.KeyCode == Forms.Keys.LMenu || e.KeyCode == Forms.Keys.RMenu)
				_altPressed = false;

			// Если оверлей открыт и выбор ещё не начат – закрываем его
			if(_overlayWindow != null && !_overlayWindow.IsSelectionStarted)
			{
				_overlayWindow.Close();
				_overlayWindow = null;
			}
		}

		protected override void OnExit(ExitEventArgs e)
		{
			if(_globalHook != null)
			{
				_globalHook.KeyDown -= GlobalHook_KeyDown;
				_globalHook.KeyUp -= GlobalHook_KeyUp;
				_globalHook.Dispose();
			}
			base.OnExit(e);
		}
	}
}
