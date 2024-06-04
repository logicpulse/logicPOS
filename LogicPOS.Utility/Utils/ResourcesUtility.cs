using LogicPOS.Globalization;
using LogicPOS.Settings;

namespace LogicPOS.Utility
{
    public static class ResourcesUtility
    {
        public static string GetResourceByName(string resourceName)
        {
            return CultureResources.GetResourceByLanguage(
                CultureSettings.CurrentCultureName, 
                resourceName);
        }
    }
}
