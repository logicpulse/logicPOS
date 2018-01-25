using FastReport.Utils;
using logicpos.financial.library.App;
using logicpos.financial.library.Classes.Finance;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace logicpos.financial.library.Classes.Reports
{
    public static class CustomFunctions
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static ResourceManager _resourceManager;
        private static bool _lowerCaseResource;

        //Using Base Project Resources
        public static void Register(string pAppName)
        {
            Register(pAppName, Resx.ResourceManager, true);
        }

        //Using Outside Project Resources: Extended for JoanaReports
        public static void Register(string pAppName, ResourceManager pResourceManager, bool pLowerCaseResource)
        {
            //Parameters
            _resourceManager = pResourceManager;
            _lowerCaseResource = pLowerCaseResource;
            //Register Methods
            RegisterFunctions();
            RegisterSystemVars(pAppName);
            RegisterCustomVars(pAppName);
        }

        private static void RegisterFunctions()
        {
            //Add Funct
            RegisteredObjects.AddFunctionCategory("Custom", "Custom Functions");
            //Obtain MethodInfo for our functions
            Type myType = typeof(CustomFunctions);
            MethodInfo funcRes = myType.GetMethod("Res", new Type[] { typeof(string) });
            MethodInfo funcGetParam = myType.GetMethod("GetParam", new Type[] { typeof(string) });
            MethodInfo funcPref = myType.GetMethod("Pref", new Type[] { typeof(string) });
            MethodInfo funcVar = myType.GetMethod("Var", new Type[] { typeof(string) });
            MethodInfo funcLog = myType.GetMethod("Log", new Type[] { typeof(string) });
            MethodInfo funcDebug = myType.GetMethod("Debug", new Type[] { typeof(object) });
            MethodInfo funcExtendedValue = myType.GetMethod("ExtendedValue", new Type[] { typeof(decimal), typeof(string) });

            //Register simple function
            RegisteredObjects.AddFunction(funcRes, "Custom");
            RegisteredObjects.AddFunction(funcGetParam, "Custom");
            RegisteredObjects.AddFunction(funcPref, "Custom");
            RegisteredObjects.AddFunction(funcVar, "Custom");
            RegisteredObjects.AddFunction(funcLog, "Custom");
            RegisteredObjects.AddFunction(funcDebug, "Custom");
            RegisteredObjects.AddFunction(funcExtendedValue, "Custom");
        }

        /// <summary>
        /// Register Custom Variables to Use With Func Var
        /// </summary>
        private static void RegisterSystemVars()
        {
            RegisterSystemVars(String.Empty);
        }        
        
        private static void RegisterSystemVars(string pAppName)
        {
            if (GlobalFramework.FastReportSystemVars == null)
            {
                Dictionary<string, string> systemVars = new Dictionary<string, string>();
                systemVars.Add("PreparedPages", "0");
                GlobalFramework.FastReportSystemVars = systemVars;
            } 
        }

        /// <summary>
        /// Register Custom Variables to Use With Func Var and Assign it to GlobalFramework.FastReportCustomVars
        /// </summary>
        public static void RegisterCustomVars(string pAppName)
        {
            Dictionary<string, string> customVars = new Dictionary<string, string>();

            //App
            customVars.Add("App_Company", SettingsApp.AppCompanyName);
            customVars.Add("App_Name", pAppName);
            customVars.Add("App_Version", FrameworkUtils.ProductVersion);
            customVars.Add("Date", FrameworkUtils.CurrentDateTimeAtomic().ToString(SettingsApp.DateFormat));
            if (SettingsApp.ConfigurationSystemCurrency != null)
            {
                customVars.Add("System_Currency_Label", SettingsApp.ConfigurationSystemCurrency.Designation);
                customVars.Add("System_Currency_Acronym", SettingsApp.ConfigurationSystemCurrency.Acronym);
                customVars.Add("System_Currency_Symbol", SettingsApp.ConfigurationSystemCurrency.Symbol);
            }
            //Licence
            customVars.Add("Licence_Name", GlobalFramework.LicenceName);
            customVars.Add("Licence_Company", GlobalFramework.LicenceCompany);
            customVars.Add("Licence_Nif", GlobalFramework.LicenceNif);
            customVars.Add("Licence_Address", GlobalFramework.LicenceAddress);
            customVars.Add("Licence_Email", GlobalFramework.LicenceEmail);
            customVars.Add("Licence_Telephone", GlobalFramework.LicenceTelephone);
            //PreferencesParameters
            customVars.Add("Company_Name", Pref("COMPANY_NAME"));
            customVars.Add("Company_Business_Name", Pref("COMPANY_BUSINESS_NAME"));
            customVars.Add("Company_Address", Pref("COMPANY_ADDRESS"));
            customVars.Add("Company_City", Pref("COMPANY_CITY"));
            customVars.Add("Company_Postalcode", Pref("COMPANY_POSTALCODE"));
            customVars.Add("Company_Region", Pref("COMPANY_REGION"));
            customVars.Add("Company_Country", Pref("COMPANY_COUNTRY"));
            customVars.Add("Company_Country_Code2", Pref("COMPANY_COUNTRY_CODE2"));
            customVars.Add("Company_Telephone", Pref("COMPANY_TELEPHONE"));
            customVars.Add("Company_Mobilephone", Pref("COMPANY_MOBILEPHONE"));
            customVars.Add("Company_Fax", Pref("COMPANY_FAX"));
            customVars.Add("Company_Email", Pref("COMPANY_EMAIL"));
            customVars.Add("Company_Website", Pref("COMPANY_WEBSITE"));
            customVars.Add("Company_Fiscalnumber", Pref("COMPANY_FISCALNUMBER"));
            customVars.Add("Company_CAE", Pref("COMPANY_CAE"));
            customVars.Add("Company_Stock_Capital", Pref("COMPANY_STOCK_CAPITAL"));
            customVars.Add("Company_Civil_Registration", Pref("COMPANY_CIVIL_REGISTRATION"));
            customVars.Add("Company_Civil_Registration_Id", Pref("COMPANY_CIVIL_REGISTRATION_ID"));
            customVars.Add("Company_Tax_Entity", Pref("COMPANY_TAX_ENTITY"));
            //Report
            customVars.Add("Report_Filename_Logo", Pref("REPORT_FILENAME_LOGO"));
            customVars.Add("Report_Footer_Line1", Pref("REPORT_FOOTER_LINE1"));
            customVars.Add("Report_Footer_Line2", Pref("REPORT_FOOTER_LINE2"));
            //Ticket
            customVars.Add("Ticket_Filename_Logo", Pref("TICKET_FILENAME_LOGO"));
            customVars.Add("Ticket_Footer_Line1", Pref("TICKET_FOOTER_LINE1"));
            customVars.Add("Ticket_Footer_Line2", Pref("TICKET_FOOTER_LINE2"));
            //Session
            customVars.Add("Session_Logged_User", String.Empty);//Not Yet Assigned (BootStrap), This is Assigned on Report Constructor

            if (GlobalFramework.Settings["POS_CurrentTerminal"] != null)
            {
                customVars.Add("Session_Logged_Terminal", GlobalFramework.Settings["POS_CurrentTerminal"]);
            }

            GlobalFramework.FastReportCustomVars = customVars;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Custom Functions

        /// <summary>
        /// Get Resource String from Resources
        /// </summary>
        /// <param name="ResourceName"></param>
        /// <returns>Resource String</returns>
        public static string GetParam(string pKey)
        {
            try
            {
                string result = string.Format("UNDEFINED [{0}]", pKey);
                if(GlobalFramework.FastReportCustomVars.ContainsKey(pKey))
                {
                    result = GlobalFramework.FastReportCustomVars[pKey];
                }
                return result;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return "ERROR";
            }
        }

        /// <summary>
        /// Get Resource String from Resources
        /// </summary>
        /// <param name="ResourceName"></param>
        /// <returns>Resource String</returns>
        public static string Res(string pResourceName)
        {
            try
            {
                string resourceName = pResourceName;

                //Override to Lower (Default)
                if (_lowerCaseResource) resourceName = pResourceName.ToLower();

                string result = (_resourceManager.GetString(resourceName) != null)
                  ? _resourceManager.GetString(resourceName)
                  : string.Format("UNDEFINED [{0}]", resourceName);

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

        /// <summary>
        /// Convert Decimal to ExtendedValue
        /// </summary>
        /// <param name="Value"></param>
        public static string ExtendedValue(decimal pValue, string pAcronym)
        {
            try
            {
                ExtendValue extendValue = new ExtendValue();
                return extendValue.GetExtendedValue(pValue, pAcronym);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return "ERROR";
            }
        }

        //Replaced by Vars
        /// <summary>
        /// Get Licence Data from Token
        /// Version,Date,Name,Company,Nif,Address,Email,Telephone,HardwareId
        /// </summary>
        /// <param name="Token"></param>
        /// <returns>Licence Value</returns>
        //public static string Licence(string pToken)
        //{
        //  string result;

        //  try
        //  {
        //    switch (pToken.ToLower())
        //    {
        //      case "version":
        //        result = GlobalFramework.LicenceVersion;
        //        break;
        //      case "date":
        //        result = GlobalFramework.LicenceDate;
        //        break;
        //      case "name":
        //        result = GlobalFramework.LicenceName;
        //        break;
        //      case "company":
        //        result = GlobalFramework.LicenceCompany;
        //        break;
        //      case "nif":
        //        result = GlobalFramework.LicenceNif;
        //        break;
        //      case "address":
        //        result = GlobalFramework.LicenceAddress;
        //        break;
        //      case "email":
        //        result = GlobalFramework.LicenceEmail;
        //        break;
        //      case "telephone":
        //        result = GlobalFramework.LicenceTelephone;
        //        break;
        //      case "hardwareid":
        //        result = GlobalFramework.LicenceHardwareId;
        //        break;
        //      default:
        //        result = "UNDEFINED LICENCE DATA";
        //        break;
        //    }
        //    return result;
        //  }
        //  catch (Exception ex)
        //  {
        //    _log.Error(ex.Message, ex);
        //    return "ERROR";
        //  }
        //} 
    }
}
