using System.Collections.Specialized;

namespace LogicPOS.Settings.Extensions
{
    public static class SettingsExtensions
    {
        public static string GetCultureName(this NameValueCollection settings)
        {
            return settings["customCultureResourceDefinition"];
        }
    }
}
