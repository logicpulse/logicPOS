using logicpos.datalayer.DataLayer.Xpo;
using logicpos.plugin.contracts;
using logicpos.plugin.library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace logicpos.shared.App
{
    public class GlobalFramework : logicpos.datalayer.App.GlobalFramework
    {
        //Log4Net
        private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Localization
        public static CultureInfo CurrentCulture;
        public static CultureInfo CurrentCultureNumberFormat;
        //Licence
        public static bool LicenceRegistered = false;
        public static String LicenceVersion;
        public static String LicenceDate;
        public static String LicenceName;
        public static String LicenceCompany;
        public static String LicenceNif;
        public static String LicenceAddress;
        public static String LicenceEmail;
        public static String LicenceTelephone;
        public static String LicenceHardwareId;
        //AT - Only Used in logicerp.Modules.FINANCIAL | LogicposHelper
        public static Hashtable AT;
        //Database
        public static String DatabaseServer;
        public static String DatabaseName;
        public static String DatabaseUser;
        public static String DatabasePassword;
        //WorkSession
        public static POS_WorkSessionPeriod WorkSessionPeriodDay;
        public static POS_WorkSessionPeriod WorkSessionPeriodTerminal;
        //Session
        public static GlobalFrameworkSession SessionApp;
        // Plugins
        public static PluginContainer PluginContainer;
        // Plugins
        public static ISoftwareVendor PluginSoftwareVendor;
        public static ILicenceManager PluginLicenceManager;
        //User/Terminal/Permissions
        public static Dictionary<string, bool> LoggedUserPermissions;
        //PreferenceParameters
        public static Dictionary<string, string> PreferenceParameters;
        //FastReport
        public static Dictionary<string, string> FastReportSystemVars;
        public static Dictionary<string, string> FastReportCustomVars;

        public static string AppRootFolder
        {
            get
            {
                //Log4Net
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

                string result = Environment.CurrentDirectory + "/";
                try
                {
                    if (GlobalFramework.Settings["AppRootFolder"] != null)
                    {
                        result = GlobalFramework.Settings["AppRootFolder"];
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message, ex);
                }
                return (result);
            }
        }

        private static bool _canOpenFiles = true;
        public static bool CanOpenFiles
        {
            get
            {
                return (_canOpenFiles);
            }
            set
            {
                _canOpenFiles = value;
            }
        }
    }
}
