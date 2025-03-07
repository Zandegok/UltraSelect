using System.Collections.Generic;
using System.Windows.Controls;
using UltimateSelect.Models;

namespace UltimateSelect.Plugins
{
    public interface IMenuItemProvider
    {
        /// <summary>
        /// Returns a collection of MenuItems based on the given context.
        /// </summary>
        IEnumerable<MenuItem> GetMenuItems(ContextMenuData context);
    }
}
