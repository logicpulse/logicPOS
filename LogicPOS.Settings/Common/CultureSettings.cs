using System;
using System.Globalization;

namespace LogicPOS.Settings
{
    public static class CultureSettings
    {
        public static CultureInfo CurrentCulture { get; set; }
        public static CultureInfo CurrentCultureNumberFormat { get; set; }

        public static string CurrentCultureName => GeneralSettings.Settings["customCultureResourceDefinition"];

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

        public static Guid PortugalCountryId { get; set; } = new Guid("e7e8c325-a0d4-4908-b148-508ed750676a");
        public static Guid EuroCurrencyId { get; set; } = new Guid("28dd2a3a-0083-11e4-96ce-00ff2353398c");
        public static Guid MozambiqueCountryId { get; set; } = new Guid("16fcd7f2-e885-48d8-9f8e-9d224cc36f32");
        public static Guid USDCurrencyId { get; set; } = new Guid("28d692ad-0083-11e4-96ce-00ff2353398c");
        public static Guid AngolaCountryId { get; set; } = new Guid("9655510a-ff58-461e-9719-c037058f10ed");

        public static bool CountryIdIsPortugal(Guid countryId)
        {
            return countryId == PortugalCountryId;
        }

        public static bool CountryIdIsMozambique(Guid countryId)
        {
            return countryId == MozambiqueCountryId;
        }

        public static bool CountryIdIsAngola(Guid countryId)
        {
            return countryId == AngolaCountryId;
        }
    }
}
