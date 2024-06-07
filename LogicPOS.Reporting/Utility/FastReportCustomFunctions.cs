using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;

namespace LogicPOS.Reporting.Utility
{
    public static class FastReportCustomFunctions
    {  
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
                return $"ERROR ({ex.Message})";
            }
        }

        public static string Var(string token)
        {
            try
            {
                string result = PrintingSettings.FastReportCustomVars.ContainsKey(token.ToUpper())
                  ? PrintingSettings.FastReportCustomVars[token.ToUpper()]
                  : string.Format("UNDEFINED [{0}]", token);
                return result;
            }
            catch (Exception ex)
            {
                return $"ERROR ({ex.Message})";
            }
        }

        public static string Pref(string token)
        {
            try
            {
                string result = GeneralSettings.PreferenceParameters.ContainsKey(token.ToUpper())
                  ? GeneralSettings.PreferenceParameters[token.ToUpper()]
                  : string.Format("UNDEFINED [{0}]", token);

                return result;
            }
            catch (Exception ex)
            {
                return $"ERROR ({ex.Message})";
            }
        }

        public static string ExtendedValue(
            decimal value, 
            string acronym)
        {
            try
            {
                NumberToWordsUtility extendValue = new NumberToWordsUtility();
                return extendValue.GetExtendedValue(value, acronym);
            }
            catch (Exception ex)
            {
                return $"ERROR {ex.Message}";
            }
        }


        public static string Log(string output)
        {
            return "Not Implemented";
        }

        public static string Debug(object pObject)
        {
           return "Not Implemented";
        }

        public static string GetResourceByName(string resourceName)
        {
            return CultureResources.GetResourceByLanguage(
                CultureSettings.CurrentCultureName,
                resourceName);
        }
    }
}
