//// File: SamplePlugins/ScreenFragmentActionsPlugin.cs
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System.Windows.Media.Imaging;
//using UltimateSelect.Models;
//using UltimateSelect.Plugins;

//namespace SamplePlugins
//{
//    public class ScreenFragmentActionsPlugin : IPluginActionProvider
//    {
//        public bool IsApplicable(ContextMenuData context)
//        {
//            // Applicable if CustomData contains a captured image.
//            return context.CustomData.ContainsKey("CapturedImage") && 
//                   context.CustomData["CapturedImage"] is BitmapSource;
//        }

//        public async Task<PluginActionOutput> GetPluginActionAsync(ContextMenuData context)
//        {
//            await Task.Yield();

//            var output = new PluginActionOutput();

//            // Define actions for the captured image.
//            output.MenuActions.Add("Copy as Image", () =>
//            {
//                // Logic to copy the image to the clipboard.
//            });
//            output.MenuActions.Add("Save as Image...", () =>
//            {
//                // Logic to save the image to a file.
//            });
//            output.MenuActions.Add("Pin on Screen", () =>
//            {
//                // Logic to pin the image window.
//            });

//            // Specify the window type that handles screen fragment actions.
//            // Assume a window class ScreenFragmentWindow inheriting from BaseAppWindow exists.
//            output.WindowType = typeof(UltimateSelect.Views.ScreenFragmentWindow);

//            // Provide initialization parameters.
//            output.InitializationParameters.Add("WindowTitle", "Screen Fragment Actions");
//            output.InitializationParameters.Add("CapturedImage", context.CustomData["CapturedImage"]);

//            return output;
//        }
//    }
//}
