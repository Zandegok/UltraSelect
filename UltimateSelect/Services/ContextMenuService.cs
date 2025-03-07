using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using UltimateSelect.Models;
using UltimateSelect.Plugins;
using SW = System.Windows; // Alias for System.Windows

namespace UltimateSelect.Services
{
    public class ContextMenuService
    {
        [ImportMany]
        public IEnumerable<IMenuItemProviderLite> MenuItemProviders { get; set; }

        public void ComposePlugins()
        {
            string pluginFolder = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Plugins");
            if (!Directory.Exists(pluginFolder))
            {
                Directory.CreateDirectory(pluginFolder);
            }
            var catalog = new DirectoryCatalog(pluginFolder);
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
        }

        public async Task<SW.Controls.ContextMenu> BuildContextMenuAsync(ContextMenuData context)
        {
            var menu = new SW.Controls.ContextMenu();
            var tasks = MenuItemProviders.Select(provider => provider.GetMenuItemsAsync(context));
            var results = await Task.WhenAll(tasks);
            foreach (var providerItems in results)
            {
                foreach (var itemData in providerItems)
                {
                    // Convert MenuItemData to a WPF MenuItem.
                    var menuItem = new SW.Controls.MenuItem { Header = itemData.Header };
                    // Optionally load icon if IconUri is provided.
                    if (!string.IsNullOrEmpty(itemData.IconUri))
                    {
                        var image = new SW.Controls.Image
                        {
                            Source = new SW.Media.Imaging.BitmapImage(new System.Uri(itemData.IconUri, System.UriKind.RelativeOrAbsolute))
                        };
                        menuItem.Icon = image;
                    }
                    // Hook up command logic, e.g., using a DelegateCommand (not shown here) or simple event handler.
                    menuItem.Click += (s, e) =>
                    {
                        // Execute command based on CommandName/Parameter.
                    };
                    menu.Items.Add(menuItem);
                }
            }

            menu.Closed += (s, e) =>
            {
                ApplicationStateService.Instance.SetState(AppState.Idle);
            };

            return menu;
        }
    }
}
