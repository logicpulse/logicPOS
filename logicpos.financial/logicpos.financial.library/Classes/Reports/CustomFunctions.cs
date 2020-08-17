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
            Type customFuncType = typeof(CustomFunctions);
            MethodInfo funcRes = customFuncType.GetMethod("Res", new Type[] { typeof(string) });
            MethodInfo funcGetParam = customFuncType.GetMethod("GetParam", new Type[] { typeof(string) });
            MethodInfo funcPref = customFuncType.GetMethod("Pref", new Type[] { typeof(string) });
            MethodInfo funcVar = customFuncType.GetMethod("Var", new Type[] { typeof(string) });
            MethodInfo funcLog = customFuncType.GetMethod("Log", new Type[] { typeof(string) });
            MethodInfo funcDebug = customFuncType.GetMethod("Debug", new Type[] { typeof(object) });
            MethodInfo funcExtendedValue = customFuncType.GetMethod("ExtendedValue", new Type[] { typeof(decimal), typeof(string) });

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
            customVars.Add("APP_COMPANY", SettingsApp.AppCompanyName);
            customVars.Add("APP_NAME", pAppName);
            customVars.Add("APP_VERSION", FrameworkUtils.ProductVersion);
            customVars.Add("DATE", FrameworkUtils.CurrentDateTimeAtomic().ToString(SettingsApp.DateFormat));
            if (SettingsApp.ConfigurationSystemCurrency != null)
            {
                customVars.Add("SYSTEM_CURRENCY_LABEL", SettingsApp.ConfigurationSystemCurrency.Designation);
                customVars.Add("SYSTEM_CURRENCY_ACRONYM", SettingsApp.ConfigurationSystemCurrency.Acronym);
                customVars.Add("SYSTEM_CURRENCY_SYMBOL", SettingsApp.ConfigurationSystemCurrency.Symbol);
            }
            //Licence
            customVars.Add("LICENCE_NAME", GlobalFramework.LicenceName);
            customVars.Add("LICENCE_COMPANY", GlobalFramework.LicenceCompany);
            customVars.Add("LICENCE_NIF", GlobalFramework.LicenceNif);
            customVars.Add("LICENCE_ADDRESS", GlobalFramework.LicenceAddress);
            customVars.Add("LICENCE_EMAIL", GlobalFramework.LicenceEmail);
            customVars.Add("LICENCE_TELEPHONE", GlobalFramework.LicenceTelephone);
            customVars.Add("LICENCE_RESELLER", GlobalFramework.LicenceReseller);
            //PreferencesParameters
            customVars.Add("COMPANY_NAME", Pref("COMPANY_NAME"));
            customVars.Add("COMPANY_BUSINESS_NAME", Pref("COMPANY_BUSINESS_NAME"));
            customVars.Add("COMPANY_ADDRESS", Pref("COMPANY_ADDRESS"));
            customVars.Add("COMPANY_CITY", Pref("COMPANY_CITY"));
            customVars.Add("COMPANY_POSTALCODE", Pref("COMPANY_POSTALCODE"));
            customVars.Add("COMPANY_REGION", Pref("COMPANY_REGION"));
            customVars.Add("COMPANY_COUNTRY", Pref("COMPANY_COUNTRY"));
            customVars.Add("COMPANY_COUNTRY_CODE2", Pref("COMPANY_COUNTRY_CODE2"));
            customVars.Add("COMPANY_TELEPHONE", Pref("COMPANY_TELEPHONE"));
            customVars.Add("COMPANY_MOBILEPHONE", Pref("COMPANY_MOBILEPHONE"));
            customVars.Add("COMPANY_FAX", Pref("COMPANY_FAX"));
            customVars.Add("COMPANY_EMAIL", Pref("COMPANY_EMAIL"));
            customVars.Add("COMPANY_WEBSITE", Pref("COMPANY_WEBSITE"));
            customVars.Add("COMPANY_FISCALNUMBER", Pref("COMPANY_FISCALNUMBER"));
            customVars.Add("COMPANY_CAE", Pref("COMPANY_CAE"));
            customVars.Add("COMPANY_STOCK_CAPITAL", Pref("COMPANY_STOCK_CAPITAL"));
            customVars.Add("COMPANY_CIVIL_REGISTRATION", Pref("COMPANY_CIVIL_REGISTRATION"));
            customVars.Add("COMPANY_CIVIL_REGISTRATION_ID", Pref("COMPANY_CIVIL_REGISTRATION_ID"));
            customVars.Add("COMPANY_TAX_ENTITY", Pref("COMPANY_TAX_ENTITY"));
            //Report
            customVars.Add("REPORT_FILENAME_LOGO", Pref("REPORT_FILENAME_LOGO"));
            customVars.Add("REPORT_FILENAME_LOGO_SMALL", Pref("REPORT_FILENAME_LOGO_SMALL"));
            customVars.Add("REPORT_FOOTER_LINE1", Pref("REPORT_FOOTER_LINE1"));
            customVars.Add("REPORT_FOOTER_LINE2", Pref("REPORT_FOOTER_LINE2"));
            //Ticket
            customVars.Add("TICKET_FILENAME_LOGO", Pref("TICKET_FILENAME_LOGO"));
            customVars.Add("TICKET_FOOTER_LINE1", Pref("TICKET_FOOTER_LINE1"));
            customVars.Add("TICKET_FOOTER_LINE2", Pref("TICKET_FOOTER_LINE2"));
            //Session
            customVars.Add("SESSION_LOGGED_USER", String.Empty);//Not Yet Assigned (BootStrap), This is Assigned on Report Constructor

            if (GlobalFramework.Settings["POS_CURRENTTERMINAL"] != null)
            {
                customVars.Add("SESSION_LOGGED_TERMINAL", GlobalFramework.Settings["POS_CURRENTTERMINAL"]);
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
                if (GlobalFramework.FastReportCustomVars.ContainsKey(pKey.ToUpper()))
                {
                    result = GlobalFramework.FastReportCustomVars[pKey.ToUpper()];
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
                if(resourceName == "global_documentfinance_type_title_fs")
                {
                    result = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_documentfinance_type_title_fs");
                }

                //_log.Debug(string.Format("Message: [{0}]", resourceName));

                // Override default values - Non Default Country Replaceables
                /* IN005993 */
                /*if (!SettingsApp.ConfigurationSystemCountry.Equals(SettingsApp.XpoOidConfigurationCountryPortugal))
                    if (resourceName.Equals("global_documentfinance_type_report_invoice_footer_at"))
                    {
                        if (GlobalFramework.CurrentCulture.Name.Equals("pt-PT"))
                            result = result.Replace(" - Alinea f) do n.5 do art. 36 do CIVA", string.Empty);
                        else if (GlobalFramework.CurrentCulture.Name.Equals("fr-FR"))
                            result = result.Replace(" - Alinea f) n.5 de l´art. 36 du Code de la IVA", string.Empty);
                        else 
                            result = result.Replace(" - paragraph f) n.5 of art. 36 of the Vat Code", string.Empty);
                    }*/

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
