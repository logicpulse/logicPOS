using FastReport.Utils;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Settings;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace LogicPOS.Reporting.Utility
{
    public static class FastReportUtils
    {
        public static void InitializeFastReports(string appName)
        {
            RegisterCustomFunctions();
            RegisterSystemVars(appName);
            RegisterCustomVars(appName);
        }

        private static void RegisterCustomFunctions()
        {
            RegisteredObjects.AddFunctionCategory("Custom", "Custom Functions");

            Type customFunctions = typeof(FastReportCustomFunctions);
            
            var functions = customFunctions.GetMethods(BindingFlags.Public | BindingFlags.Static);

            foreach (var function in functions)
            {
                RegisteredObjects.AddFunction(function,"Custom");
            }
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

        public static string GetReportFilePath(string reportFileName)
        {
            string fileLocation = $"{PathsSettings.Paths["reports"]}{"UserReports"}\\{reportFileName}";

            if (!File.Exists(fileLocation))
            {
                throw new FileNotFoundException($"Report File Not Found: {fileLocation}");
            }

            return fileLocation;
        }
    }
}
