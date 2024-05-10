using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reflection;

namespace LogicPOS.Settings
{
    public static class GeneralSettings
    {
        public static string AppTheme = "Default";
        public static Hashtable Paths { get; set; }
        public static NameValueCollection Settings { get; set; }
        public static Dictionary<string, string> PreferenceParameters { get; set; }
        public static string ProductVersion
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                return string.Format("v{0}", fileVersionInfo.ProductVersion);
            }
        }
        public static Assembly ProductAssembly
        {
            get
            {
                return Assembly.GetExecutingAssembly();
            }
        }
        public static string POSSessionJsonFileName => $"appsession_{LicenseSettings.LicenseHardwareId}.json";
        
        public static int GetRequiredCustomerDetailsAboveValue(Guid countryId)
        {
            if (CultureSettings.CountryIdIsPortugal(countryId))
            {
                return PluginSettings.HasPlugin
                    ? PluginSettings.SoftwareVendor.GetRequiredCustomerDetailsAboveValue()
                    : 1000;

            }

            return 999999999;
        }

    }
}
