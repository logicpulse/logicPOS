namespace logicpos.Utils
{
/* 
    public static class CustomFunctions
    {

        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Register()
        {
            RegisterFunctions();
            RegisterSystemVars();
            RegisterCustomVars();
        }

        private static void RegisterFunctions()
        {
            //Add Funct
            RegisteredObjects.AddFunctionCategory("Custom", "Custom Functions");
            //Obtain MethodInfo for our functions
            Type myType = typeof(CustomFunctions);
            MethodInfo funcRes = myType.GetMethod("Res", new Type[] { typeof(string) });
            MethodInfo funcPref = myType.GetMethod("Pref", new Type[] { typeof(string) });
            MethodInfo funcVar = myType.GetMethod("Var", new Type[] { typeof(string) });
            MethodInfo funcLog = myType.GetMethod("Log", new Type[] { typeof(string) });
            MethodInfo funcDebug = myType.GetMethod("Debug", new Type[] { typeof(string) });
            //Register simple function
            RegisteredObjects.AddFunction(funcRes, "Custom");
            RegisteredObjects.AddFunction(funcPref, "Custom");
            RegisteredObjects.AddFunction(funcVar, "Custom");
            RegisteredObjects.AddFunction(funcLog, "Custom");
            RegisteredObjects.AddFunction(funcDebug, "Custom");
        }

        /// <summary>
        /// Register Custom Variables to Use With Func Var
        /// </summary>
        private static void RegisterSystemVars()
        {
            Dictionary<string, string> systemVars = new Dictionary<string, string>();
            //App
            systemVars["App Name"] = GlobalFramework.Settings["appName"];
            systemVars["App Verison"] = FrameworkUtils.ProductVersion;
            //Check if not null, Reports dont use this STATIC, this way we dont get errors
            if (SettingsApp.ConfigurationSystemCurrency != null)
            {
                systemVars["System Currency Label"] = SettingsApp.ConfigurationSystemCurrency.Designation;
                systemVars["System Currency Acronym"] = SettingsApp.ConfigurationSystemCurrency.Acronym;
                systemVars["System Currency Symbol"] = SettingsApp.ConfigurationSystemCurrency.Symbol;
            }
            //Licence
            systemVars["Licence Name"] = GlobalFramework.LicenceName;
            systemVars["Licence Company"] = GlobalFramework.LicenceCompany;
            systemVars["Licence Nif"] = GlobalFramework.LicenceNif;
            systemVars["Licence Address"] = GlobalFramework.LicenceAddress;
            systemVars["Licence Email"] = GlobalFramework.LicenceEmail;
            systemVars["Licence Telephone"] = GlobalFramework.LicenceTelephone;
            //PreferencesParameters
            systemVars["Company Name"] = Pref("COMPANY_NAME");
            systemVars["Company Business Name"] = Pref("COMPANY_BUSINESS_NAME");
            systemVars["Company Address"] = Pref("COMPANY_ADDRESS");
            systemVars["Company City"] = Pref("COMPANY_CITY");
            systemVars["Company Postalcode"] = Pref("COMPANY_POSTALCODE");
            systemVars["Company Region"] = Pref("COMPANY_REGION");
            systemVars["Company Country"] = Pref("COMPANY_COUNTRY");
            systemVars["Company Country Code2"] = Pref("COMPANY_COUNTRY_CODE2");
            systemVars["Company Telephone"] = Pref("COMPANY_TELEPHONE");
            systemVars["Company Mobilephone"] = Pref("COMPANY_MOBILEPHONE");
            systemVars["Company Fax"] = Pref("COMPANY_FAX");
            systemVars["Company Email"] = Pref("COMPANY_EMAIL");
            systemVars["Company Website"] = Pref("COMPANY_WEBSITE");
            systemVars["Company Fiscalnumber"] = Pref("COMPANY_FISCALNUMBER");
            systemVars["Company CAE"] = Pref("COMPANY_CAE");
            systemVars["Company Stock Capital"] = Pref("COMPANY_STOCK_CAPITAL");
            systemVars["Company Company ID"] = Pref("COMPANY_COMPANY_ID");
            systemVars["Company Tax Entity"] = Pref("COMPANY_TAX_ENTITY");
            systemVars["Report Filename Logo"] = Pref("REPORT_FILENAME_LOGO");
            systemVars["Report Footer Line1"] = Pref("REPORT_FOOTER_LINE1");
            systemVars["Report Footer Line2"] = Pref("REPORT_FOOTER_LINE2");
            //Session
            systemVars["Session Logged User"] = String.Empty;//Not Yet Assigned (BootStrap), This is Assigned on Report Constructor
            //systemVars["Session Logged Terminal"] = GlobalFramework.LoggedTerminal.Designation;

            GlobalFramework.FastReportSystemVars = systemVars;
        }

        /// <summary>
        /// Register Custom Variables to Use With Func Var
        /// </summary>
        private static void RegisterCustomVars()
        {
            Dictionary<string, string> customVars = new Dictionary<string, string>();

            //Custom Vars
            customVars["CUSTOM_VAR_1"] = "Custom Var1";
            customVars["CUSTOM_VAR_2"] = "Custom Var2";
            customVars["CUSTOM_VAR_3"] = "Custom Var3";

            GlobalFramework.FastReportCustomVars = customVars;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Custom Functions

        /// <summary>
        /// Get Resource String from Resources
        /// </summary>
        /// <param name="ResourceName"></param>
        /// <returns>Resource String</returns>
        public static string Res(string pResourceName)
        {
            try
            {
                string result = (logicpos.reports.Resources.Localization.Resx.ResourceManager.GetString(pResourceName) != null)
                  ? logicpos.reports.Resources.Localization.Resx.ResourceManager.GetString(pResourceName)
                  : string.Format("UNDEFINED [{0}]", pResourceName);

                return result;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return "ERROR";
            }
        }

        /// <summary>
        /// Get Preference Parameter String from Token
        /// </summary>
        /// <param name="Token"></param>
        /// <returns>Preference Parameter String</returns>
        public static string Pref(string pToken)
        {
            try
            {
                string result = (GlobalFramework.PreferenceParameters.ContainsKey(pToken.ToUpper()))
                  ? GlobalFramework.PreferenceParameters[pToken.ToUpper()]
                  : string.Format("UNDEFINED [{0}]", pToken);

                return result;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return "ERROR";
            }
        }

        /// <summary>
        /// Get Custom Var
        /// </summary>
        /// <param name="Token"></param>
        /// <returns>Custom Var String</returns>
        public static string Var(string pToken)
        {
            try
            {
                string result = (GlobalFramework.FastReportCustomVars.ContainsKey(pToken.ToUpper()))
                  ? GlobalFramework.FastReportCustomVars[pToken.ToUpper()]
                  : string.Format("UNDEFINED [{0}]", pToken);
                return result;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return "ERROR";
            }
        }

        /// <summary>
        /// Send to Log
        /// </summary>
        /// <param name="Output"></param>
        public static string Log(string pOutput)
        {
            try
            {
                _log.Debug(string.Format("FastReport: [{0}]", pOutput));
                return "LOG";
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return "ERROR";
            }
        }

        /// <summary>
        /// SendObject to Debug
        /// </summary>
        /// <param name="Object"></param>
        public static string Debug(object pObject)
        {
            try
            {
                _log.Debug(string.Format("FastReport: Debug Object Type [{0}]", pObject.GetType()));
                return "DEBUG";
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return "ERROR";
            }
        }
    }
*/ 
}
