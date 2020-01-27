using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.App;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using System;
using System.Collections;
using System.IO;

namespace logicpos.Classes.Logic.License
{
    public class LicenseRouter
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static bool showDebug = true;
        string hardwareID = string.Empty;

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
                _log.Debug("Debug Mode");
            }

            _loadApp = true;

#if (DEBUG)
            GlobalFramework.LicenceDate = DateTime.Now.ToString("dd/MM/yyyy");
            GlobalFramework.LicenceVersion = "POS_CORPORATE";
            GlobalFramework.LicenceName = "DEBUG";
            GlobalFramework.LicenceHardwareId = "####-####-####-####-####-####";
            //Company Details
            GlobalFramework.LicenceCompany = "Empresa Demonstração";
            GlobalFramework.LicenceNif = "NIF Demonstração";
            GlobalFramework.LicenceAddress = "Morada Demonstração";
            GlobalFramework.LicenceEmail = "mail@demonstracao.tld";
            GlobalFramework.LicenceTelephone = "DEBUG";
#else
            if (showDebug)
            {
                _log.Debug("Not Debug Mode");
            }
#endif

#if (!DEBUG)
            if (showDebug)
            {
                _log.Debug("Before GetLicenceInfo");
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
                        _log.Debug("Skip License Manager, Plugin is Not Registered!");
                    }
                }
                // If Plugin Registered in Container                
                else
                {
                    byte[] registredLicence = new byte[0];

                    hardwareID = GlobalFramework.PluginLicenceManager.GetHardwareID();
                    GlobalFramework.LicenceHardwareId = hardwareID;
                    _log.Debug("Detected hardwareID: " + GlobalFramework.LicenceHardwareId);

                    //Try Update Licence    
                    try
                    {
                        bool haveLicence = false;
                        if (GlobalFramework.PluginLicenceManager.GetLicenseInformation().Count > 0)
                        {
                            version = GlobalFramework.PluginLicenceManager.GetLicenseInformation()["version"].ToString();
                            haveLicence = true;
                        }

                        //Compare WS License with Local License (GlobalFramework.LicenceVersion)
                        registredLicence = GlobalFramework.PluginLicenceManager.GetLicence(hardwareID, version, haveLicence);

                        //If Diferent Licenses return 1 byte and update local license file, else if equal return byte 0, skipping if
                        if (showDebug)
                        {
                            _log.Debug("registredLicence.Length: " + registredLicence.Length);
                        }

                        if (registredLicence.Length > 0)
                        {
                            string completeFilePath = string.Format("{0}{1}", LicenseRouter.GetCurrentDirectory(), GlobalFramework.PluginLicenceManager.GetLicenseFilename());
                            completeFilePath = completeFilePath.Replace("\\", "/");
                            //Used to generate diferent license file names per HardwareId : to Enable find "completeFilePath"
                            //string completeFilePath = GetCurrentDirectory() + string.Format("logicpos_{0}.license", textBoxHardwareID.Text);

                            WriteByteArrayToFile(registredLicence, completeFilePath);

                            Utils.ShowMessageTouch(null, DialogFlags.Modal, new System.Drawing.Size(600, 300), MessageType.Info, ButtonsType.Close, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_information"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_license_updated"));

                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex.Message);
                    }
                    //Detected Blocked Version : Code must be here to works with Online and Offline Mode
                    if (version == "LOGICPOS_BLOCK")
                    {
                        Utils.ShowMessageTouch(null, DialogFlags.Modal, new System.Drawing.Size(600, 300), MessageType.Error, ButtonsType.Close, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_license_blocked"));

                        return;
                    }
                }

                if (showDebug)
                {
                    _log.Debug("Check if need register");
                }

                if (version == "logicpos")//NeedToRegister())
                {
                    if (showDebug)
                    {
                        _log.Debug("Need Register");
                    }

                    //Show Form Register
                    if (showDebug)
                    {
                        _log.Debug("ShowDialog");
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
                    _log.Debug("LicenceRegistered: " + GlobalFramework.LicenceRegistered);
                }
            }
            catch (Exception ex)
            {
                _log.Error("Cannot connect with the intellilock WebService: " + ex.Message, ex);
            }
#endif

            if (showDebug)
            {
                _log.Debug("loadPOS = " + _loadApp);
            }

            if (_loadApp)
            {
                if (showDebug)
                {
                    _log.Debug("LicenseRouter() :: StartApp: AppMode.FrontOffice");
                }

                /* IN009005 and IN009034: Show "loading" */
                System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(StartPOSFrontOffice));
                GlobalApp.DialogThreadNotify = new ThreadNotify(new ReadyEvent(Utils.ThreadDialogReadyEvent));
                thread.Start();

                _log.Debug("LicenseRouter() :: Show 'loading'");
                GlobalApp.DialogThreadWork = Utils.GetThreadDialog(new Window("POS start up"), Utils.checkIfDbExists());
                GlobalApp.DialogThreadWork.Run();
                /* IN009005 and IN009034: end" */

            }

            if (showDebug)
            {
                _log.Debug("end");
            }
        }

        /// <summary>
        /// Start application in FrontOffice mode.
		/// Please see IN009005 and IN009034 for details.
        /// </summary>
        private static void StartPOSFrontOffice()
        {
            if (!GlobalFramework.AppUseBackOfficeMode)
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
            GlobalFramework.LicenceUpdateDate = DateTime.Now.AddDays(-1);
#if DEBUG
            GlobalFramework.LicenceVersion = "LOGICPOS_CORPORATE";
            GlobalFramework.LicenceName = "DEBUG";
            GlobalFramework.LicenceCompany = "DEBUG";
            GlobalFramework.LicenceAddress = "DEBUG";
            GlobalFramework.LicenceEmail = "DEBUG";
            GlobalFramework.LicenceTelephone = "DEBUG";
#endif

            SortedList sortedList = GlobalFramework.PluginLicenceManager.GetLicenseInformation();

            GlobalFramework.ServerVersion = GlobalFramework.PluginLicenceManager.GetCurrentVersion();
            //GlobalFramework.ServerVersion = "2.0.0.0";

            if (showDebug)
            {
                _log.Debug("licence info count:" + sortedList.Count.ToString());
            }

            for (int i = 0; i < sortedList.Count; i++)
            {
                string key = sortedList.GetKey(i).ToString();
                string value = sortedList.GetByIndex(i).ToString();
                _log.Debug("Licence Key:" + key + "=" + value);

                switch (key)
                {
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
            if (!GlobalFramework.PluginLicenceManager.IsLicensed())
            {
                if (showDebug)
                {
                    _log.Debug("NeedToRegister = true");
                }

                return true;
            }
            else
            {
                if (showDebug)
                {
                    _log.Debug("NeedToRegister = false");
                }

                return false;
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Static Shared Methods (this|PosLicenseDialog)

        public static string GetCurrentDirectory()
        {
            string currentDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
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
                    _log.Debug("WriteByteArrayToFile: " + filePath);
                }

                FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(buff);
                bw.Close();
                response = true;
            }
            catch (Exception ex)
            {
                _log.Error("Error Writing ByteArrayToFile!", ex);
            }

            if (showDebug)
            {
                _log.Debug("WriteByteArrayToFile response: " + response);
            }

            return response;
        }
    }
}
