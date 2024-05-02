using logicpos.plugin.contracts;
using logicpos.plugin.library;

namespace LogicPOS.Settings
{
    public static class PluginSettings
    {
        public static PluginContainer PluginContainer { get; set; }
        public static ISoftwareVendor PluginSoftwareVendor { get; set; }
        public static ILicenceManager PluginLicenceManager { get; set; }
    }
}
