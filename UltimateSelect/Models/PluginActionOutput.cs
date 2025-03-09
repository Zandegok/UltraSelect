using System;
using System.Collections.Generic;

namespace UltimateSelect.Models
{
    public class PluginActionOutput
    {
        // Using a list of key/value pairs to allow duplicate keys.
        public List<KeyValuePair<string, Action>> MenuActions { get; set; } = new List<KeyValuePair<string, Action>>();

        // Optionally, you might include additional properties:
        public Type WindowType { get; set; }
        public object InitializationParameters { get; set; }
    }
}
