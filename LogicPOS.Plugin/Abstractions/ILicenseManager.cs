using LogicPOS.Plugin.Licensing;
using System.Collections;
using System.Data;
using System.Drawing;

namespace LogicPOS.Plugin.Abstractions
{
    public interface ILicenseManager : IPlugin
    {
        string GetHardwareID();

        byte[] GetLicense(string hardwareID,
                          string version,
                          bool hasLicense,
                          byte[] license,
                          DataTable licenseKeys);

        SortedList GetLicenseInformation();
        string GetLicenseFilename();
        bool IsLicensed();

        byte[] ActivateLicense(string name,
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

        int UpdateCurrentVersion(string hardwareID,
                                 string productID,
                                 string versionApp);

        Image DecodeImage(string filePath,
                          int width,
                          int height);

        LicenseData GetLicenseData();

    }
}
