using logicpos.shared.App;

namespace logicpos.App
{
    internal class LicenceManagement
    {

        public static bool IsLicensed => POSSettings.LicenceRegistered || LogicPOS.Settings.LicenseSettings.LicenceRegistered;
        public static bool CanPrint => IsLicensed;
    }
}
