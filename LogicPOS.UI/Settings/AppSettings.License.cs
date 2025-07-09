using LogicPOS.Plugin.Abstractions;
using System;
using System.Collections;
using System.Data;

namespace LogicPOS.UI.Settings
{
    public partial class AppSettings
    {
        public static class License
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
            public static SortedList LicenseInformations { get; set; }

            public static void ApplyDemoData()
            {
                LicenseDate = DateTime.Now.ToString("dd/MM/yyyy");
                LicenseVersion = "POS_CORPORATE";
                LicenseName = "DEBUG";
                LicenseHardwareId = "####-####-####-####-####-####";
                LicenseCompany = "Empresa Demonstração";
                LicenseNif = "NIF Demonstração";
                LicenseAddress = "Morada Demonstração";
                LicenseEmail = "mail@demonstracao.tld";
                LicenseTelephone = "DEBUG";
                LicenseModuleStocks = true;

                LicenseKeys = new DataTable("keysLicence");
                LicenseKeys.Columns.Add("name", typeof(string));
                LicenseKeys.Columns.Add("value", typeof(string));

                LicenseKeys.Rows.Clear();

                LicenseReseller = "LogicPulse";
                AppSettings.ServerVersion = "1.0";
                LicenceUpdateDate = DateTime.Now.AddDays(-1);
            }

            public static void ApplyDataFromPlugin(ILicenseManager licenseManager)
            {
                LicenseInformations = licenseManager.GetLicenseInformation();

                AppSettings.ServerVersion = licenseManager.GetCurrentVersion();

                LicenseHardwareId = AppSettings.Plugins.LicenceManager.GetHardwareID();

                for (int i = 0; i < LicenseInformations.Count; i++)
                {
                    string key = LicenseInformations.GetKey(i).ToString();
                    string value = LicenseInformations.GetByIndex(i).ToString();

                    LicenseKeys.Rows.Add(key, value);

                    switch (key)
                    {
                        case "version":
                            LicenseVersion = value;
                            break;
                        case "data":
                            LicenseDate = value;
                            break;
                        case "name":
                            LicenseName = value;
                            break;
                        case "company":
                            LicenseCompany = value;
                            break;
                        case "nif":
                            LicenseNif = value;
                            break;
                        case "adress":
                            LicenseAddress = value;
                            break;
                        case "email":
                            LicenseEmail = value;
                            break;
                        case "telefone":
                            LicenseTelephone = value;
                            break;
                        case "reseller":
                            LicenseReseller = value;
                            AppSettings.Plugins.AppCompanyName = value;
                            break;
                        case "logicpos_Module_Stocks":
                            LicenseModuleStocks = Convert.ToBoolean(value);
                            break;
                        case "all_UpdateExpirationDate":
                            LicenceUpdateDate = Convert.ToDateTime(value);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
