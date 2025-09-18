using LogicPOS.UI.Components.Licensing;
using Serilog;
using System;
using System.Data;
using System.IO;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace LogicPOS.UI.Application.Licensing
{
    public partial class LicenseRouter
    {
        private string hardwareID = string.Empty;

        public bool LoadApp { get; private set; }

        public LicenseRouter()
        {
            LoadApp = true;

            try
            {
                GetLicenceInfo();

                var version = GlobalFramework.LicenceVersion;

                hardwareID = GetTerminalHardwareID();
                GlobalFramework.LicenceHardwareId = hardwareID;
                Log.Debug("Detected terminal hardwareID: " + GlobalFramework.LicenceHardwareId);

                if (version == "LOGICPOS_BLOCK")
                {
                    MessageBox.Show("A licença foi bloqueada. Contacte o suporte técnico.", "Erro de Licença", MessageBoxButton.OK, MessageBoxImage.Error);
                    LoadApp = false;
                    return;
                }

                var needRegister = GlobalFramework.LicenceVersion == "LOGICPOS_UNLICENSED" || SafeNeedToRegister();
                if (needRegister)
                {
                    Log.Debug("Need Register");
                    Log.Debug("ShowDialog");
                    var licenseUIResult = PosLicenceDialog.GetLicenseDetails(GlobalFramework.LicenceServerHardwareId);
                }
                else
                {
                    LoadApp = true;
                    if (SafeIsLicensed()) GlobalFramework.LicenceRegistered = true;
                    Log.Debug("LicenceRegistered: " + GlobalFramework.LicenceRegistered);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Cannot connect with the licence service: " + ex.Message, ex);
            }

            Log.Debug("loadPOS = " + LoadApp);

            if (LoadApp)
            {
                Log.Debug("LicenseRouter() :: StartApp");
                StartApplication();
            }

            Log.Debug("end");

            bool SafeNeedToRegister()
            {
                try { return LicensingService.NeedToRegister(); }
                catch (Exception ex)
                {
                    Log.Error("NeedToRegister failed: " + ex.Message, ex);
                    return false;
                }
            }

            bool SafeIsLicensed()
            {
                try { return LicensingService.IsLicensed(); }
                catch (Exception ex)
                {
                    Log.Error("IsLicensed failed: " + ex.Message, ex);
                    return false;
                }
            }
        }


        /// <summary>
        /// Start application in FrontOffice mode.
        /// </summary>
        private static void StartApplication()
        {
            // This will be handled by the main application startup
            Log.Debug("Application started successfully");
        }

        public static void GetLicenceInfo()
        {
            if (GlobalFramework.DtLicenceKeys == null)
            {
                GlobalFramework.DtLicenceKeys = new DataTable("keysLicence");
                GlobalFramework.DtLicenceKeys.Columns.Add("name", typeof(string));
                GlobalFramework.DtLicenceKeys.Columns.Add("value", typeof(string));
            }
            GlobalFramework.DtLicenceKeys.Rows.Clear();

            GlobalFramework.LicenceDate = DateTime.Now.ToString("dd/MM/yyyy");
            GlobalFramework.LicenceVersion = "LOGICPOS_LICENSED";
            GlobalFramework.LicenceName = "Nome DEMO";
            GlobalFramework.LicenceCompany = "Empresa DEMO";
            GlobalFramework.LicenceNif = "NIF DEMO";
            GlobalFramework.LicenceAddress = "Morada DEMO";
            GlobalFramework.LicenceEmail = "Email DEMO";
            GlobalFramework.LicenceTelephone = "Telefone DEMO";
            GlobalFramework.LicenceReseller = "LogicPulse";
            GlobalFramework.ServerVersion = "1.0";
            GlobalFramework.LicenceCountry = 168;
            GlobalFramework.LicenceUpdateDate = DateTime.Now.AddDays(-1);
#if DEBUG
            GlobalFramework.LicenceVersion = "LOGICPOS_CORPORATE";
            GlobalFramework.LicenceName = "DEBUG";
            GlobalFramework.LicenceCompany = "DEBUG";
            GlobalFramework.LicenceAddress = "DEBUG";
            GlobalFramework.LicenceEmail = "DEBUG";
            GlobalFramework.LicenceTelephone = "DEBUG";
            GlobalFramework.LicenceModuleStocks = true;
            GlobalFramework.LicenceReseller = "Logicpulse";
            GlobalFramework.LicenceCountry = 168;
#endif

            // Get licence information from API
            string licencePath = GetCurrentDirectory();
            var licenseInfo = LicensingService.GetLicenseInformation();
            GlobalFramework.ServerVersion = LicensingService.GetCurrentVersion();
            Log.Debug("licence info count:" + licenseInfo.Count.ToString());

            foreach (var kvp in licenseInfo)
            {
                string key = kvp.Key;
                string value = kvp.Value;
                Log.Debug("Licence Key:" + key + "=" + value);
                GlobalFramework.DtLicenceKeys.Rows.Add(key, value);
                switch (key)
                {
                    case "hardwareID":
                        GlobalFramework.LicenceServerHardwareId = value;
                        break;
                    case "version":
                        GlobalFramework.LicenceVersion = value;
                        break;
                    case "data":
                        GlobalFramework.LicenceDate = value;
                        break;
                    case "name":
                        GlobalFramework.LicenceName = value;
                        break;
                    case "company":
                        GlobalFramework.LicenceCompany = value;
                        break;
                    case "nif":
                        GlobalFramework.LicenceNif = value;
                        break;
                    case "adress":
                        GlobalFramework.LicenceAddress = value;
                        break;
                    case "email":
                        GlobalFramework.LicenceEmail = value;
                        break;
                    case "telefone":
                        GlobalFramework.LicenceTelephone = value;
                        break;
                    case "reseller":
                        GlobalFramework.LicenceReseller = value;
                        break;
                    case "logicpos_Module_Stocks":
                        GlobalFramework.LicenceModuleStocks = Convert.ToBoolean(value);
                        break;
                    case "all_UpdateExpirationDate":
                        GlobalFramework.LicenceUpdateDate = Convert.ToDateTime(value);
                        break;
                    default:
                        break;
                }
            }
        }

        public static bool NeedToRegister()
        {
            return LicensingService.NeedToRegister();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Static Shared Methods

        public static string GetCurrentDirectory()
        {
            string currentDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (!currentDir.EndsWith(@"\"))
            {
                currentDir = currentDir + @"\";
            }
            return currentDir;
        }

        public string GetTerminalHardwareID()
        {
            string result = string.Empty;

            //int p = (int)Environment.OSVersion.Platform;

            try
            {
                result = GetHashString(GetMacAddress());
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            return result;
        }

        public static string GetMacAddress()
        {
            string macAddresses = string.Empty;

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    macAddresses += nic.GetPhysicalAddress().ToString();
                    break;
                }
            }

            return macAddresses;
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHashSHA256(inputString))
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }

        public static byte[] GetHashSHA256(string inputString)
        {
            HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

    }
}
