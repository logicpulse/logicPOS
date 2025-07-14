using LogicPOS.Plugin.Licensing;

namespace LogicPOS.UI.Settings
{
    public partial class AppSettings
    {
        public static class License
        {
            public static LicenseData LicenseData { get; set; } = new LicenseData();
        }
    }
}
