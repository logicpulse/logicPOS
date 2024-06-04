using FastReport.Utils;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Settings;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace LogicPOS.Reporting
{
    public static class CustomFunctions
    {
        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static bool _useLowerCaseResources = true;


        //Using Outside Project Resources: Extended for JoanaReports
        public static void Register(string pAppName)
        {
            RegisterFunctions();
            RegisterSystemVars(pAppName);
            RegisterCustomVars(pAppName);
        }

        private static void RegisterFunctions()
        {
            RegisteredObjects.AddFunctionCategory("Custom", "Custom Functions");

            Type customFuncType = typeof(CustomFunctions);

            var getResourceMethodName = nameof(Utility.ResourcesUtility.GetResourceByName);
            MethodInfo getResourceFunction = typeof(Utility.ResourcesUtility).GetMethod(getResourceMethodName, new Type[] { typeof(string) });

            MethodInfo funcGetParam = customFuncType.GetMethod(nameof(GetParam), new Type[] { typeof(string) });
            MethodInfo funcPref = customFuncType.GetMethod(nameof(Pref), new Type[] { typeof(string) });
            MethodInfo funcVar = customFuncType.GetMethod(nameof(Var), new Type[] { typeof(string) });
            MethodInfo funcLog = customFuncType.GetMethod(nameof(Log), new Type[] { typeof(string) });
            MethodInfo funcDebug = customFuncType.GetMethod(nameof(Debug), new Type[] { typeof(object) });
            MethodInfo funcExtendedValue = customFuncType.GetMethod(nameof(ExtendedValue), new Type[] { typeof(decimal), typeof(string) });

            //Register simple function
            RegisteredObjects.AddFunction(getResourceFunction, "Custom");
            RegisteredObjects.AddFunction(funcGetParam, "Custom");
            RegisteredObjects.AddFunction(funcPref, "Custom");
            RegisteredObjects.AddFunction(funcVar, "Custom");
            RegisteredObjects.AddFunction(funcLog, "Custom");
            RegisteredObjects.AddFunction(funcDebug, "Custom");
            RegisteredObjects.AddFunction(funcExtendedValue, "Custom");
        }

        private static void RegisterSystemVars(string pAppName)
        {
            if (PrintingSettings.FastReportSystemVars == null)
            {
                Dictionary<string, string> systemVars = new Dictionary<string, string>
                {
                    { "PreparedPages", "0" }
                };
                PrintingSettings.FastReportSystemVars = systemVars;
            }
        }

        /// <summary>
        /// Register Custom Variables to Use With Func Var and Assign it to GlobalFramework.FastReportCustomVars
        /// </summary>
        public static void RegisterCustomVars(string pAppName)
        {
            Dictionary<string, string> customVars = new Dictionary<string, string>
            {
                //App
                { "APP_COMPANY", PluginSettings.AppCompanyName },
                { "APP_NAME", pAppName },
                { "APP_VERSION", GeneralSettings.ProductVersion },
                { "DATE", XPOUtility.CurrentDateTimeAtomic().ToString(CultureSettings.DateFormat) }
            };
            if (XPOSettings.ConfigurationSystemCurrency != null)
            {
                customVars.Add("SYSTEM_CURRENCY_LABEL", XPOSettings.ConfigurationSystemCurrency.Designation);
                customVars.Add("SYSTEM_CURRENCY_ACRONYM", XPOSettings.ConfigurationSystemCurrency.Acronym);
                customVars.Add("SYSTEM_CURRENCY_SYMBOL", XPOSettings.ConfigurationSystemCurrency.Symbol);
            }
            //Licence
            customVars.Add("LICENCE_NAME", LicenseSettings.LicenseName);
            customVars.Add("LICENCE_COMPANY", LicenseSettings.LicenseCompany);
            customVars.Add("LICENCE_NIF", LicenseSettings.LicenseNif);
            customVars.Add("LICENCE_ADDRESS", LicenseSettings.LicenseAddress);
            customVars.Add("LICENCE_EMAIL", LicenseSettings.LicenseEmail);
            customVars.Add("LICENCE_TELEPHONE", LicenseSettings.LicenseTelephone);
            customVars.Add("LICENCE_RESELLER", LicenseSettings.LicenseReseller);
            //PreferencesParameters
            customVars.Add("COMPANY_NAME", PreferencesUtils.GetPreferenceParameterFromToken("COMPANY_NAME"));
            customVars.Add("COMPANY_BUSINESS_NAME", PreferencesUtils.GetPreferenceParameterFromToken("COMPANY_BUSINESS_NAME"));
            customVars.Add("COMPANY_ADDRESS", PreferencesUtils.GetPreferenceParameterFromToken("COMPANY_ADDRESS"));
            customVars.Add("COMPANY_CITY", PreferencesUtils.GetPreferenceParameterFromToken("COMPANY_CITY"));
            customVars.Add("COMPANY_POSTALCODE", PreferencesUtils.GetPreferenceParameterFromToken("COMPANY_POSTALCODE"));
            customVars.Add("COMPANY_REGION", PreferencesUtils.GetPreferenceParameterFromToken("COMPANY_REGION"));
            customVars.Add("COMPANY_COUNTRY", PreferencesUtils.GetPreferenceParameterFromToken("COMPANY_COUNTRY"));
            customVars.Add("COMPANY_COUNTRY_CODE2", PreferencesUtils.GetPreferenceParameterFromToken("COMPANY_COUNTRY_CODE2"));
            customVars.Add("COMPANY_TELEPHONE", PreferencesUtils.GetPreferenceParameterFromToken("COMPANY_TELEPHONE"));
            customVars.Add("COMPANY_MOBILEPHONE", PreferencesUtils.GetPreferenceParameterFromToken("COMPANY_MOBILEPHONE"));
            customVars.Add("COMPANY_FAX", PreferencesUtils.GetPreferenceParameterFromToken("COMPANY_FAX"));
            customVars.Add("COMPANY_EMAIL", PreferencesUtils.GetPreferenceParameterFromToken("COMPANY_EMAIL"));
            customVars.Add("COMPANY_WEBSITE", PreferencesUtils.GetPreferenceParameterFromToken("COMPANY_WEBSITE"));
            customVars.Add("COMPANY_FISCALNUMBER", PreferencesUtils.GetPreferenceParameterFromToken("COMPANY_FISCALNUMBER"));
            customVars.Add("COMPANY_CAE", PreferencesUtils.GetPreferenceParameterFromToken("COMPANY_CAE"));
            customVars.Add("COMPANY_STOCK_CAPITAL", PreferencesUtils.GetPreferenceParameterFromToken("COMPANY_STOCK_CAPITAL"));
            customVars.Add("COMPANY_CIVIL_REGISTRATION", PreferencesUtils.GetPreferenceParameterFromToken("COMPANY_CIVIL_REGISTRATION"));
            customVars.Add("COMPANY_CIVIL_REGISTRATION_ID", PreferencesUtils.GetPreferenceParameterFromToken("COMPANY_CIVIL_REGISTRATION_ID"));
            customVars.Add("COMPANY_TAX_ENTITY", PreferencesUtils.GetPreferenceParameterFromToken("COMPANY_TAX_ENTITY"));
            //Report
            customVars.Add("REPORT_FILENAME_loggerO", PreferencesUtils.GetPreferenceParameterFromToken("REPORT_FILENAME_loggerO"));
            customVars.Add("REPORT_FILENAME_loggerO_SMALL", PreferencesUtils.GetPreferenceParameterFromToken("REPORT_FILENAME_loggerO_SMALL"));
            customVars.Add("REPORT_FOOTER_LINE1", PreferencesUtils.GetPreferenceParameterFromToken("REPORT_FOOTER_LINE1"));
            customVars.Add("REPORT_FOOTER_LINE2", PreferencesUtils.GetPreferenceParameterFromToken("REPORT_FOOTER_LINE2"));
            //Ticket
            customVars.Add("TICKET_FILENAME_loggerO", PreferencesUtils.GetPreferenceParameterFromToken("TICKET_FILENAME_loggerO"));
            customVars.Add("TICKET_FOOTER_LINE1", PreferencesUtils.GetPreferenceParameterFromToken("TICKET_FOOTER_LINE1"));
            customVars.Add("TICKET_FOOTER_LINE2", PreferencesUtils.GetPreferenceParameterFromToken("TICKET_FOOTER_LINE2"));
            //Session
            customVars.Add("SESSION_loggerGED_USER", string.Empty);//Not Yet Assigned (BootStrap), This is Assigned on Report Constructor

            if (GeneralSettings.Settings["POS_CURRENTTERMINAL"] != null)
            {
                customVars.Add("SESSION_loggerGED_TERMINAL", GeneralSettings.Settings["POS_CURRENTTERMINAL"]);
            }

            PrintingSettings.FastReportCustomVars = customVars;
        }

        public static string GetParam(string key)
        {
            try
            {
                string result = $"Key not found [{key}]";

                if (PrintingSettings.FastReportCustomVars.ContainsKey(key.ToUpper()))
                {
                    result = PrintingSettings.FastReportCustomVars[key.ToUpper()];
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
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
                string result = (PrintingSettings.FastReportCustomVars.ContainsKey(pToken.ToUpper()))
                  ? PrintingSettings.FastReportCustomVars[pToken.ToUpper()]
                  : string.Format("UNDEFINED [{0}]", pToken);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
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
                string result = (GeneralSettings.PreferenceParameters.ContainsKey(pToken.ToUpper()))
                  ? GeneralSettings.PreferenceParameters[pToken.ToUpper()]
                  : string.Format("UNDEFINED [{0}]", pToken);

                return result;
            }
            catch (Exception ex)
            {
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
                NumberToWordsUtility extendValue = new NumberToWordsUtility();
                return extendValue.GetExtendedValue(pValue, pAcronym);
            }
            catch (Exception ex)
            {
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
                _logger.Debug(string.Format("FastReport: [{0}]", pOutput));
                return "LOG";
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
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
                _logger.Debug(string.Format("FastReport: Debug Object Type [{0}]", pObject.GetType()));
                return "DEBUG";
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return "ERROR";
            }
        }
    }
}
