using System.Globalization;

namespace LogicPOS.Settings
{
    public static class CultureSettings
    {
        public static CultureInfo CurrentCulture { get; set; }
        public static CultureInfo CurrentCultureNumberFormat { get; set; }

        public static string DateFormat { get; set; } = "yyyy-MM-dd";
        public static string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";
        public static string DateTimeFormatHour { get; set; } = "HH:mm:ss";
        public static string DecimalFormat { get; set; } = "0.00";
        public static string DecimalFormatStockQuantity { get; set; } = "0.00000000";
        public static string FileFormatDateTime { get; set; }
        public static string FileFormatSaftPT { get; set; }
        public static string FileFormatSaftAO { get; set; }
        public static string DateTimeFormatDocumentDate { get; set; } = "yyyy-MM-dd";
        public static string DateTimeFormatCombinedDateTime { get; set; } = "yyyy-MM-ddTHH:mm:ss";
        public static string DecimalFormatSAFTPT { get; set; }
        public static string DecimalFormatSAFTAO { get; set; }
        public static string DecimalFormatGrossTotalSAFTPT { get; set; }
        public static int DecimalRoundTo { get; set; }
        public static string SaftCurrencyCode { get; set; }
        public static string SaftCurrencyCodeAO { get; set; }
    }
}
