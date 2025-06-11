using System;
using System.Globalization;

namespace LogicPOS.Settings
{
    public static class CultureSettings
    {
        public static CultureInfo CurrentCulture { get; set; }
        public static CultureInfo CurrentCultureNumberFormat { get; set; }
        public static string CurrentCultureName => AppSettings.Instance.customCultureResourceDefinition;
        public static string DateFormat { get; set; } = "yyyy-MM-dd";
        public static string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";
        public static string FileFormatDateTime { get; set; }
        public static string FileFormatSaftPT { get; set; }
        public static string FileFormatSaftAO { get; set; }
        public static string DateTimeFormatDocumentDate { get; set; } = "yyyy-MM-dd";
        public static string DateTimeFormatCombinedDateTime { get; set; } = "yyyy-MM-ddTHH:mm:ss";
        public static string DecimalFormatSAFTPT { get; set; }
        public static string DecimalFormatSAFTAO { get; set; }
        public static string DecimalFormatGrossTotalSAFTPT { get; set; }
        public static int DecimalRoundTo { get; set; }

        public static bool OSHasCulture(string culture)
        {
            foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                if (culture == CultureInfo.CreateSpecificCulture(ci.Name).Name)
                {
                    return true;
                }

            }
            return false;
        }
    }
}
