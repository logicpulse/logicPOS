using logicpos.datalayer.DataLayer.Xpo;
using logicpos.plugin.contracts;
using logicpos.plugin.library;
using logicpos.shared.Classes.Utils;
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
        public static String LicenceReseller;
        public static DateTime LicenceUpdateDate;
        //TK016248 - BackOffice - Check New Version 
        public static String ServerVersion;
        //AT - Only Used in logicerp.Modules.FINANCIAL | LogicposHelper
        public static Hashtable AT;
        //Database
        public static String DatabaseServer;
        public static String DatabaseName;
        public static String DatabaseUser;
        public static String DatabasePassword;
        //WorkSession
        public static pos_worksessionperiod WorkSessionPeriodDay;
        public static pos_worksessionperiod WorkSessionPeriodTerminal;
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
        //TK013134: HardCoded Modules
        public static bool AppUseParkingTicketModule = false;
		//TK016235 BackOffice - Mode
        public static bool AppUseBackOfficeMode = false;
        public static Dictionary<string,Guid> PendentPayedParkingTickets = new Dictionary<string, Guid>();
        public static Dictionary<string,Guid> PendentPayedParkingCards = new Dictionary<string, Guid>();
		//TK016249 - Impressoras - Diferenciação entre Tipos
        public static bool UsingThermalPrinter;

        //Get Screen Size to use in shared
        public static System.Drawing.Size screenSize;

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
