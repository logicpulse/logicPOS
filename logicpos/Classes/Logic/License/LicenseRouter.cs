using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.App;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.shared.App;
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

        private bool _loadApp = false;
        public bool LoadApp
        {
            get { return _loadApp; }
            set { _loadApp = value; }
        }

        public LicenseRouter()
        {
            //Local Vars
            string dialogMessage = string.Empty;

            if (showDebug)
            {
                _logger.Debug("Debug Mode");
            }

            _loadApp = true;

#if (DEBUG)
            SharedFramework.LicenseDate = DateTime.Now.ToString("dd/MM/yyyy");
            SharedFramework.LicenseVersion = "POS_CORPORATE";
            SharedFramework.LicenseName = "DEBUG";
            SharedFramework.LicenseHardwareId = "####-####-####-####-####-####";
            //Company Details
            SharedFramework.LicenseCompany = "Empresa Demonstração";
            SharedFramework.LicenseNif = "NIF Demonstração";
            SharedFramework.LicenseAddress = "Morada Demonstração";
            SharedFramework.LicenseEmail = "mail@demonstracao.tld";
            SharedFramework.LicenseTelephone = "DEBUG";
            SharedFramework.LicenseModuleStocks = true;
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

                            logicpos.Utils.ShowMessageTouch(null, DialogFlags.Modal, new System.Drawing.Size(600, 300), MessageType.Info, ButtonsType.Close, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_information"), resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "dialog_message_license_updated"));

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
                        logicpos.Utils.ShowMessageTouch(null, DialogFlags.Modal, new System.Drawing.Size(600, 300), MessageType.Error, ButtonsType.Close, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_error"), resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "dialog_message_license_blocked"));

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
                _logger.Debug("loadPOS = " + _loadApp);
                //LicenseUIResult licenseUIResult = PosLicenceDialog.GetLicenseDetails(hardwareID);
            }

            if (_loadApp)
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
                LogicPos logicPos = new LogicPos();
                logicPos.StartApp(AppMode.FrontOffice);
            }
            else
            {
                LogicPos logicPos = new LogicPos();
                logicPos.StartApp(AppMode.Backoffice);
            }
        }

        public static void GetLicenceInfo()
        {
            if (SharedFramework.DtLicenseKeys == null)
            {
                SharedFramework.DtLicenseKeys = new DataTable("keysLicence");
                SharedFramework.DtLicenseKeys.Columns.Add("name", typeof(string));
                SharedFramework.DtLicenseKeys.Columns.Add("value", typeof(string));
            }
            SharedFramework.DtLicenseKeys.Rows.Clear();

            SharedFramework.LicenseDate = DateTime.Now.ToString("dd/MM/yyyy");
            SharedFramework.LicenseVersion = "LOGICPOS_LICENSED";
            SharedFramework.LicenseName = "Nome DEMO";
            SharedFramework.LicenseCompany = "Empresa DEMO";
            SharedFramework.LicenseNif = "NIF DEMO";
            SharedFramework.LicenseAddress = "Morada DEMO";
            SharedFramework.LicenseEmail = "Email DEMO";
            SharedFramework.LicenseTelephone = "Telefone DEMO";
            SharedFramework.LicenseReseller = "LogicPulse";
            SharedFramework.ServerVersion = "1.0";
            SharedFramework.LicenceUpdateDate = DateTime.Now.AddDays(-1);
#if DEBUG
            SharedFramework.LicenseVersion = "LOGICPOS_CORPORATE";
            SharedFramework.LicenseName = "DEBUG";
            SharedFramework.LicenseCompany = "DEBUG";
            SharedFramework.LicenseAddress = "DEBUG";
            SharedFramework.LicenseEmail = "DEBUG";
            SharedFramework.LicenseTelephone = "DEBUG";
            SharedFramework.LicenseModuleStocks = true;
            SharedFramework.LicenseReseller = "Logicpulse";
#endif

            SortedList sortedList =SharedFramework.PluginLicenceManager.GetLicenseInformation();

            SharedFramework.ServerVersion =SharedFramework.PluginLicenceManager.GetCurrentVersion();
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
                SharedFramework.DtLicenseKeys.Rows.Add(key, value);
                switch (key)
                {
                    case "version":
                        SharedFramework.LicenseVersion = value;
                        break;
                    case "data":
                        SharedFramework.LicenseDate = value;
                        break;
                    case "name":
                        SharedFramework.LicenseName = value;
                        break;
                    case "company":
                        SharedFramework.LicenseCompany = value;
                        break;
                    case "nif":
                        SharedFramework.LicenseNif = value;
                        break;
                    case "adress":
                        SharedFramework.LicenseAddress = value;
                        break;
                    case "email":
                        SharedFramework.LicenseEmail = value;
                        break;
                    case "telefone":
                        SharedFramework.LicenseTelephone = value;
                        break;
                    case "reseller":
                        SharedFramework.LicenseReseller = value;
                        SharedSettings.AppCompanyName = value;
                        break;
                    case "logicpos_Module_Stocks":
                        SharedFramework.LicenseModuleStocks = Convert.ToBoolean(value);
                        break;
                    case "all_UpdateExpirationDate":
                        SharedFramework.LicenceUpdateDate = Convert.ToDateTime(value);
                        break;
                    default:
                        break;
                }
            }
        }

        public static bool NeedToRegister()
        {
            if (!SharedFramework.PluginLicenceManager.IsLicensed())
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

        private static byte[] ReadFileToByteArray(string filePath)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(filePath);
                return bytes;
            }
            catch (Exception ex)
            {
                _logger.Error("Error ReadFileToByteArray!", ex);
            }
            return null;
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
