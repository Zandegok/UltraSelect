using System.Threading.Tasks;
using UltimateSelect.Models;

namespace UltimateSelect.Plugins
{
    public interface IPluginActionProvider
    {
        Task<PluginActionOutput> GetPluginActionAsync(ContextMenuData context);
        bool IsApplicable(ContextMenuData context);
    }
}
