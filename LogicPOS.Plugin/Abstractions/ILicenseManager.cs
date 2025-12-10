using System.Collections;
using System.Data;
using System.Drawing;

namespace LogicPOS.Plugin.Abstractions
{
    public interface ILicenseManager : IPlugin
    {
        string GetHardwareID();

        byte[] GetLicence(
            string hardwareID,
            string version,
            bool haveLicence,
            byte[] licence,
            DataTable keysLicence);

        SortedList GetLicenseInformation();

        string GetLicenseFilename();

        bool IsLicensed();

        bool ConnectToWS();

        byte[] ActivateLicense(
            string name,
            string company,
            string fiscalNumber,
            string address,
            string email,
            string phone,
            string hardwareId,
            string assemblyVersion,
            int idCountry,
            string softwareKey);

        string GetCurrentVersion();

        DataTable GetCountries();

        int updateCurrentVersion(
            string hardwareID,
            string productID,
            string versionApp);

        Image DecodeImage(
            string filePath,
            int width,
            int height);

        string GenHardwareID(GenHardwareIDQuery genHardwareID);
    }

    public class GenHardwareIDQuery
    {
        public string CpuId { get; set; }
        public string MotherboardSerial { get; set; }
        public string DiskSerial { get; set; }
        public string MachineGuid { get; set; }
        public string SystemUuid { get; set; }
    }
}
