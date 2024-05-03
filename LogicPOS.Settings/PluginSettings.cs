using logicpos.plugin.contracts;
using logicpos.plugin.library;
using System.Reflection;
using System;

namespace LogicPOS.Settings
{
    public static class PluginSettings
    {
        public static PluginContainer PluginContainer { get; set; }
        public static ISoftwareVendor PluginSoftwareVendor { get; set; }
        public static ILicenceManager PluginLicenceManager { get; set; }

        public static string GetSoftwareVendorValueAsString(string property)
        {
            return Convert.ToString(GetPluginSoftwareVendorValue(property));
        }

        public static int GetSoftwareVendorValueAsInt(string property)
        {
            return Convert.ToInt16(GetPluginSoftwareVendorValue(property));
        }

        public static bool GetSoftwareVendorValueAsBool(string property)
        {
            return Convert.ToBoolean(GetPluginSoftwareVendorValue(property));
        }

        public static object GetPluginSoftwareVendorValue(string property)
        {
            object resultObject = null;

            if (PluginSoftwareVendor != null)
            {
                Type thisType = PluginSoftwareVendor.GetType();
                string methodName = string.Format("Get{0}", property);
                MethodInfo methodInfo = thisType.GetMethod(methodName);
                object[] methodParameters = null;
                resultObject = methodInfo.Invoke(PluginSoftwareVendor, methodParameters);
            }
            if (resultObject != null)
            {
                return resultObject;
            }
            else
            {
                return null;
            }
        }
    }
}
