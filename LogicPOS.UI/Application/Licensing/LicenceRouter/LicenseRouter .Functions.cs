using LogicPOS.Plugin.Licensing;
using LogicPOS.UI.Application;
using LogicPOS.UI.Settings;
using System.IO;

namespace LogicPOS.UI.Components.Licensing
{
    public partial class LicenseRouter
    {
        
        private static void StartPOSFrontOffice()
        {
            if (AppSettings.Instance.UseBackOfficeMode == false)
            {
                LogicPOSApp logicPos = new LogicPOSApp();
                logicPos.Start();
            }
            else
            {
                LogicPOSApp logicPos = new LogicPOSApp();
                logicPos.Start();
            }
        }

        public static void GetLicenceInfo()
        {
            if (Program.DebugMode) 
            { 
                AppSettings.License.LicenseData.GetDemoData(); 
            r
            }

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
