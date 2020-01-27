using System.Collections;

namespace logicpos.plugin.contracts
{
    public interface ILicenceManager : IPlugin
    {
        string GetHardwareID();
        byte[] GetLicence(string hardwareID, string version, bool haveLicence);
        SortedList GetLicenseInformation();
        string GetLicenseFilename();
        bool IsLicensed();
        bool ConnectToWS();
        byte[] ActivateLicense(string name, string company, string fiscalNumber, string address, string email, string phone, string hardwareId, string assemblyVersion);
        //TK016248 - BackOffice - Check New Version 
		string GetCurrentVersion();

    }
}
