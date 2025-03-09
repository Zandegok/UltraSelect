using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Threading.Tasks;
using System.Windows.Controls;
using UltimateSelect.Models;
using UltimateSelect.Plugins;
using SW = System.Windows;

namespace UltimateSelect.Services
{
    public class ContextMenuService
    {
        [ImportMany]
        public IEnumerable<IPluginActionProvider> PluginActionProviders { get; set; }

        public void ComposePlugins()
        {
            // Build the path to the Plugins folder next to the executable.
            string pluginsPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Plugins");
            if (!System.IO.Directory.Exists(pluginsPath))
            {
                System.IO.Directory.CreateDirectory(pluginsPath);
            }

            // Use an AggregateCatalog to include both the host assembly and the Plugins folder.
            var aggregateCatalog = new AggregateCatalog();
            aggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(ContextMenuService).Assembly));
            aggregateCatalog.Catalogs.Add(new DirectoryCatalog(pluginsPath));

            var container = new CompositionContainer(aggregateCatalog);
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
                    // Iterate over the list of KeyValuePair items.
                    foreach (var kvp in output.MenuActions)
                    {
                        var menuItem = new SW.Controls.MenuItem { Header = kvp.Key };
                        menuItem.Click += (s, e) => kvp.Value.Invoke();
                        menu.Items.Add(menuItem);
                    }
                }
            }

            menu.Closed += (s, e) =>
            {
                // Cleanup code if needed.
            };

            return menu;
        }
    }
}
