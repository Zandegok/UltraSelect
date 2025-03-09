using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows; // For MessageBox
using UltimateSelect.Models;
using UltimateSelect.Plugins;

namespace UltimateSelect.Plugins
{
    // Export the plugin so MEF can discover it.
    [Export(typeof(IPluginActionProvider))]
    public class WindowPlugin : IPluginActionProvider
    {
        public bool IsApplicable(ContextMenuData context)
        {
            // For demo purposes, this plugin is always applicable.
            return true;
        }

        public async Task<PluginActionOutput> GetPluginActionAsync(ContextMenuData context)
        {
            var output = new PluginActionOutput();

            // Example: Adding two actions with the same key ("Activate: Калькулятор")
            string actionKey = "Activate: Калькулятор";
            output.MenuActions.Add(new KeyValuePair<string, Action>(actionKey, OpenCalculator));
            output.MenuActions.Add(new KeyValuePair<string, Action>(actionKey, OpenCalculatorAdvanced));

            // Simulate async operation
            await Task.CompletedTask;
            return output;
        }

        private void OpenCalculator()
        {
            MessageBox.Show("Calculator opened (simple).", "Calculator", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OpenCalculatorAdvanced()
        {
            MessageBox.Show("Calculator opened (advanced).", "Calculator", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
