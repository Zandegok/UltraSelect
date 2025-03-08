// File: Plugins/IPluginActionProvider.cs
using System.Threading.Tasks;
using UltimateSelect.Models;

namespace UltimateSelect.Plugins
{
    public interface IPluginActionProvider
    {
        /// <summary>
        /// Determines whether this plugin is applicable based on the current context.
        /// </summary>
        bool IsApplicable(ContextMenuData context);

        /// <summary>
        /// Asynchronously returns a PluginActionOutput containing the menu actions,
        /// a window type, and any initialization parameters.
        /// </summary>
        Task<PluginActionOutput> GetPluginActionAsync(ContextMenuData context);
    }
}
