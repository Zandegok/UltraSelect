using System.Collections.Generic;
using System.Threading.Tasks;
using UltimateSelect.Models;

namespace UltimateSelect.Plugins
{
    public interface IMenuItemProviderLite
    {
        /// <summary>
        /// Asynchronously returns menu item data based on the given context.
        /// </summary>
        Task<IEnumerable<MenuItemData>> GetMenuItemsAsync(ContextMenuData context);
    }
}
