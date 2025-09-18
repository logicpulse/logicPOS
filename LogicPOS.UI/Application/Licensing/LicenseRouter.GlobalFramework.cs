using System;
using System.Data;

namespace LogicPOS.UI.Application.Licensing
{
    public partial class LicenseRouter
    {
        public static class GlobalFramework
        {
            public static string LicenceDate { get; set; }
            public static string LicenceVersion { get; set; }
            public static string LicenceName { get; set; }
            public static string LicenceHardwareId { get; set; }
            public static string LicenceServerHardwareId { get; set; }
            public static string LicenceCompany { get; set; }
            public static string LicenceNif { get; set; }
            public static string LicenceAddress { get; set; }
            public static string LicenceEmail { get; set; }
            public static string LicenceTelephone { get; set; }
            public static bool LicenceModuleStocks { get; set; }
            public static int LicenceCountry { get; set; }
            public static bool LicenceRegistered { get; set; }
            public static string LicenceReseller { get; set; }
            public static string ServerVersion { get; set; }
            public static DateTime LicenceUpdateDate { get; set; }
            public static DataTable DtLicenceKeys { get; set; }
            public static bool AppUseBackOfficeMode { get; set; }
        }

    }
}
