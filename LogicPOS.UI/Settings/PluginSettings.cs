using LogicPOS.Plugin.Abstractions;
using LogicPOS.Plugin.Utils;

namespace LogicPOS.Settings
{
    public static class PluginSettings
    {
        public static string SecretKey = ")p[r#HW'gOg|KNI1L3k]H&~D!DKy`Y[fx2/t&s7{:!S<xDl,l#5)[YHcVf'3UUc";

        public static string AppSoftwareName = "LogicPos";
        public static string AppCompanyName = "LogicPulse Technologies";
        public static string AppCompanyPhone = "+351 233 042 347 / +351 910 287 029 / +351 800 180 500";
        public static string AppCompanyEmail = "comercial@logicpulse.com";
        public static string AppCompanyWeb = "http://www.logicpulse.com";
        public static string AppSoftwareVersionFormat = string.Format("Powered by {0}© Vers. {{0}}", AppCompanyName);

        public static PluginContainer PluginContainer { get; set; }

        public static ILicenseManager LicenceManager { get; set; }

        public static string PluginsFolderLocation => PathsSettings.Paths["plugins"].ToString();

        public static void InitializeContainer()
        {
            PluginContainer = new PluginContainer(PluginsFolderLocation);
        }
    }
}
