using logicpos.shared.App;

namespace logicpos.App
{
    internal class LicenceManagement
    {

        public static bool IsLicensed => POSSettings.LicenceRegistered || SharedFramework.LicenceRegistered;
        public static bool CanPrint => IsLicensed;
    }
}
