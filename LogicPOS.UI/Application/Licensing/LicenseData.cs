using System;
using System.Data;

namespace LogicPOS.UI.Application.Licensing
{
    public class LicenseData
    {
        public string LicenceDate { get; set; }
        public string LicenceVersion { get; set; }
        public string LicenceName { get; set; }
        public string TerminalHardwareId { get; set; }
        public string ApiHardwareId { get; set; }
        public string LicenceCompany { get; set; }
        public string LicenceNif { get; set; }
        public string LicenceAddress { get; set; }
        public string LicenceEmail { get; set; }
        public string LicenceTelephone { get; set; }
        public bool LicenceModuleStocks { get; set; }
        public int NumberDevices { get; set; }
        public int LicenceCountry { get; set; }
        public bool LicenceRegistered { get; set; }
        public string LicenceReseller { get; set; }
        public string ServerVersion { get; set; }
        public DateTime LicenceUpdateDate { get; set; }
        public DataTable DtLicenceKeys { get; set; }
        public bool AppUseBackOfficeMode { get; set; }
        public bool IsBlocked => LicenceVersion == "LOGICPOS_BLOCK";
        public bool IsUnlicensed => LicenceVersion == "LOGICPOS_UNLICENSED";

        public bool ModuleAgtFe { get; set; }
    }
}
