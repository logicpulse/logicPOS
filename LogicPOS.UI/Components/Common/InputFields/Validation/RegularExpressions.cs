using LogicPOS.UI.Services;
using System.Globalization;

namespace LogicPOS.UI.Components.InputFields.Validation
{
    public class RegularExpressions
    {
        private static string DecimalSeparator => CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        public static string Money => $@"^(?!0+(?:{DecimalSeparator}0+)?$)\d+({DecimalSeparator}\d{{1,2}})?$";
        public static string NullableMoney => $@"^\d+({DecimalSeparator}\d{{1,2}})?$";
        public static string DecimalNumber => $@"^\d+(\{DecimalSeparator}\d+)?$";
        public static string Quantity => $@"^(?!-?0({DecimalSeparator}0{{1,}})?$)-?\d+({DecimalSeparator}\d+)?$";
        public static string PositiveQuantity => $@"^(0*[1-9]\d*(\{DecimalSeparator}\d+)?|0*\{DecimalSeparator}\d*[1-9]\d*)$";
        public static string IntegerNumber => @"^\d+$";
        public static string AlphaNumeric => @"^[a-zA-Z0-9]+$";
        public static string ExtendedAlphaNumeric => @"^[À-ÿ\-_ \s\w]*$";
        public static string AlfaNumericExtendedForMotive => @"^[À-ÿ€$&@#%ºª(){}\[\]';:!?<>+\-_ \.\,\s\\\/\*\w]{1,50}$";
        public static string AlfaNumericExtended => @"^[À-ÿ€$&@#%ºª(){}\[\]';:!?<>+\-_ \.\,\s\\\/\*\w]{1,200}$";
        public static string DecimalGreaterThanZero => @"^\s*(?=.*[1-9])\d*(?:[\.,]\d{1,})?\s*$";
        public static string DecimalGreaterEqualThanZeroFinancial => @"^\s*(?=.*[0-9])\d*(?:[\.,]\d{1,2})?\s*$";
        public static string PhoneNumber => @"^[0-9+().\s\-]{0,20}$";
        public static string Email => @"^(|([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}){1,25})+([,.](([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}){1,25})+)*$";
        public static string Date => @"^(18|20)\d{2}-(0[1-9]|1[0-2])-(0[1-9]|[12][0-9]|3[01])$";
        public static string DateTime => @"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}$";
        public static string DateYear => @"^((19|20)\d\d)$";
        private static string LoginPin => @"^[0-9]{4,}$";
        private static string AlfaNumericPassword => @"^[A-Za-z0-9]{4,30}$";
        public static string AlfaNumericArticleCode => @"^.{0,}$";
        public static string PostalCode = @"^\d{4}-\d{3}$";
        public static string AlfaAcronym1Char => @"^[A-Za-záéíóúÁÉÍÓÚàèìòùÀÈÌÒÙãõñÃÕÑâêîôûÂÊÎÔÛçÇ \s]{1}$";
        public static string AlfaCountryCode2 => @"^(AF|AX|AL|DZ|AS|AD|AO|AI|AQ|AG|AR|AM|AW|AU|AT|AZ|BS|BH|BD|BB|BY|BE|BZ|BJ|BM|BT|BO|BQ|BA|BW|BV|BR|IO|BN|BG|BF|BI|KH|CM|CA|CV|KY|CF|TD|CL|CN|CX|CC|CO|KM|CG|CD|CK|CR|CI|HR|CU|CW|CY|CZ|DK|DJ|DM|DO|EC|EG|SV|GQ|ER|EE|ET|FK|FO|FJ|FI|FR|GF|PF|TF|GA|GM|GE|DE|GH|GI|GR|GL|GD|GP|GU|GT|GG|GN|GW|GY|HT|HM|VA|HN|HK|HU|IS|IN|ID|IR|IQ|IE|IM|IL|IT|JM|JP|JE|JO|KZ|KE|KI|KP|KR|KW|KG|LA|LV|LB|LS|LR|LY|LI|LT|LU|MO|MK|MG|MW|MY|MV|ML|MT|MH|MQ|MR|MU|YT|MX|FM|MD|MC|MN|ME|MS|MA|MZ|MM|NA|NR|NP|NL|NC|NZ|NI|NE|NG|NU|NF|MP|NO|OM|PK|PW|PS|PA|PG|PY|PE|PH|PN|PL|PT|PR|QA|RE|RO|RU|RW|BL|SH|KN|LC|MF|PM|VC|WS|SM|ST|SA|SN|RS|SC|SL|SG|SX|SK|SI|SB|SO|ZA|GS|SS|ES|LK|SD|SR|SJ|SZ|SE|CH|SY|TW|TJ|TZ|TH|TL|TG|TK|TO|TT|TN|TR|TM|TC|TV|UG|UA|AE|GB|US|UM|UY|UZ|VU|VE|VN|VG|VI|WF|EH|YE|ZM|ZW|PT-AC|PT-MA)$";
        public static string Acronym2Chars => @"^[0-9A-Za-z]{2}$";
        private static string AngolanFiscalNumber => @"^[0-9A-Z]{9,}$";
        private static string PortugueseFiscalNumber => @"^[0-9]{9,}$";
        public static string LoginPassword
        {
            get
            {
                if (SystemInformationService.SystemInformation.IsAngola)
                {
                    return AlfaNumericPassword;
                }

                return LoginPin;
            }
        }
        public static string GetFiscalNumberRegexForCountry(string countryCode2)
        {
            switch(countryCode2.ToLower())
            {
                case "pt":
                return PortugueseFiscalNumber;
                case "ao":
                    return AngolanFiscalNumber;
                default:
                return "^[A-Za-z0-9]{5,}$";
            }
        }
    }
}
