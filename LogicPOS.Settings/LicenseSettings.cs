using System;
using System.Data;

namespace LogicPOS.Settings
{
    public static class LicenseSettings
    {
        public static bool LicenceRegistered { get; set; } = false;
        public static string LicenseVersion { get; set; }
        public static string LicenseDate { get; set; }
        public static string LicenseName { get; set; }
        public static string LicenseCompany { get; set; }
        public static string LicenseNif { get; set; }
        public static string LicenseAddress { get; set; }
        public static string LicenseEmail { get; set; }
        public static string LicenseTelephone { get; set; }
        public static string LicenseHardwareId { get; set; }
        public static string LicenseReseller { get; set; }
        public static bool LicenseModuleStocks { get; set; }
        public static DateTime LicenceUpdateDate { get; set; }
        public static DataTable LicenseKeys { get; set; }
    }
}
