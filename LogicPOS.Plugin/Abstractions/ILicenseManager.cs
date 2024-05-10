using System.Collections;
using System.Data;

namespace LogicPOS.Plugin.Abstractions
{
    public interface ILicenseManager : IPlugin
    {
        string GetHardwareID();

        byte[] GetLicence(
            string hardwareID, 
            string version, 
            bool hasLicense, 
            byte[] licence, 
            DataTable licenseKeys);

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
            string softwareKey);

		string GetCurrentVersion();

        int UpdateCurrentVersion(
            string hardwareID, 
            string productID, 
            string appVersion);

    }
}
