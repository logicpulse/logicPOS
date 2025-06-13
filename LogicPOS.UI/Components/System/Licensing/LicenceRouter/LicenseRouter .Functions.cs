using LogicPOS.Settings;
using LogicPOS.UI.Application;
using System.IO;

namespace LogicPOS.UI.Components.Licensing
{
    public partial class LicenseRouter
    {
        
        private static void StartPOSFrontOffice()
        {
            if (GeneralSettings.AppUseBackOfficeMode == false)
            {
                LogicPOSAppUtils logicPos = new LogicPOSAppUtils();
                logicPos.StartApp();
            }
            else
            {
                LogicPOSAppUtils logicPos = new LogicPOSAppUtils();
                logicPos.StartApp();
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
