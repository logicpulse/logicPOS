using System.Collections;
using System.Data;

namespace logicpos.plugin.contracts
{
    public interface ILicenceManager : IPlugin
    {
        string GetHardwareID();
        byte[] GetLicence(string hardwareID, string version, bool haveLicence, byte[] licence, DataTable keysLicence);
        SortedList GetLicenseInformation();
        string GetLicenseFilename();
        bool IsLicensed();
        bool ConnectToWS();
        byte[] ActivateLicense(string name, string company, string fiscalNumber, string address, string email, string phone, string hardwareId, string assemblyVersion, string softwareKey);
        //TK016248 - BackOffice - Check New Version 
		string GetCurrentVersion();
        int updateCurrentVersion(string hardwareID, string productID, string versionApp);
        //System.Drawing.Image DecodeImage(string filePath, int width, int height);
    }
}
