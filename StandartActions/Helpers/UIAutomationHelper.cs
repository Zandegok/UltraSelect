using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Documents;
using UIA = System.Windows.Automation;
using UltimateSelect.Models;

namespace UltimateSelect.Helpers
{
	// Контейнер для хранения информации об элементе интерфейса.
	public class UIElementInfo
	{
		public string Name { get; set; }
		public string TextContent { get; set; }
		public UIA.AutomationElement AutomationElement { get; set; }
		public string Framework { get; set; }
		public UIA.ControlType ControlType { get; set; }
	}

	public static class UIAutomationHelper
	{
		/// <summary>
		/// Получает все UI-элементы окна, пересекающиеся с заданным регионом.
		/// </summary>
		public static List<UIElementInfo> GetVisualElementsFromRegion(WindowInfo topWindow, Rect region)
		{
			var list = new List<UIElementInfo>();

			// Получаем AutomationElement для указанного окна.
			UIA.AutomationElement windowElement = UIA.AutomationElement.FromHandle(topWindow.Handle);
			if(windowElement == null)
				return list;

			// Фильтруем только элементы управления.
			UIA.Condition condition = new UIA.PropertyCondition(UIA.AutomationElement.IsControlElementProperty, true);
			UIA.AutomationElementCollection elements = windowElement.FindAll(UIA.TreeScope.Descendants, condition);

			foreach(UIA.AutomationElement element in elements)
			{
				// Получаем bounding rectangle элемента.
				Rect elementRect = element.Current.BoundingRectangle;

				// Вычисляем пересечение выделенной области и прямоугольника элемента.
				Rect intersection = Rect.Intersect(region, elementRect);

				// Если пересечение не пустое, вычисляем отношение площади пересечения к площади элемента.
				if(!intersection.IsEmpty && elementRect.Width > 0 && elementRect.Height > 0)
				{
					double intersectionArea = intersection.Width * intersection.Height;
					double elementArea = elementRect.Width * elementRect.Height;
					double ratio = intersectionArea / elementArea;

					// Добавляем элемент, если пересечение составляет не менее 80% площади элемента.
					if(ratio >= 0.8)
					{
						string name = element.Current.Name;
						string textContent = string.Empty;
						object patternObj;

						// Пытаемся получить текст через ValuePattern (например, для текстовых полей)
						if(element.TryGetCurrentPattern(UIA.ValuePattern.Pattern, out patternObj))
						{
							if(patternObj is UIA.ValuePattern valuePattern)
							{
								textContent = valuePattern.Current.Value;
							}
						}
						// Или через TextPattern (например, для текстовых блоков)
						else if(element.TryGetCurrentPattern(UIA.TextPattern.Pattern, out patternObj))
						{
							if(patternObj is UIA.TextPattern textPattern && textPattern.DocumentRange != null)
							{
								textContent = textPattern.DocumentRange.GetText(-1);
							}
						}

						list.Add(new UIElementInfo
						{
							Name = name,
							TextContent = textContent,
							AutomationElement = element,
							Framework = element.Current.FrameworkId,
							ControlType = element.Current.ControlType
						});
					}
				}
			}
			return list;
		}

		// Выбираем только текстовые элементы (например, метки, текстовые блоки).
		public static List<UIElementInfo> GetTextElementsFromRegion(WindowInfo topWindow, Rect region)
		{
			return GetVisualElementsFromRegion(topWindow, region)
				 .Where(e => e.ControlType.Equals(UIA.ControlType.Text))
				 .ToList();
		}

		// Выбираем элементы-картинки.
		public static List<UIElementInfo> GetImageElementsFromRegion(WindowInfo topWindow, Rect region)
		{
			return GetVisualElementsFromRegion(topWindow, region)
				 .Where(e => e.ControlType.Equals(UIA.ControlType.Image))
				 .ToList();
		}

		// Выбираем элементы-ссылки.
		public static List<UIElementInfo> GetLinkElementsFromRegion(WindowInfo topWindow, Rect region)
		{
			return GetVisualElementsFromRegion(topWindow, region)
				 .Where(e => e.ControlType.Equals(UIA.ControlType.Hyperlink))
				 .ToList();
		}

		// Выбираем контролируемые элементы (интерактивные, за исключая текст, картинки и ссылки).
		public static List<UIElementInfo> GetControlElementsFromRegion(WindowInfo topWindow, Rect region)
		{
			return GetVisualElementsFromRegion(topWindow, region)
				 .Where(e => !e.ControlType.Equals(UIA.ControlType.Text)
							 && !e.ControlType.Equals(UIA.ControlType.Image)
							 && !e.ControlType.Equals(UIA.ControlType.Hyperlink))
				 .ToList();
		}

		/// <summary>
		/// Устанавливает фокус на элемент, представленный UIElementInfo.
		/// </summary>
		public static void SetFocus(UIElementInfo elementInfo)
		{
			if(elementInfo?.AutomationElement != null)
			{
				try
				{
					elementInfo.AutomationElement.SetFocus();
				}
				catch(Exception ex)
				{
					// Log the error for debugging purposes
					System.Diagnostics.Debug.WriteLine($"[UIAutomationHelper] Error setting focus on element '{elementInfo.Name}': {ex}");
				}
			}
		}
	}

	// Билдер меню для создания разделенных по категориям меню с группировкой по Framework.
}
