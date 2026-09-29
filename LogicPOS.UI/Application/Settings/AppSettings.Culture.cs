using System.Globalization;

namespace LogicPOS.UI.Settings
{
    public partial class AppSettings
    {
        public static class Culture
        {
            public static string CurrentCultureName => AppSettings.Instance.CustomCultureResourceDefinition;
            public static string DateFormat { get; set; } = "yyyy-MM-dd";
            public static string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";
        }
    }
}
