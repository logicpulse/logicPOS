using System;

namespace logicpos.App
{
    class LicenceManagement
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static bool IsLicensed { get { return GetIsLicensed(); } }
        public static bool CanPrint { get { return GetCanPrint(); } }

        private static bool GetIsLicensed()
        {
            bool result = false;

            try
            {
                //Override default Licence values with SettingsApp.LicenceRegistered
                if (SettingsApp.LicenceRegistered)
                {
                    result = true;
                }
                else
                {
                    result = GlobalFramework.LicenceRegistered;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        private static bool GetCanPrint()
        {
            bool result = false;

            try
            {
                if (IsLicensed)
                {
                    result = true;
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }
    }
}
