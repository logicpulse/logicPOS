using Gtk;
using logicpos;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application;
using LogicPOS.UI.Components.Windows;
using LogicPOS.Utility;
using System;
using System.IO;

namespace LogicPOS.UI.Components.Licensing
{
    public partial class LicenseRouter
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


                int result = 0;
                try
                {
                    result = PluginSettings.LicenceManager.updateCurrentVersion(
                        HardwareId,
                        version,
                        System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message);
                }

                if (result <= 0)
                {
                    //throw new Exception("Cannot update current version"); -> tchial0
                    System.Windows.Forms.MessageBox.Show("Cannot update current version");
                }

                if (registredLicence.Length > 0)
                {

                    WriteByteArrayToFile(registredLicence, licenseFilePath);

                    CustomAlerts.Information(BackOfficeWindow.Instance)
                                .WithSize(new System.Drawing.Size(600, 400))
                                .WithTitleResource("global_information")
                                .WithMessage(GeneralUtils.GetResourceByName("dialog_message_license_updated"))
                                .ShowAlert();

                    //return; -> removed by tchial0 to continue the execution
                }

                if (version == "LOGICPOS_BLOCK")
                {
                    CustomAlerts.Error(BackOfficeWindow.Instance)
                                .WithSize(new System.Drawing.Size(600, 400))
                                .WithMessage(GeneralUtils.GetResourceByName("dialog_message_license_blocked"))
                                .ShowAlert();

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
                    LogicPOSAppContext.DialogThreadNotify = new ThreadNotify(new ReadyEvent(Utils.NotifyLoadingIsDone));
                    thread.Start();

                    LogicPOSAppContext.LoadingDialog = Utils.CreateSplashScreen();

                    LogicPOSAppContext.LoadingDialog.Run();
                }
            }
        }

      
    }
}
