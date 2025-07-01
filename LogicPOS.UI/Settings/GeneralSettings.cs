using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace LogicPOS.Settings
{
    public static class GeneralSettings
    {
        public static string AppTheme = "Default";
        public static string ServerVersion { get; set; }
        public static string ProductVersion
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                return $"v{fileVersionInfo.ProductVersion}";
            }
        }
        public static System.Drawing.Size ScreenSize { get; set; }
        public static bool AppUseParkingTicketModule { get; set; } = false;
        public static bool AppUseBackOfficeMode { get; set; } = false;
    }
}
