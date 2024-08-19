using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.App;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using LogicPOS.Persistence.Services;
using LogicPOS.Settings;
using LogicPOS.UI;
using LogicPOS.Utility;
using System;
using System.IO;

namespace logicpos.Classes.Logic.License
{
    public class LicenseRouter
    {
        public static string HardwareId = string.Empty;
        public bool LoadApp { get; set; } = true;

        public LicenseRouter()
        {
            string dialogMessage = string.Empty;

            if (Program.DebugMode)
            {
                LicenseSettings.ApplyDemoData();
            }
            else
            {
                GetLicenceInfo();

                string version = "logicpos";

                byte[] registredLicence = new byte[0];

                HardwareId = LicenseSettings.LicenseHardwareId;
                bool hasLicense = false;

                if (LicenseSettings.LicenseInformations.Count > 0)
                {
                    version = LicenseSettings.LicenseVersion;
                    hasLicense = true;
                }

                string licenseFilePath = PluginSettings.LicenceManager.GetLicenseFilename();
                var licenseFileBytes = File.ReadAllBytes(licenseFilePath);

                registredLicence = PluginSettings.LicenceManager.GetLicence(
                    HardwareId,
                    version,
                    hasLicense,
                    licenseFileBytes,
                    LicenseSettings.LicenseKeys);


                int result = PluginSettings.LicenceManager.updateCurrentVersion(
                    HardwareId,
                    version,
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

                if (result <= 0)
                {
                    throw new Exception("Cannot update current version");
                }

                if (registredLicence.Length > 0)
                {

                    WriteByteArrayToFile(registredLicence, licenseFilePath);

                    logicpos.Utils.ShowMessageTouch(
                        null,
                        DialogFlags.Modal,
                        MessageType.Info,
                        ButtonsType.Close,
                        GeneralUtils.GetResourceByName("global_information"),
                        GeneralUtils.GetResourceByName("dialog_message_license_updated"));

                    //return; -> removed by tchial0 to continue the execution
                }

                if (version == "LOGICPOS_BLOCK")
                {
                    logicpos.Utils.ShowMessageTouch(
                        null,
                        DialogFlags.Modal,
                        MessageType.Error,
                        ButtonsType.Close,
                        GeneralUtils.GetResourceByName("global_error"),
                        GeneralUtils.GetResourceByName("dialog_message_license_blocked"));

                    return;
                }

                if (version == "logicpos")
                {

                    LicenseUIResult licenseUIResult = PosLicenceDialog.GetLicenseDetails(HardwareId);
                }
                else
                {
                    LoadApp = true;

                    switch (version)
                    {
                        case "LOGICPOS_LICENSED":
                        case "LOGICPOS_PROFESSIONAL":
                        case "LOGICPOS_ENTERPRISE":
                        case "LOGICPOS_CORPORATE":
                            LicenseSettings.LicenceRegistered = true;
                            break;
                    }

                }




                if (LoadApp)
                {
                    System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(StartPOSFrontOffice));
                    GlobalApp.DialogThreadNotify = new ThreadNotify(new ReadyEvent(logicpos.Utils.NotifyLoadingIsDone));
                    thread.Start();

                    GlobalApp.LoadingDialog = logicpos.Utils.CreateSplashScreen(
                        new Window("POS start up"),
                        DatabaseService.DatabaseExists());

                    GlobalApp.LoadingDialog.Run();
                }
            }
        }

        private static void StartPOSFrontOffice()
        {
            if (GeneralSettings.AppUseBackOfficeMode == false)
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
            if (Program.DebugMode)
            {
                LicenseSettings.ApplyDemoData();
                return;
            }

            LicenseSettings.ApplyDataFromPlugin(PluginSettings.LicenceManager);
        }

        public static void WriteByteArrayToFile(byte[] buff, string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(buff);
            bw.Close();
        }
    }
}
