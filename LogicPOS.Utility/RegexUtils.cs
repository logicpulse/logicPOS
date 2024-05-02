namespace LogicPOS.Utility
{
    public static class RegexUtils
    {
        public static string RegexAlfa => @"^[A-Za-záéíóúÁÉÍÓÚàèìòùÀÈÌÒÙãõñÃÕÑâêîôûÂÊÎÔÛçÇ \s]*$";
        public static string RegexAlfaNumeric => @"^[À-ÿ\-_ \s\w]*$";
        public static string RegexAlfaNumericPlus => @"^[À-ÿ&ºª,;'\.\\\/\*\- \w]*$";
        public static string RegexAlfaNumericExtendedForMotive => @"^[À-ÿ€$&@#%ºª(){}\[\]';:!?<>+\-_ \.\,\s\\\/\*\w]{1,50}$";
        public static string RegexAlfaNumericExtended => @"^[À-ÿ€$&@#%ºª(){}\[\]';:!?<>+\-_ \.\,\s\\\/\*\w]{1,200}$";
        public static string RegexAlfaNumericEmail => @"^[À-ÿ€$&@#%ºª(){}\[\]';:!?<>+\-_ \.\,\s\\\/\*\w]*$";
        public static string RegexAlfaNumericFilePath => @"^[0-9A-Za-z\-\\\.:_\/ \s]*$";
        public static string RegexInteger => @"^\d+$";
        public static string RegexIntegerGreaterThanZero => @"^[1-9][0-9]*$";
        public static string RegexIntegerColonSeparated => @"^(\d+(:\d+)*)?$";
        public static string RegexDecimal => @"^\s*(?=.*[0-9])\d*(?:[\.,]\d{1,4})?\s*$";
        public static string RegexDecimalPositiveAndNegative => @"^-?\s*(?=.*[1-9])\d*(?:[\.,]\d{1,})?\s*$";
        public static string RegexDecimalGreaterThanZero => @"^\s*(?=.*[1-9])\d*(?:[\.,]\d{1,})?\s*$";
        public static string RegexDecimalGreaterEqualThanZero => @"^\s*(?=.*[0-9])\d*(?:[\.,]\d{1,})?\s*$";
        public static string RegexDecimalGreaterEqualThanZeroFinancial => @"^\s*(?=.*[0-9])\d*(?:[\.,]\d{1,2})?\s*$";
        public static string RegexEan12andEan4 => @"^[0-9A-Za-z]+";
        public static string RegexEmail => @"^(|([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}){1,25})+([,.](([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}){1,25})+)*$";
        public static string RegexGuid => @"^\b[a-fA-F0-9]{8}(?:-[a-fA-F0-9]{4}){3}-[a-fA-F0-9]{12}\b$";
        public static string RegexDate => @"^(((([0-9][0-9][0-9][1-9])|([1-9][0-9][0-9][0-9])|([0-9][1-9][0-9][0-9])|([0-9][0-9][1-9][0-9]))[-\/\\.]((0[13578])|(1[02]))[-\/\\.]((0[1-9])|([12][0-9])|(3[01])))|((([0-9][0-9][0-9][1-9])|([1-9][0-9][0-9][0-9])|([0-9][1-9][0-9][0-9])|([0-9][0-9][1-9][0-9]))[-\/\\.]((0[469])|11)[-\/\\.]((0[1-9])|([12][0-9])|(30)))|(((000[48])|([0-9]0-9)|([0-9][1-9][02468][048])|([1-9][0-9][02468][048])|([0-9]0-9)|([0-9][1-9][13579][26])|([1-9][0-9][13579][26]))[-\/\\.]02[-\/\\.]((0[1-9])|([12][0-9])))|((([0-9][0-9][0-9][1-9])|([1-9][0-9][0-9][0-9])|([0-9][1-9][0-9][0-9])|([0-9][0-9][1-9][0-9]))[-\/\\.]02[-\/\\.]((0[1-9])|([1][0-9]\S*)|([2][0-8]\S*)\S*)\S*)\S*)$";
        public static string RegexDateTime => @"^(((\d\d)(([02468][048])|([13579][26]))[-\/\\.]02[-\/\\.]29)|(((\d\d)(\d\d)))[-\/\\.]((((0\d)|(1[0-2]))[-\/\\.]((0\d)|(1\d)|(2[0-8])))|((((0[13578])|(1[02]))-31)|(((0[1,3-9])|(1[0-2]))-(29|30)))))\s(([01]\d|2[0-3]):([0-5]\d):([0-5]\d))$";
        public static string RegexDateYear => @"^((19|20)\d\d)$";
        public static string RegexDateYearHolidays => @"^((19|20)\d\d)|0$";
        public static string RegexDateMonth => @"^(0?[1-9]|1[012])$";
        public static string RegexDateDay => @"^(0?[1-9]|[12][0-9]|3[01])$";
        public static string RegexPercentage => @"^(100([\.\,][0]{1,})?$|[0-9]{1,2}([\.\,][0-9]{1,})?)$";
        public static string RegexLoginPin => @"^[0-9]{4,}$";
        public static string RegexPrintCopies => @"^[1-4]$";
        public static string RegexDocumentSeriesAcronym => @"^[0-9A-Za-z]{4,12}$";
        public static string RegexDocumentSeriesYearAcronym => @"^[0-9A-Za-z]{4,12}$";
        public static string RegexAlfaNumericArticleCode => @"^[0-9A-Za-z€$&@#%()\[\]+\-_ \.\\\/]{2,25}$";
        public static string RegexAlfaNumericArticleButtonLabel => @"^[0-9A-Za-zéúíóáÉÚÍÓÁèùìòàÈÙÌÒÀõãñÕÃÑêûîôâÊÛÎÔÂçÇ€$&@#%ºª()\[\]';:!?<>+\-_ \.\,\s\\\/\*]{2,35}$";
        public static string RegexAlfaAcronym1Char => @"^[A-Za-záéíóúÁÉÍÓÚàèìòùÀÈÌÒÙãõñÃÕÑâêîôûÂÊÎÔÛçÇ \s]{1}$";
        public static string RegexAlfaCountryCode2 => @"^(AF|AX|AL|DZ|AS|AD|AO|AI|AQ|AG|AR|AM|AW|AU|AT|AZ|BS|BH|BD|BB|BY|BE|BZ|BJ|BM|BT|BO|BQ|BA|BW|BV|BR|IO|BN|BG|BF|BI|KH|CM|CA|CV|KY|CF|TD|CL|CN|CX|CC|CO|KM|CG|CD|CK|CR|CI|HR|CU|CW|CY|CZ|DK|DJ|DM|DO|EC|EG|SV|GQ|ER|EE|ET|FK|FO|FJ|FI|FR|GF|PF|TF|GA|GM|GE|DE|GH|GI|GR|GL|GD|GP|GU|GT|GG|GN|GW|GY|HT|HM|VA|HN|HK|HU|IS|IN|ID|IR|IQ|IE|IM|IL|IT|JM|JP|JE|JO|KZ|KE|KI|KP|KR|KW|KG|LA|LV|LB|LS|LR|LY|LI|LT|LU|MO|MK|MG|MW|MY|MV|ML|MT|MH|MQ|MR|MU|YT|MX|FM|MD|MC|MN|ME|MS|MA|MZ|MM|NA|NR|NP|NL|NC|NZ|NI|NE|NG|NU|NF|MP|NO|OM|PK|PW|PS|PA|PG|PY|PE|PH|PN|PL|PT|PR|QA|RE|RO|RU|RW|BL|SH|KN|LC|MF|PM|VC|WS|SM|ST|SA|SN|RS|SC|SL|SG|SX|SK|SI|SB|SO|ZA|GS|SS|ES|LK|SD|SR|SJ|SZ|SE|CH|SY|TW|TJ|TZ|TH|TL|TG|TK|TO|TT|TN|TR|TM|TC|TV|UG|UA|AE|GB|US|UM|UY|UZ|VU|VE|VN|VG|VI|WF|EH|YE|ZM|ZW|PT-AC|PT-MA)$";
        public static string RegexAlfaCountryCode3 => @"^(ABW|AFG|AGO|AIA|ALA|ALB|AND|ARE|ARG|ARM|ASM|ATA|ATF|ATG|AUS|AUT|AZE|BDI|BEL|BEN|BES|BFA|BGD|BGR|BHR|BHS|BIH|BLM|BLR|BLZ|BMU|BOL|BRA|BRB|BRN|BTN|BVT|BWA|CAF|CAN|CCK|CHE|CHL|CHN|CIV|CMR|COD|COG|COK|COL|COM|CPV|CRI|CUB|CUW|CXR|CYM|CYP|CZE|DEU|DJI|DMA|DNK|DOM|DZA|ECU|EGY|ERI|ESH|ESP|EST|ETH|FIN|FJI|FLK|FRA|FRO|FSM|GAB|GBR|GEO|GGY|GHA|GIB|GIN|GLP|GMB|GNB|GNQ|GRC|GRD|GRL|GTM|GUF|GUM|GUY|HKG|HMD|HND|HRV|HTI|HUN|IDN|IMN|IND|IOT|IRL|IRN|IRQ|ISL|ISR|ITA|JAM|JEY|JOR|JPN|KAZ|KEN|KGZ|KHM|KIR|KNA|KOR|KWT|LAO|LBN|LBR|LBY|LCA|LIE|LKA|LSO|LTU|LUX|LVA|MAC|MAF|MAR|MCO|MDA|MDG|MDV|MEX|MHL|MKD|MLI|MLT|MMR|MNE|MNG|MNP|MOZ|MRT|MSR|MTQ|MUS|MWI|MYS|MYT|NAM|NCL|NER|NFK|NGA|NIC|NIU|NLD|NOR|NPL|NRU|NZL|OMN|PAK|PAN|PCN|PER|PHL|PLW|PNG|POL|PRI|PRK|PRT|PRY|PSE|PYF|QAT|REU|ROU|RUS|RWA|SAU|SDN|SEN|SGP|SGS|SHN|SJM|SLB|SLE|SLV|SMR|SOM|SPM|SRB|SSD|STP|SUR|SVK|SVN|SWE|SWZ|SXM|SYC|SYR|TCA|TCD|TGO|THA|TJK|TKL|TKM|TLS|TON|TTO|TUN|TUR|TUV|TWN|TZA|UGA|UKR|UMI|URY|USA|UZB|VAT|VCT|VEN|VGB|VIR|VNM|VUT|WLF|WSM|YEM|ZAF|ZMB|ZWE|PRT-AC|PRT-MA)$";
        public static string RegexAcronym2Chars => @"^[0-9A-Za-z]{2}$";
        public static string RegexAcronym3Chars => @"^[0-9A-Za-z]{3}$";
        public static string RegexAcronym2Or3Chars => @"^[0-9A-Za-z]{2,3}$";
        public static string RegexIntegerSplittedByComma => @"^(\d+(,\d+)*)?$";
        public static string RegexHardwareVidAndPid => @"^0x[0-9A-Fa-f]{4}$";
        public static string RegexHardwareEndpoint => @"^Ep[0-9]{1,2}$";
        public static string RegexHardwareCodeTable => @"^0x[0-9]{1,2}$";
        public static string RegexHardwarePortName => @"^(COM1|COM2|COM3|COM4|COM5|COM6|COM7|COM8|COM9|COM10|COM11|COM12|COM13|COM14|COM15)$";
        public static string RegexHardwareBaudRate => @"^(300|600|1200|2400|4800|9600|14400|28800|36000|115000)$";
        public static string RegexHardwareParity => @"^(None|Odd|Even|Mark|Space)$";
        public static string RegexHardwareStopBits => @"^(None|One|Two|OnePointFive)$";
        public static string RegexHardwareDataBits => @"^(7|8|9)$";

        public static string RegexCulture = @"^(en-GB|en-US|es-ES|fr-FR|pt-AO|pt-BR|pt-MZ|pt-PT)$";
    }
}
