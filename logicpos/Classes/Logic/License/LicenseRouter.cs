using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.App;
using logicpos.shared.App;
using LogicPOS.Settings;
using System;
using System.Collections;
using System.Data;
using System.IO;

namespace logicpos.Classes.Logic.License
{
    public class LicenseRouter
    {
        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static bool showDebug = true;
        private readonly string hardwareID = string.Empty;

        public bool LoadApp { get; set; } = false;

        public LicenseRouter()
        {
            //Local Vars
            string dialogMessage = string.Empty;

            if (showDebug)
            {
                _logger.Debug("Debug Mode");
            }

            LoadApp = true;

#if (DEBUG)
            LicenseSettings.LicenseDate = DateTime.Now.ToString("dd/MM/yyyy");
            LicenseSettings.LicenseVersion = "POS_CORPORATE";
            LicenseSettings.LicenseName = "DEBUG";
            LicenseSettings.LicenseHardwareId = "####-####-####-####-####-####";
            LicenseSettings.LicenseCompany = "Empresa Demonstração";
            LicenseSettings.LicenseNif = "NIF Demonstração";
            LicenseSettings.LicenseAddress = "Morada Demonstração";
            LicenseSettings.LicenseEmail = "mail@demonstracao.tld";
            LicenseSettings.LicenseTelephone = "DEBUG";
            LicenseSettings.LicenseModuleStocks = true;
#else
            if (showDebug)
            {
                _logger.Debug("Not Debug Mode");
            }
#endif

#if (!DEBUG)
            if (showDebug)
            {
                _logger.Debug("Before GetLicenceInfo");
            }

            GetLicenceInfo();

            try
            {
                string version = "logicpos";
                // If Plugin is Not Registered in Container                
                if (GlobalFramework.PluginLicenceManager == null)
                {
                    if (showDebug)
                    {
                        _logger.Debug("Skip License Manager, Plugin is Not Registered!");
                    }
                }
                // If Plugin Registered in Container                
                else
                {
                    byte[] registredLicence = new byte[0];

                    hardwareID = GlobalFramework.PluginLicenceManager.GetHardwareID();
                    SharedFramework.LicenseHardwareId = hardwareID;
                    _logger.Debug("Detected hardwareID: " + SharedFramework.LicenseHardwareId);

                    //Try Update Licence    
                    try
                    {
                        bool haveLicence = false;
                        if (GlobalFramework.PluginLicenceManager.GetLicenseInformation().Count > 0)
                        {
                            version = GlobalFramework.PluginLicenceManager.GetLicenseInformation()["version"].ToString();
                            haveLicence = true;
                        }

                        string completeFilePath = string.Format("{0}{1}", LicenseRouter.GetCurrentDirectory(), GlobalFramework.PluginLicenceManager.GetLicenseFilename());
                        completeFilePath = completeFilePath.Replace("\\", "/");

                        //Compare WS License with Local License (GlobalFramework.LicenceVersion)
                        registredLicence = GlobalFramework.PluginLicenceManager.GetLicence(hardwareID, version, haveLicence, ReadFileToByteArray(completeFilePath), GlobalFramework.DtLicenceKeys);

                        //If Diferent Licenses return 1 byte and update local license file, else if equal return byte 0, skipping if
                        if (showDebug)
                        {
                            _logger.Debug("registredLicence.Length: " + registredLicence.Length);
                        }

                        //Update Current Version
                        int result = GlobalFramework.PluginLicenceManager.updateCurrentVersion(hardwareID, version, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
                        if (result > 0)
                        {
                            _logger.Debug("licence updated to version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
                        }
                        else { _logger.Error("Error updating licence version"); }

                        if (registredLicence.Length > 0)
                        {
                            //Used to generate diferent license file names per HardwareId : to Enable find "completeFilePath"
                            //string completeFilePath = GetCurrentDirectory() + string.Format("logicpos_{0}.license", textBoxHardwareID.Text);

                            WriteByteArrayToFile(registredLicence, completeFilePath);

                            logicpos.Utils.ShowMessageTouch(null, DialogFlags.Modal, new System.Drawing.Size(600, 300), MessageType.Info, ButtonsType.Close, CultureResources.GetCustomResources(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_information"), CultureResources.GetCustomResources(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "dialog_message_license_updated"));

                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex.Message);
                    }
                    //Detected Blocked Version : Code must be here to works with Online and Offline Mode
                    if (version == "LOGICPOS_BLOCK")
                    {
                        logicpos.Utils.ShowMessageTouch(null, DialogFlags.Modal, new System.Drawing.Size(600, 300), MessageType.Error, ButtonsType.Close, CultureResources.GetCustomResources(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_error"), CultureResources.GetCustomResources(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "dialog_message_license_blocked"));

                        return;
                    }
                }

                if (showDebug)
                {
                    _logger.Debug("Check if need register");
                }

                if (version == "logicpos")//NeedToRegister())
                {
                    if (showDebug)
                    {
                        _logger.Debug("Need Register");
                    }

                    //Show Form Register
                    if (showDebug)
                    {
                        _logger.Debug("ShowDialog");
                    }

                    LicenseUIResult licenseUIResult = PosLicenceDialog.GetLicenseDetails(hardwareID);
                }
                else
                {
                    _loadApp = true;
                    if (version == "LOGICPOS_LICENSED" || version == "LOGICPOS_PROFESSIONAL" || version == "LOGICPOS_ENTERPRISE" || version == "LOGICPOS_CORPORATE")
                    {
                        GlobalFramework.LicenceRegistered = true;
                    }
                    _logger.Debug("LicenceRegistered: " + GlobalFramework.LicenceRegistered);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Cannot connect with the intellilock WebService: " + ex.Message, ex);
            }
#endif

            if (showDebug)
            {
                _logger.Debug("loadPOS = " + LoadApp);
                //LicenseUIResult licenseUIResult = PosLicenceDialog.GetLicenseDetails(hardwareID);
            }

            if (LoadApp)
            {
                if (showDebug)
                {
                    _logger.Debug("LicenseRouter() :: StartApp: AppMode.FrontOffice");
                }

                /* IN009005 and IN009034: Show "loading" */
                System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(StartPOSFrontOffice));
                GlobalApp.DialogThreadNotify = new ThreadNotify(new ReadyEvent(logicpos.Utils.ThreadDialogReadyEvent));
                thread.Start();

                _logger.Debug("LicenseRouter() :: Show 'loading'");
                GlobalApp.DialogThreadWork = logicpos.Utils.GetThreadDialog(new Window("POS start up"), logicpos.Utils.checkIfDbExists());
                GlobalApp.DialogThreadWork.Run();
                /* IN009005 and IN009034: end" */

            }

            if (showDebug)
            {
                _logger.Debug("end");
            }
        }

        /// <summary>
        /// Start application in FrontOffice mode.
		/// Please see IN009005 and IN009034 for details.
        /// </summary>
        private static void StartPOSFrontOffice()
        {
            if (!SharedFramework.AppUseBackOfficeMode)
            {
                LogicPOSApp logicPos = new LogicPOSApp();
                logicPos.StartApp(AppMode.FrontOffice);
            }
            else
            {
                LogicPOSApp logicPos = new LogicPOSApp();
                logicPos.StartApp(AppMode.Backoffice);
            }
        }

        public static void GetLicenceInfo()
        {
            if (LicenseSettings.LicenseKeys == null)
            {
                LicenseSettings.LicenseKeys = new DataTable("keysLicence");
                LicenseSettings.LicenseKeys.Columns.Add("name", typeof(string));
                LicenseSettings.LicenseKeys.Columns.Add("value", typeof(string));
            }
            LicenseSettings.LicenseKeys.Rows.Clear();

            LicenseSettings.LicenseDate = DateTime.Now.ToString("dd/MM/yyyy");
            LicenseSettings.LicenseVersion = "LOGICPOS_LICENSED";
            LicenseSettings.LicenseName = "Nome DEMO";
            LicenseSettings.LicenseCompany = "Empresa DEMO";
            LicenseSettings.LicenseNif = "NIF DEMO";
            LicenseSettings.LicenseAddress = "Morada DEMO";
            LicenseSettings.LicenseEmail = "Email DEMO";
            LicenseSettings.LicenseTelephone = "Telefone DEMO";
            LicenseSettings.LicenseReseller = "LogicPulse";
            SharedFramework.ServerVersion = "1.0";
            LicenseSettings.LicenceUpdateDate = DateTime.Now.AddDays(-1);
#if DEBUG
            LicenseSettings.LicenseVersion = "LOGICPOS_CORPORATE";
            LicenseSettings.LicenseName = "DEBUG";
            LicenseSettings.LicenseCompany = "DEBUG";
            LicenseSettings.LicenseAddress = "DEBUG";
            LicenseSettings.LicenseEmail = "DEBUG";
            LicenseSettings.LicenseTelephone = "DEBUG";
            LicenseSettings.LicenseModuleStocks = true;
            LicenseSettings.LicenseReseller = "Logicpulse";
#endif

            SortedList sortedList = PluginSettings.PluginLicenceManager.GetLicenseInformation();

            SharedFramework.ServerVersion = PluginSettings.PluginLicenceManager.GetCurrentVersion();
            //GlobalFramework.ServerVersion = "2.0.0.0";

            if (showDebug)
            {
                _logger.Debug("licence info count:" + sortedList.Count.ToString());
            }

            for (int i = 0; i < sortedList.Count; i++)
            {
                string key = sortedList.GetKey(i).ToString();
                string value = sortedList.GetByIndex(i).ToString();
                _logger.Debug("Licence Key:" + key + "=" + value);
                LicenseSettings.LicenseKeys.Rows.Add(key, value);
                switch (key)
                {
                    case "version":
                        LicenseSettings.LicenseVersion = value;
                        break;
                    case "data":
                        LicenseSettings.LicenseDate = value;
                        break;
                    case "name":
                        LicenseSettings.LicenseName = value;
                        break;
                    case "company":
                        LicenseSettings.LicenseCompany = value;
                        break;
                    case "nif":
                        LicenseSettings.LicenseNif = value;
                        break;
                    case "adress":
                        LicenseSettings.LicenseAddress = value;
                        break;
                    case "email":
                        LicenseSettings.LicenseEmail = value;
                        break;
                    case "telefone":
                        LicenseSettings.LicenseTelephone = value;
                        break;
                    case "reseller":
                        LicenseSettings.LicenseReseller = value;
                        PluginSettings.AppCompanyName = value;
                        break;
                    case "logicpos_Module_Stocks":
                        LicenseSettings.LicenseModuleStocks = Convert.ToBoolean(value);
                        break;
                    case "all_UpdateExpirationDate":
                        LicenseSettings.LicenceUpdateDate = Convert.ToDateTime(value);
                        break;
                    default:
                        break;
                }
            }
        }

        public static bool NeedToRegister()
        {
            if (!PluginSettings.PluginLicenceManager.IsLicensed())
            {
                if (showDebug)
                {
                    _logger.Debug("NeedToRegister = true");
                }

                return true;
            }
            else
            {
                if (showDebug)
                {
                    _logger.Debug("NeedToRegister = false");
                }

                return false;
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Static Shared Methods (this|PosLicenseDialog)

        public static string GetCurrentDirectory()
        {
            string currentDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (!currentDir.EndsWith(@"\"))
            {
                currentDir = currentDir + @"/";
            }

            return currentDir;
        }

        public static bool WriteByteArrayToFile(byte[] buff, string filePath)
        {
            bool response = false;
            try
            {
                if (showDebug)
                {
                    _logger.Debug("WriteByteArrayToFile: " + filePath);
                }

                FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(buff);
                bw.Close();
                response = true;
            }
            catch (Exception ex)
            {
                _logger.Error("Error Writing ByteArrayToFile!", ex);
            }

            if (showDebug)
            {
                _logger.Debug("WriteByteArrayToFile response: " + response);
            }

            return response;
        }
    }
}
