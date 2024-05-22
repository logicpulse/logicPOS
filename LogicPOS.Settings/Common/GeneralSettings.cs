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
        public static NameValueCollection Settings { get; set; }
        public static Dictionary<string, string> PreferenceParameters { get; set; }
        public static string ServerVersion { get; set; }
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
        public static System.Drawing.Size ScreenSize { get; set; }
        public static int GetRequiredCustomerDetailsAboveValue(Guid countryId)
        {
            if (CultureSettings.CountryIdIsPortugal(countryId))
            {
                return PluginSettings.HasSoftwareVendorPlugin
                    ? PluginSettings.SoftwareVendor.GetFinanceRuleRequiredCustomerDetailsAboveValue()
                    : 1000;

            }

            return 999999999;
        }
        public static Dictionary<string, bool> LoggedUserPermissions { get; set; }
        public static bool AppUseParkingTicketModule { get; set; } = false;
        public static bool CheckStocks { get; set; } = true;
        public static bool CheckStockMessage { get; set; } = true;
        public static bool AppUseBackOfficeMode { get; set; } = false;
        public static Dictionary<string, Guid> PendentPaidParkingTickets { get; set; } = new Dictionary<string, Guid>();
        public static Dictionary<string, Guid> PendentPaidParkingCards { get; set; } = new Dictionary<string, Guid>();
        public static bool UsePosPDFViewer => Convert.ToBoolean(PreferenceParameters["USE_POS_PDF_VIEWER"]);
        public static bool HasPermissionTo(string pToken)
        {
            if (LoggedUserPermissions != null && LoggedUserPermissions.ContainsKey(pToken))
            {
                return LoggedUserPermissions[pToken];
            }

            return false;
        }

    }
}
