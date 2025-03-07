namespace UltimateSelect.Plugins
{
    public class MenuItemData
    {
        public string Header { get; set; }
        public string IconUri { get; set; }  // URI for the icon, if any.
        public string CommandName { get; set; }
        public object CommandParameter { get; set; }
    }
}
