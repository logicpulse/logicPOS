using logicpos.shared.App;
using System;

namespace logicpos.App
{
    internal class LicenceManagement
    {
        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static bool IsLicensed { get { return GetIsLicensed(); } }
        public static bool CanPrint { get { return GetCanPrint(); } }

        private static bool GetIsLicensed()
        {
            bool result = false;

            try
            {
                //Override default Licence values with SettingsApp.LicenceRegistered
                if (POSSettings.LicenceRegistered)
                {
                    result = true;
                }
                else
                {
                    result = SharedFramework.LicenceRegistered;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
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
                _logger.Error(ex.Message, ex);
            }

            return result;
        }
    }
}
