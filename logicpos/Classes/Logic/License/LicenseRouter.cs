using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.App;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.resources.Resources.Localization;
using System;
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

            if (showDebug) _log.Debug("Debug Mode");

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
            if (showDebug) _log.Debug("Not Debug Mode");
#endif

#if (!DEBUG)
            if (showDebug) _log.Debug("Before GetLicenceInfo");

            GetLicenceInfo();

            try
            {
                // If Plugin is Not Registered in Container                
                if (GlobalFramework.PluginLicenceManager == null)
                {
                    if (showDebug) _log.Debug("Skip License Manager, Plugin is Not Registered!");
                }
                // If Plugin Registered in Container                
                else
                {
                    byte[] registredLicence = new byte[0];

                    hardwareID = GlobalFramework.PluginLicenceManager.GetHardwareID();
                    GlobalFramework.LicenceHardwareId = hardwareID;
                    _log.Info("Detected hardwareID: " + GlobalFramework.LicenceHardwareId);
                    string version = "logicpos";

                    //Try Update Licence    
                    try
                    {
                        if (GlobalFramework.PluginLicenceManager.GetLicenseInformation().Count > 0)
                        {
                            version = GlobalFramework.PluginLicenceManager.GetLicenseInformation()["version"].ToString();
                        }

                        //Compare WS License with Local License (GlobalFramework.LicenceVersion)
                        registredLicence = GlobalFramework.PluginLicenceManager.GetLicence(hardwareID, version);

                        //If Diferent Licenses return 1 byte and update local license file, else if equal return byte 0, skipping if
                        if (showDebug) _log.Info("registredLicence.Length: " + registredLicence.Length);

                        if (registredLicence.Length > 0)
                        {
                            string completeFilePath = string.Format("{0}{1}", LicenseRouter.GetCurrentDirectory(), GlobalFramework.PluginLicenceManager.GetLicenseFilename());
                            completeFilePath = completeFilePath.Replace("\\", "/");
                            //Used to generate diferent license file names per HardwareId : to Enable find "completeFilePath"
                            //string completeFilePath = GetCurrentDirectory() + string.Format("logicpos_{0}.license", textBoxHardwareID.Text);

                            WriteByteArrayToFile(registredLicence, completeFilePath);

                            Utils.ShowMessageTouch(null, DialogFlags.Modal, new System.Drawing.Size(600, 300), MessageType.Info, ButtonsType.Close, Resx.global_information, Resx.dialog_message_license_updated);

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
                        Utils.ShowMessageTouch(null, DialogFlags.Modal, new System.Drawing.Size(600, 300), MessageType.Error, ButtonsType.Close, Resx.global_error, Resx.dialog_message_license_blocked);

                        return;
                    }
                }

                if (showDebug) _log.Debug("Check if need register");

                if (NeedToRegister())
                {
                    if (showDebug) _log.Debug("Need Register");

                    //Show Form Register
                    if (showDebug) _log.Debug("ShowDialog");
                    LicenseUIResult licenseUIResult = PosLicenceDialog.GetLicenseDetails(hardwareID);
                }
                else
                {
                    _loadApp = true;
                    GlobalFramework.LicenceRegistered = true;
                    _log.Info("LicenceRegistered: " + GlobalFramework.LicenceRegistered);
                }
            }
            catch (Exception ex)
            {
                _log.Error("Cannot connect with the intellilock WebService: " + ex.Message, ex);
            }
#endif

            if (showDebug) _log.Debug("loadPOS = " + _loadApp);

            if (_loadApp)
            {
                LogicPos logicPos = new LogicPos();
                if (showDebug) _log.Debug("StartApp: AppMode.FrontOffice");
                logicPos.StartApp(AppMode.FrontOffice);
            }

            if (showDebug) _log.Debug("end");
        }
        
        public static void GetLicenceInfo()
        {
            GlobalFramework.LicenceDate = DateTime.Now.ToString("dd/MM/yyyy");
            GlobalFramework.LicenceVersion = "LOGICPOS_EXPRESS";
            GlobalFramework.LicenceName = "Nome DEMO";
            GlobalFramework.LicenceCompany = "Empresa DEMO";
            GlobalFramework.LicenceNif = "NIF DEMO";
            GlobalFramework.LicenceAddress = "Morada DEMO";
            GlobalFramework.LicenceEmail = "Email DEMO";
            GlobalFramework.LicenceTelephone = "Telefone DEMO";
#if DEBUG
            GlobalFramework.LicenceVersion = "LOGICPOS_CORPORATE";
            GlobalFramework.LicenceName = "DEBUG";
            GlobalFramework.LicenceCompany = "DEBUG";
            GlobalFramework.LicenceAddress = "DEBUG";
            GlobalFramework.LicenceEmail = "DEBUG";
            GlobalFramework.LicenceTelephone = "DEBUG";
#endif
            if (showDebug) _log.Debug("licence info count:" + GlobalFramework.PluginLicenceManager.GetLicenseInformation().Count.ToString());

            for (int i = 0; i < GlobalFramework.PluginLicenceManager.GetLicenseInformation().Count; i++)
            {
                string key = GlobalFramework.PluginLicenceManager.GetLicenseInformation().GetKey(i).ToString();
                string value = GlobalFramework.PluginLicenceManager.GetLicenseInformation().GetByIndex(i).ToString();
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
                    default:
                        break;
                }
            }
        }

        public static bool NeedToRegister()
        {
            if (! GlobalFramework.PluginLicenceManager.IsLicensed())
            {
                if (showDebug) _log.Debug("NeedToRegister = true");
                return true;
            }
            else
            {
                if (showDebug) _log.Debug("NeedToRegister = false");
                return false;
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Static Shared Methods (this|PosLicenseDialog)

        public static string GetCurrentDirectory()
        {
            string currentDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (!currentDir.EndsWith(@"\"))
                currentDir = currentDir + @"/";

            return currentDir;
        }

        public static bool WriteByteArrayToFile(byte[] buff, string filePath)
        {
            bool response = false;
            try
            {
                if (showDebug) _log.Debug("WriteByteArrayToFile: " + filePath);
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

            if (showDebug) _log.Debug("WriteByteArrayToFile response: " + response);

            return response;
        }
    }
}
