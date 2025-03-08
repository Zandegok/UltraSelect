// File: SamplePlugins/WindowsActionsPlugin.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UltimateSelect.Models;
using UltimateSelect.Plugins;

namespace SamplePlugins
{
    public class WindowsActionsPlugin : IPluginActionProvider
    {
        public bool IsApplicable(ContextMenuData context)
        {
            // Applicable only if there is at least one window.
            return context.Windows != null && context.Windows.Any();
        }

        public async Task<PluginActionOutput> GetPluginActionAsync(ContextMenuData context)
        {
            // Simulate asynchronous work.
            await Task.Yield();

            var output = new PluginActionOutput();

            // Define menu actions.
            output.MenuActions.Add("Minimize All", () =>
            {
                // Iterate over windows and minimize them.
                foreach (var win in context.Windows)
                {
                    // Your minimization logic here, e.g.:
                    // Win32Interop.ShowWindow(win.Handle, Win32Interop.SW_MINIMIZE);
                }
            });
            output.MenuActions.Add("Close All", () =>
            {
                foreach (var win in context.Windows)
                {
                    // Your close window logic here.
                }
            });

            // For each individual window, add an action.
            foreach (var win in context.Windows)
            {
                output.MenuActions.Add($"Activate: {win.Title}", () =>
                {
                    // Logic to bring this window to the foreground.
                });
            }
            return output;
        }
    }
}
