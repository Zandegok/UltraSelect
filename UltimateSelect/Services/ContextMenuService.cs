// File: Services/ContextMenuService.cs
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Threading.Tasks;
using System.Windows.Controls;
using UltimateSelect.Models;
using UltimateSelect.Plugins;
using SW = System.Windows; // alias for System.Windows

namespace UltimateSelect.Services
{
    public class ContextMenuService
    {
        [ImportMany]
        public IEnumerable<IPluginActionProvider> PluginActionProviders { get; set; }

        public void ComposePlugins()
        {
            // Compose plugins from a folder called "Plugins" next to the executable.
            var catalog = new DirectoryCatalog(System.AppDomain.CurrentDomain.BaseDirectory + "\\Plugins");
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
        }

        public async Task<SW.Controls.ContextMenu> BuildContextMenuAsync(ContextMenuData context)
        {
            var menu = new SW.Controls.ContextMenu();

            foreach (var provider in PluginActionProviders)
            {
                if (provider.IsApplicable(context))
                {
                    var output = await provider.GetPluginActionAsync(context);
                    // Create menu items from the dictionary of actions.
                    foreach (var kvp in output.MenuActions)
                    {
                        var menuItem = new SW.Controls.MenuItem { Header = kvp.Key };
                        menuItem.Click += (s, e) => kvp.Value.Invoke();
                        menu.Items.Add(menuItem);
                    }
                    // Optionally, you might also register the window type and parameters with a window manager service.
                    // For instance:
                    // WindowManagerService.Instance.RegisterPluginWindow(output.WindowType, output.InitializationParameters);
                }
            }

            // When the context menu is closed, you may perform cleanup.
            menu.Closed += (s, e) =>
            {
                // Cleanup code here (if needed) and update global state.
            };

            return menu;
        }
    }
}
