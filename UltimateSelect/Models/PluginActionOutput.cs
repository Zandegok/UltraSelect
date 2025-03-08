// File: Plugins/PluginActionOutput.cs
using System;
using System.Collections.Generic;

namespace UltimateSelect.Models
{
    public class PluginActionOutput
    {
        /// <summary>
        /// A dictionary of menu items. Each key is the menu item label and its value is the action (lambda) to execute.
        /// </summary>
        public Dictionary<string, Action> MenuActions { get; set; } = new Dictionary<string, Action>();

        /// <summary>
        /// The type of window to be instantiated. Must inherit from BaseAppWindow.
        /// </summary>
        public Type WindowType { get; set; }

        /// <summary>
        /// A dictionary of initialization parameters for the window.
        /// </summary>
        public Dictionary<string, object> InitializationParameters { get; set; } = new Dictionary<string, object>();
    }
}
