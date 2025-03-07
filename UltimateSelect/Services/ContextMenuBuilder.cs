using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using UltimateSelect.Helpers;
using UltimateSelect.Models;
using UltimateSelect.Views;

namespace UltimateSelect.Services
{
	public class ContextMenuBuilder
	{
		// Вспомогательный класс-обёртка для флага
		public ContextMenu BuildMenu(ContextMenuContext context)
		{
			ContextMenu menu = new ContextMenu();

			// Добавляем раздел "Работа с окнами"
			menu.Items.Add(BuildWindowOperationsMenu(context));

			// Добавляем раздел "Фрагмент"
			menu.Items.Add(BuildImageFragmentMenu(context));

			// Добавляем раздел "Рисование"
			menu.Items.Add(BuildDrawingMenu(context));

			menu.Items.Add(BuildTextElementsMenu(context));
			menu.Items.Add(BuildImageElementsMenu(context));
			menu.Items.Add(BuildLinkElementsMenu(context));
			menu.Items.Add(BuildControlElementsMenu(context));

			// Пункт "Отмена"
			MenuItem cancelItem = new MenuItem { Header = "Отмена" };
			cancelItem.Click += (s, e) =>
			{
				menu.IsOpen = false;
			};
			menu.Items.Add(cancelItem);

			// Если меню закрывается без выполнения действий, вызывается callback
			menu.Closed += (s, e) =>
			{
				context.CloseFrameCallback?.Invoke();
			};

			// Устанавливаем расположение меню
			menu.Placement = System.Windows.Controls.Primitives.PlacementMode.Absolute;
			menu.HorizontalOffset = context.MenuLocation.X;
			menu.VerticalOffset = context.MenuLocation.Y;

			return menu;
		}

		private MenuItem BuildWindowOperationsMenu(ContextMenuContext context)
		{
			MenuItem windowMenu = new MenuItem { Header = "Работа с окнами" };

			// Пункт "Свернуть все"
			MenuItem minimizeItem = new MenuItem { Header = "Свернуть все" };
			minimizeItem.Click += (s, e) =>
			{
				foreach(var win in context.Windows)
				{
					Win32Interop.ShowWindow(win.Handle, Win32Interop.SW_MINIMIZE);
				}
			};
			windowMenu.Items.Add(minimizeItem);

			// Пункт "Закрыть все"
			MenuItem closeItem = new MenuItem { Header = "Закрыть все" };
			closeItem.Click += (s, e) =>
			{
				foreach(var win in context.Windows)
				{
					Win32Interop.PostMessage(win.Handle, Win32Interop.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
				}
			};
			windowMenu.Items.Add(closeItem);

			// Добавляем пункты для каждого окна из списка
			foreach(var win in context.Windows)
			{
				MenuItem winItem = new MenuItem { Header = win.Title, Tag = win };
				winItem.Click += (s, e) =>
				{
					if(s is MenuItem mi && mi.Tag is WindowInfo winInfo)
					{
						Win32Interop.SetForegroundWindow(winInfo.Handle);
					}
				};
				windowMenu.Items.Add(winItem);
			}

			return windowMenu;
		}

		private MenuItem BuildImageFragmentMenu(ContextMenuContext context)
		{
			MenuItem fragmentMenu = new MenuItem { Header = "Фрагмент" };

			// Пункт "Копировать как изображение"
			MenuItem copyImageItem = new MenuItem { Header = "Копировать как изображение" };
			copyImageItem.Click += (s, e) =>
			{
				Clipboard.SetImage(context.CapturedImage);
				context.CloseFrameCallback?.Invoke();
			};
			fragmentMenu.Items.Add(copyImageItem);

			// Пункт "Сохранить как изображение..."
			MenuItem saveImageItem = new MenuItem { Header = "Сохранить как изображение..." };
			saveImageItem.Click += (s, e) =>
			{
				var dlg = new Microsoft.Win32.SaveFileDialog { Filter = "PNG Image|*.png|JPEG Image|*.jpg" };
				if(dlg.ShowDialog() == true)
				{
					using(var fileStream = new System.IO.FileStream(dlg.FileName, System.IO.FileMode.Create))
					{
						BitmapEncoder encoder = dlg.FilterIndex == 1
									 ? (BitmapEncoder)new PngBitmapEncoder()
									 : new JpegBitmapEncoder();
						encoder.Frames.Add(BitmapFrame.Create(context.CapturedImage));
						encoder.Save(fileStream);
					}
				}
			};
			fragmentMenu.Items.Add(saveImageItem);

			// Пункт "Закрепить на экране"
			MenuItem pinItem = new MenuItem { Header = "Закрепить на экране" };
			pinItem.Click += (s, e) =>
			{
				var pinWindow = new PinWindow(context.CapturedImage, context.SelectedRegion);
				pinWindow.Show();
				context.CloseFrameCallback?.Invoke();
			};
			fragmentMenu.Items.Add(pinItem);

			return fragmentMenu;
		}

		private MenuItem BuildDrawingMenu(ContextMenuContext context)
		{
			MenuItem drawingMenu = new MenuItem { Header = "Рисование" };

			// Пункт "Карандаш"
			MenuItem pencilItem = new MenuItem { Header = "Карандаш" };
			pencilItem.Click += (s, e) =>
			{
				var drawingWindow = new UltimateSelect.Views.DrawingWindow(context.CapturedImage);
				drawingWindow.Show();
			};
			drawingMenu.Items.Add(pencilItem);

			// Пункт "Очистить рисунок"
			MenuItem clearDrawingItem = new MenuItem { Header = "Очистить рисунок" };
			clearDrawingItem.Click += (s, e) =>
			{
				MessageBox.Show("Функция очистки рисунка пока не реализована.");
			};
			drawingMenu.Items.Add(clearDrawingItem);

			// Пункт "Сохранить рисунок..."
			MenuItem saveDrawingItem = new MenuItem { Header = "Сохранить рисунок..." };
			saveDrawingItem.Click += (s, e) =>
			{
				MessageBox.Show("Функция сохранения рисунка пока не реализована.");
			};
			drawingMenu.Items.Add(saveDrawingItem);

			return drawingMenu;
		}

		private MenuItem CreateGroupedMenu(string header, List<UIElementInfo> elements, Func<UIElementInfo, MenuItem> createSubItem)
		{
			MenuItem categoryMenu = new MenuItem { Header = header };

			// Группируем элементы по Framework
			var groups = elements.GroupBy(e => e.Framework);
			foreach(var group in groups)
			{
				MenuItem frameworkMenu = new MenuItem { Header = $"Framework: {group.Key}" };
				foreach(var element in group)
				{
					frameworkMenu.Items.Add(createSubItem(element));
				}
				categoryMenu.Items.Add(frameworkMenu);
			}
			return categoryMenu;
		}

		// Меню для текстовых элементов.
		public MenuItem BuildTextElementsMenu(ContextMenuContext context)
		{
			if(context.Windows == null || !context.Windows.Any())
				return new MenuItem { Header = "Текст" };

			WindowInfo topWindow = context.Windows.First();
			List<UIElementInfo> textElements = UIAutomationHelper.GetTextElementsFromRegion(topWindow, context.SelectedRegion);

			return CreateGroupedMenu("Текст", textElements, element =>
			{
				MenuItem item = new MenuItem
					{
						Header = $"{element.Name} - {element.TextContent}",
						Tag = element
					};
				item.Click += (s, e) =>
					{
					 UIAutomationHelper.SetFocus(element);
				 };
				return item;
			});
		}

		// Меню для элементов-картинок. Для картинки добавляется иконка (здесь используется placeholder).
		public MenuItem BuildImageElementsMenu(ContextMenuContext context)
		{
			if(context.Windows == null || !context.Windows.Any())
				return new MenuItem { Header = "Картинки" };

			WindowInfo topWindow = context.Windows.First();
			List<UIElementInfo> imageElements = UIAutomationHelper.GetImageElementsFromRegion(topWindow, context.SelectedRegion);

			return CreateGroupedMenu("Картинки", imageElements, element =>
			{
				MenuItem item = new MenuItem { Tag = element };

				// Заголовок пункта – имя элемента.
				item.Header = $"{element.Name}";

				// Для картинки задаем иконку. Здесь используется placeholder. 
				// В реальном проекте можно реализовать генерацию thumbnail для элемента.
				Image icon = new Image();
				icon.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/placeholder.png", UriKind.Absolute));
				item.Icon = icon;

				item.Click += (s, e) =>
					{
					 UIAutomationHelper.SetFocus(element);
				 };
				return item;
			});
		}

		// Меню для элементов-ссылок. Ссылки отображаются в виде гиперссылки внутри TextBlock.
		public MenuItem BuildLinkElementsMenu(ContextMenuContext context)
		{
			if(context.Windows == null || !context.Windows.Any())
				return new MenuItem { Header = "Ссылки" };

			WindowInfo topWindow = context.Windows.First();
			List<UIElementInfo> linkElements = UIAutomationHelper.GetLinkElementsFromRegion(topWindow, context.SelectedRegion);

			return CreateGroupedMenu("Ссылки", linkElements, element =>
			{
				MenuItem item = new MenuItem { Tag = element };

				// Создаем TextBlock с гиперссылкой
				TextBlock tb = new TextBlock();
				Hyperlink hl = new Hyperlink(new Run(element.TextContent))
					{
					// Если доступен URL, его можно задать здесь.
						NavigateUri = new Uri("http://example.com")
					};
				tb.Inlines.Add(hl);
				item.Header = tb;

				item.Click += (s, e) =>
					{
					 UIAutomationHelper.SetFocus(element);
				 };
				return item;
			});
		}

		// Меню для интерактивных элементов управления.
		public MenuItem BuildControlElementsMenu(ContextMenuContext context)
		{
			if(context.Windows == null || !context.Windows.Any())
				return new MenuItem { Header = "Элементы управления" };

			WindowInfo topWindow = context.Windows.First();
			List<UIElementInfo> controlElements = UIAutomationHelper.GetControlElementsFromRegion(topWindow, context.SelectedRegion);

			return CreateGroupedMenu("Элементы управления", controlElements, element =>
			{
				MenuItem item = new MenuItem
					{
						Header = $"{element.Name} - {element.TextContent}",
						Tag = element
					};
				item.Click += (s, e) =>
					{
					 UIAutomationHelper.SetFocus(element);
				 };
				return item;
			});
		}
	}

}

