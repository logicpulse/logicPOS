using LogicPOS.UI.Services;
using System.Globalization;

namespace LogicPOS.UI.Components.InputFields.Validation
{
    public class RegularExpressions
    {
        private static string DecimalSeparator => CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        public static string Money => $@"^\d+(\{DecimalSeparator}\d{{1,2}})?$";
        public static string DecimalNumber => $@"^\d+(\{DecimalSeparator}\d+)?$";
        public static string Quantity => @"^-?\d+(\" + DecimalSeparator + @"\d+)?$";
        public static string PositiveQuantity => $@"^(0*[1-9]\d*(\{DecimalSeparator}\d+)?|0*\{DecimalSeparator}\d*[1-9]\d*)$";
        public static string IntegerNumber => @"^\d+$";
        public static string Alphanumeric => @"^[a-zA-Z0-9]+$";
        public static string Alfa => @"^[A-Za-záéíóúÁÉÍÓÚàèìòùÀÈÌÒÙãõñÃÕÑâêîôûÂÊÎÔÛçÇ \s]*$";
        public static string AlfaNumeric => @"^[À-ÿ\-_ \s\w]*$";
        public static string AlfaNumericPlus => @"^[À-ÿ&ºª,;'\.\\\/\*\- \w]*$";
        public static string AlfaNumericExtendedForMotive => @"^[À-ÿ€$&@#%ºª(){}\[\]';:!?<>+\-_ \.\,\s\\\/\*\w]{1,50}$";
        public static string AlfaNumericExtended => @"^[À-ÿ€$&@#%ºª(){}\[\]';:!?<>+\-_ \.\,\s\\\/\*\w]{1,200}$";
        public static string AlfaNumericEmail => @"^[À-ÿ€$&@#%ºª(){}\[\]';:!?<>+\-_ \.\,\s\\\/\*\w]*$";
        public static string AlfaNumericFilePath => @"^[0-9A-Za-z\-\\\.:_\/ \s]*$";
        public static string Integer => @"^\d+$";
        public static string IntegerGreaterThanZero => @"^[1-9][0-9]*$";
        public static string IntegerColonSeparated => @"^(\d+(:\d+)*)?$";
        public static string DecimalPositiveAndNegative => @"^-?\s*(?=.*[1-9])\d*(?:[\.,]\d{1,})?\s*$";
        public static string DecimalGreaterThanZero => @"^\s*(?=.*[1-9])\d*(?:[\.,]\d{1,})?\s*$";
        public static string DecimalGreaterEqualThanZero => @"^\s*(?=.*[0-9])\d*(?:[\.,]\d{1,})?\s*$";
        public static string DecimalGreaterEqualThanZeroFinancial => @"^\s*(?=.*[0-9])\d*(?:[\.,]\d{1,2})?\s*$";
        public static string Ean12andEan4 => @"^[0-9A-Za-z]+";
        public static string Email => @"^(|([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}){1,25})+([,.](([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}){1,25})+)*$";
        public static string Date => @"^\d{4}-\d{2}-\d{2}$";
        public static string DateTime => @"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}$";
        public static string DateYear => @"^((19|20)\d\d)$";
        public static string DateYearHolidays => @"^((19|20)\d\d)|0$";
        public static string DateMonth => @"^(0?[1-9]|1[012])$";
        public static string DateDay => @"^(0?[1-9]|[12][0-9]|3[01])$";
        public static string Percentage => @"^(100([\.\,][0]{1,})?$|[0-9]{1,2}([\.\,][0-9]{1,})?)$";
        public static string LoginPin => @"^[0-9]{4,}$";
        public static string PrintCopies => @"^[1-4]$";
        public static string DocumentSeriesAcronym => @"^[0-9A-Za-z]{4,12}$";
        public static string DocumentSeriesYearAcronym => @"^[0-9A-Za-z]{4,12}$";
        public static string AlfaNumericArticleCode => @"^[0-9A-Za-z€$&@#%()\[\]+\-_ \.\\\/]{2,25}$";
        public static string AlfaNumericArticleButtonLabel => @"^[0-9A-Za-zéúíóáÉÚÍÓÁèùìòàÈÙÌÒÀõãñÕÃÑêûîôâÊÛÎÔÂçÇ€$&@#%ºª()\[\]';:!?<>+\-_ \.\,\s\\\/\*]{2,35}$";
        public static string AlfaAcronym1Char => @"^[A-Za-záéíóúÁÉÍÓÚàèìòùÀÈÌÒÙãõñÃÕÑâêîôûÂÊÎÔÛçÇ \s]{1}$";
        public static string AlfaCountryCode2 => @"^(AF|AX|AL|DZ|AS|AD|AO|AI|AQ|AG|AR|AM|AW|AU|AT|AZ|BS|BH|BD|BB|BY|BE|BZ|BJ|BM|BT|BO|BQ|BA|BW|BV|BR|IO|BN|BG|BF|BI|KH|CM|CA|CV|KY|CF|TD|CL|CN|CX|CC|CO|KM|CG|CD|CK|CR|CI|HR|CU|CW|CY|CZ|DK|DJ|DM|DO|EC|EG|SV|GQ|ER|EE|ET|FK|FO|FJ|FI|FR|GF|PF|TF|GA|GM|GE|DE|GH|GI|GR|GL|GD|GP|GU|GT|GG|GN|GW|GY|HT|HM|VA|HN|HK|HU|IS|IN|ID|IR|IQ|IE|IM|IL|IT|JM|JP|JE|JO|KZ|KE|KI|KP|KR|KW|KG|LA|LV|LB|LS|LR|LY|LI|LT|LU|MO|MK|MG|MW|MY|MV|ML|MT|MH|MQ|MR|MU|YT|MX|FM|MD|MC|MN|ME|MS|MA|MZ|MM|NA|NR|NP|NL|NC|NZ|NI|NE|NG|NU|NF|MP|NO|OM|PK|PW|PS|PA|PG|PY|PE|PH|PN|PL|PT|PR|QA|RE|RO|RU|RW|BL|SH|KN|LC|MF|PM|VC|WS|SM|ST|SA|SN|RS|SC|SL|SG|SX|SK|SI|SB|SO|ZA|GS|SS|ES|LK|SD|SR|SJ|SZ|SE|CH|SY|TW|TJ|TZ|TH|TL|TG|TK|TO|TT|TN|TR|TM|TC|TV|UG|UA|AE|GB|US|UM|UY|UZ|VU|VE|VN|VG|VI|WF|EH|YE|ZM|ZW|PT-AC|PT-MA)$";
        public static string AlfaCountryCode3 => @"^(ABW|AFG|AGO|AIA|ALA|ALB|AND|ARE|ARG|ARM|ASM|ATA|ATF|ATG|AUS|AUT|AZE|BDI|BEL|BEN|BES|BFA|BGD|BGR|BHR|BHS|BIH|BLM|BLR|BLZ|BMU|BOL|BRA|BRB|BRN|BTN|BVT|BWA|CAF|CAN|CCK|CHE|CHL|CHN|CIV|CMR|COD|COG|COK|COL|COM|CPV|CRI|CUB|CUW|CXR|CYM|CYP|CZE|DEU|DJI|DMA|DNK|DOM|DZA|ECU|EGY|ERI|ESH|ESP|EST|ETH|FIN|FJI|FLK|FRA|FRO|FSM|GAB|GBR|GEO|GGY|GHA|GIB|GIN|GLP|GMB|GNB|GNQ|GRC|GRD|GRL|GTM|GUF|GUM|GUY|HKG|HMD|HND|HRV|HTI|HUN|IDN|IMN|IND|IOT|IRL|IRN|IRQ|ISL|ISR|ITA|JAM|JEY|JOR|JPN|KAZ|KEN|KGZ|KHM|KIR|KNA|KOR|KWT|LAO|LBN|LBR|LBY|LCA|LIE|LKA|LSO|LTU|LUX|LVA|MAC|MAF|MAR|MCO|MDA|MDG|MDV|MEX|MHL|MKD|MLI|MLT|MMR|MNE|MNG|MNP|MOZ|MRT|MSR|MTQ|MUS|MWI|MYS|MYT|NAM|NCL|NER|NFK|NGA|NIC|NIU|NLD|NOR|NPL|NRU|NZL|OMN|PAK|PAN|PCN|PER|PHL|PLW|PNG|POL|PRI|PRK|PRT|PRY|PSE|PYF|QAT|REU|ROU|RUS|RWA|SAU|SDN|SEN|SGP|SGS|SHN|SJM|SLB|SLE|SLV|SMR|SOM|SPM|SRB|SSD|STP|SUR|SVK|SVN|SWE|SWZ|SXM|SYC|SYR|TCA|TCD|TGO|THA|TJK|TKL|TKM|TLS|TON|TTO|TUN|TUR|TUV|TWN|TZA|UGA|UKR|UMI|URY|USA|UZB|VAT|VCT|VEN|VGB|VIR|VNM|VUT|WLF|WSM|YEM|ZAF|ZMB|ZWE|PRT-AC|PRT-MA)$";
        public static string Acronym2Chars => @"^[0-9A-Za-z]{2}$";
        public static string Acronym3Chars => @"^[0-9A-Za-z]{3}$";
        public static string Acronym2Or3Chars => @"^[0-9A-Za-z]{2,3}$";
        public static string IntegerSplittedByComma => @"^(\d+(,\d+)*)?$";
        public static string HardwareVidAndPid => @"^0x[0-9A-Fa-f]{4}$";
        public static string HardwareEndpoint => @"^Ep[0-9]{1,2}$";
        public static string HardwareCodeTable => @"^0x[0-9]{1,2}$";
        public static string HardwarePortName => @"^(COM1|COM2|COM3|COM4|COM5|COM6|COM7|COM8|COM9|COM10|COM11|COM12|COM13|COM14|COM15)$";
        public static string HardwareBaudRate => @"^(300|600|1200|2400|4800|9600|14400|28800|36000|115000)$";
        public static string HardwareParity => @"^(None|Odd|Even|Mark|Space)$";
        public static string HardwareStopBits => @"^(None|One|Two|OnePointFive)$";
        public static string HardwareDataBits => @"^(7|8|9)$";
        public static string Culture = @"^(en-GB|en-US|es-ES|fr-FR|pt-AO|pt-BR|pt-MZ|pt-PT)$";
        public static string AngolanFiscalNumber => @"^[0-9A-Z]{10,}$";
        public static string PortugueseFiscalNumber => @"^[0-9]{9,}$";

        public static string FiscalNumber
        {
            get
            {
                switch (PreferenceParametersService.CompanyInformations.CountryCode2.ToLower())
                {
                    case "ao":
                        return AngolanFiscalNumber;
                    default:
                        return PortugueseFiscalNumber;
                }
            }
        }
    }
}
