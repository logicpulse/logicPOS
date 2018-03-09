using logicpos.datalayer.DataLayer.Xpo;
using System;
using System.Diagnostics;
using System.IO;

namespace logicpos.shared.App
{
    public abstract class SettingsApp : logicpos.datalayer.App.SettingsApp
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Developer Mode

#if (DEBUG)
        //Used to Disable Protected Files Check etc
        public static bool DeveloperMode = true;
        //Developer Mode : Use PDF Print
        public static bool PrintPDFEnabled = false;
#else
        public static bool DeveloperMode = false;
        public static bool PrintPDFEnabled = false;
#endif

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Application

        //Overrided by SoftwareVendor Plugin - ex: "LogicPos"
        public static string AppSoftwareName;
        //Overrided by SoftwareVendor Plugin - ex: "LogicPulse"
        public static string AppCompanyName;
        //Overrided by SoftwareVendor Plugin - ex: "+351 233 042 347"
        public static string AppCompanyPhone;
        //Overrided by SoftwareVendor Plugin - ex: "comercial@logicpulse.com"
        public static string AppCompanyEmail;
        //Overrided by SoftwareVendor Plugin - ex: "http://www.logicpulse.com"
        public static string AppCompanyWeb;

        public static string AppSessionFile = "appsession_{0}.json";
        public static bool AppSessionFileJsonIndented = true;

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Override Settings

        public static bool EnablePosSessionApp = false;
        public static bool EnablePosWorkSessionPeriod = false;
        public static bool EnablePosTables = false;

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Country : for Country Finance Rules

        //Defaults if Not Defined in Config
        public static Guid XpoOidConfigurationCountryPortugal = new Guid("e7e8c325-a0d4-4908-b148-508ed750676a");
        public static string XpoOidConfigurationCountryPortugalCode2 = "PT";
        public static Guid XpoOidConfigurationCurrencyEuro = new Guid("28dd2a3a-0083-11e4-96ce-00ff2353398c");
        public static CFG_ConfigurationCurrency ConfigurationSystemCurrency = null;//Assigned on InitPlataformParameters()

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //DateTime Format

        public static string DateFormat = "yyyy-MM-dd";
        public static string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        public static string DateTimeFormatHour = "HH:mm:ss";

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Decimal Format
        public static string DecimalFormat = "0.00";
        public static string DecimalFormatCurrency = "0.00";

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Files/File Formats

        //FileFormat Used in SAF-T(PT)/Backups Etc
        //Overrided by SoftwareVendor Plugin - ex: "yyyyMMdd_HHmmss"
        public static string FileFormatDateTime;
        //filename_countrycode_version_date.extension
        //Overrided by SoftwareVendor Plugin - ex: "saf-t_{0}_{1}_{2}.xml"
        public static string FileFormatSaftPT;

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //DataBase Backup System

        //Overrided by SoftwareVendor Plugin - ex: "hiddenpassword"
        public static string BackupPassword;

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //SAF-T PT

        //SAF-T(PT) : Formats 
        //Overrided by SoftwareVendor Plugin - ex: 6
        public static int DocumentsPadLength;
        //SAF-T(PT) : DateTime Formats 
        //Overrided by SoftwareVendor Plugin - ex: "yyyy-MM-dd"
        //Leave Default Here, in case we dont have Plugin Registered
        public static string DateTimeFormatDocumentDate = "yyyy-MM-dd";
        //Overrided by SoftwareVendor Plugin - ex: "yyyy-MM-ddTHH:mm:ss"
        //Leave Default Here, in case we dont have Plugin Registered
        public static string DateTimeFormatCombinedDateTime = "yyyy-MM-ddTHH:mm:ss";
        //Overrided by SoftwareVendor Plugin - ex: "999999990"
        public static string FinanceFinalConsumerFiscalNumber;
        //Overrided by SoftwareVendor Plugin - ex: "---------"
        public static string FinanceFinalConsumerFiscalNumberDisplay;
        //SAF-T(PT) : Decimal Format
        //Overrided by SoftwareVendor Plugin - ex: "0.00000000"
        public static string DecimalFormatSAFTPT;
        //Overrided by SoftwareVendor Plugin - ex: "0.00"
        public static string DecimalFormatGrossTotalSAFTPT;
        //Used to Compare, Round first, Compare After
        //Overrided by SoftwareVendor Plugin - ex: 2
        public static int DecimalRoundTo;
        //RSA Private Key :Sign Finance Documents used in SHA1SignMessage()
        //Overrided by SoftwareVendor Plugin - ex: 
        //@"<RSAKeyValue>
        //    <Modulus>PLACE VALUE HERE</Modulus>
        //    <Exponent>PLACE VALUE HERE</Exponent>
        //    <P>PLACE VALUE HERE</P>
        //    <Q>PLACE VALUE HERE</Q>
        //    <DP>PLACE VALUE HERE</DP>
        //    <DQ>PLACE VALUE HERE</DQ>
        //    <InverseQ>PLACE VALUE HERE</InverseQ>
        //    <D>PLACE VALUE HERE</D>
        //</RSAKeyValue>"
        public static string RsaPrivateKey;
        //SAFT-T XML Export Header
        public static string SaftProductID { get { return GetSaftProductID(); } }
        //Overrided by SoftwareVendor Plugin - ex: "000000000" : Your Company FiscalNumber;
        public static string SaftProductCompanyTaxID;
        //Overrided by SoftwareVendor Plugin - ex: "0000" : Your Company CertificateNumber;
        public static string SaftSoftwareCertificateNumber;
        //Overrided by SoftwareVendor Plugin - ex: "PT"
        public static string SaftVersionPrefix;
        //Overrided by SoftwareVendor Plugin - ex: "1.04_01"
        public static string SaftVersion;
        //Versão da Chave Privada utilizada na criação da Assinatura 
        //Overrided by SoftwareVendor Plugin - ex: 1
        public static int HashControl;
        //C — Contabilidade;
        //E — Faturação emitida por terceiros;
        //F — Faturação;
        //I — Contabilidade integrada com a faturação;
        //P — Faturação parcial;
        //R — Recibos (a);
        //S — Autofaturação;
        //T — Documentos de transporte (a).	
        //Overrided by SoftwareVendor Plugin - ex: "F"
        public static string TaxAccountingBasis;
        //Currency Code
        //Overrided by SoftwareVendor Plugin - ex: "EUR"
        public static string SaftCurrencyCode;

        //SAFT(PT) : Country Rules
        //Retalhistas e vendedores ambulantes é permitida a emissão de faturas simplificadas a não sujeitos passivos, 
        //Até limite de 1000,00€ e a todas as outras atividades é apenas permitida a emissão de faturas até aos 100,00€
        public static int FinanceRuleSimplifiedInvoiceMaxTotal { get { return GetFinanceRuleSimplifiedInvoiceMaxTotal(); } }
        //Services
        public static int FinanceRuleSimplifiedInvoiceMaxTotalServices { get { return GetFinanceRuleSimplifiedInvoiceMaxTotalServices(); } }
        //This rule is to force fill Customer details if total document value is Greater or Equal to
        public static int FinanceRuleRequiredCustomerDetailsAboveValue { get { return GetFinanceRuleRequiredCustomerDetailsAboveValue(); } }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Series

        //Use Alphanumeric Random|Or Numeric Sequential 
        //Overrided by SoftwareVendor Plugin - ex: false
        public static bool DocumentFinanceSeriesGenerationFactoryUseRandomAcronymPrefix;
        //Overrided by SoftwareVendor Plugin - ex: "000"
        public static string DocumentFinanceSeriesGenerationFactoryAcronymLastSerieFormat;

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //RegEx

        //RegEx : http://regex101.com | http://regexr.com
        public static string RegexAlfa = @"^[A-Za-záéíóúÁÉÍÓÚàèìòùÀÈÌÒÙãõñÃÕÑâêîôûÂÊÎÔÛçÇ \s]*$";
        public static string RegexAlfaNumeric = @"^[0-9A-Za-zéúíóáÉÚÍÓÁèùìòàÈÙÌÒÀõãñÕÃÑêûîôâÊÛÎÔÂçÇ\-_ \s]*$";
        public static string RegexAlfaNumericExtended = @"^[0-9A-Za-zéúíóáÉÚÍÓÁèùìòàÈÙÌÒÀõãñÕÃÑêûîôâÊÛÎÔÂçÇ€$&@#%ºª()\[\]';:!?<>+\-_ \.\,\s\\\/\*]*$";
        public static string RegexAlfaNumericFilePath = @"^[0-9A-Za-z\-\\\.:_\/ \s]*$";
        public static string RegexInteger = @"^\d+$";
        public static string RegexIntegerGreaterThanZero = @"^[1-9][0-9]*$";
        public static string RegexIntegerGreaterEqualThanZero = @"^[0-9]*$";
        public static string RegexIntegerColonSeparated = @"^(\d+(:\d+)*)?$";
        //Used to detect Decimal Input Entrys in BackOffice XPOWidgets : , or . Culture Related
        public static string RegexDecimal = @"^\s*(?=.*[0-9])\d*(?:[\.,]\d{1,4})?\s*$";
        public static string RegexDecimalPositiveAndNegative = @"^-?\s*(?=.*[1-9])\d*(?:[\.,]\d{1,})?\s*$";//d{1,4}
        public static string RegexDecimalGreaterThanZero = @"^\s*(?=.*[1-9])\d*(?:[\.,]\d{1,})?\s*$";//d{1,4}
        public static string RegexDecimalGreaterEqualThanZero = @"^\s*(?=.*[0-9])\d*(?:[\.,]\d{1,})?\s*$";//d{1,4}
        //http://www.nationwidebarcode.com/upc-country-codes/ 
        public static string RegexEan12andEan4 = @"^\d{12,14}$|^560\d{12,14}$";// <EAN12 a 14 any COUNTRY (^560\d{9}$|^560\d{11}$ < EAN11 PT, ^600\d{9}$|^560\d{11}$ < EAN11 AO)
        public static string RegexEmail = @"^[_a-z0-9-]+(\.[_a-z0-9-]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4})$";
        public static string RegexGuid = @"^\b[a-fA-F0-9]{8}(?:-[a-fA-F0-9]{4}){3}-[a-fA-F0-9]{12}\b$";
        //This will match valid dates in the format DD/MM/YYYY. It takes leap years into account when matching feb 29th and covers from 01/01/0000 to 31/12/9999
        //public static string RegexDate = @"^(((0[1-9]|[12][0-9]|3[01])[- /\\.](0[13578]|1[02])|(0[1-9]|[12][0-9]|30)[- /\\.](0[469]|11)|(0[1-9]|1\d|2[0-8])[- /\\.]02)[- /\\.]\d{4}|29[- /\\.]02[- /\\.](\d{2}(0[48]|[2468][048]|[13579][26])|([02468][048]|[1359][26])00))$";
        //Custom YYYY/MM/DD Leap Years
        public static string RegexDate = @"^((([0-9][0-9][0-9][1-9])|([1-9][0-9][0-9][0-9])|([0-9][1-9][0-9][0-9])|([0-9][0-9][1-9][0-9]))[-\/\\.]((0[13578])|(1[02]))[-\/\\.]((0[1-9])|([12][0-9])|(3[01])))|((([0-9][0-9][0-9][1-9])|([1-9][0-9][0-9][0-9])|([0-9][1-9][0-9][0-9])|([0-9][0-9][1-9][0-9]))-((0[469])|11)-((0[1-9])|([12][0-9])|(30)))|(((000[48])|([0-9]0-9)|([0-9][1-9][02468][048])|([1-9][0-9][02468][048])|([0-9]0-9)|([0-9][1-9][13579][26])|([1-9][0-9][13579][26]))-02-((0[1-9])|([12][0-9])))|((([0-9][0-9][0-9][1-9])|([1-9][0-9][0-9][0-9])|([0-9][1-9][0-9][0-9])|([0-9][0-9][1-9][0-9]))-02-((0[1-9])|([1][0-9])|([2][0-8])))$";
        public static string RegexTime = @"^(0[0-9]|1[0-9]|2[0-3])(:[0-5]\d)(:[0-5]\d)$";
        //public static string RegexDateTime = @"^(((\d\d)(([02468][048])|([13579][26]))-02-29)|(((\d\d)(\d\d)))-((((0\d)|(1[0-2]))-((0\d)|(1\d)|(2[0-8])))|((((0[13578])|(1[02]))-31)|(((0[1,3-9])|(1[0-2]))-(29|30)))))\s(([01]\d|2[0-3]):([0-5]\d):([0-5]\d))$";
        //Custom YYYY/MM/DD 23:11:28 With Leap Years
        public static string RegexDateTime = @"^(((\d\d)(([02468][048])|([13579][26]))[-\/\\.]02[-\/\\.]29)|(((\d\d)(\d\d)))[-\/\\.]((((0\d)|(1[0-2]))[-\/\\.]((0\d)|(1\d)|(2[0-8])))|((((0[13578])|(1[02]))-31)|(((0[1,3-9])|(1[0-2]))-(29|30)))))\s(([01]\d|2[0-3]):([0-5]\d):([0-5]\d))$";
        //Year|Month|Day Splitted
        public static string RegexDateYear = @"^((19|20)\d\d)$";
        public static string RegexDateYearHolidays = @"^((19|20)\d\d)|0$";//Includes 0 for Holidays
        public static string RegexDateMonth = @"^(0?[1-9]|1[012])$";
        public static string RegexDateDay = @"^(0?[1-9]|[12][0-9]|3[01])$";
        //Percentage
        public static string RegexPercentage = @"^(100([\.\,][0]{1,})?$|[0-9]{1,2}([\.\,][0-9]{1,})?)$";
        //Minimal 4 Chars
        public static string RegexLoginPin = @"^[0-9]{4,}$";
        //PrintCopies
        public static string RegexPrintCopies = @"^[1-4]$";
        //Minimal 8 Chars, Require one lower letter, one upper letter, and one number
        //public static string RegexLoginPassword = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[^ ]{8,}$";
        //^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&'])[^ ]{8,}$
        //Document Series Acronym
        public static string RegexDocumentSeriesAcronym = @"^[0-9A-Za-z]{4,8}$";
        public static string RegexIPv4 = @"^(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5]){3}$";
        //BackOffice Specific Fields
        public static string RegexAlfaNumericArticleCode = @"^[0-9A-Za-z€$&@#%()\[\]+\-_ \.\\\/]{2,25}$";
        public static string RegexAlfaNumericArticleButtonLabel = @"^[0-9A-Za-zéúíóáÉÚÍÓÁèùìòàÈÙÌÒÀõãñÕÃÑêûîôâÊÛÎÔÂçÇ€$&@#%ºª()\[\]';:!?<>+\-_ \.\,\s\\\/\*]{2,35}$";
        //ArticleClass Acronym 1 Char
        public static string RegexAlfaAcronym1Char = @"^[A-Za-záéíóúÁÉÍÓÚàèìòùÀÈÌÒÙãõñÃÕÑâêîôûÂÊÎÔÛçÇ \s]{1}$";
        //Country Codes - ISO 3166-1 alpha-2 - Incluedes -AC e -MA (Açores e Madeira)
        public static string RegexAlfaCountryCode2 = @"^(AF|AX|AL|DZ|AS|AD|AO|AI|AQ|AG|AR|AM|AW|AU|AT|AZ|BS|BH|BD|BB|BY|BE|BZ|BJ|BM|BT|BO|BQ|BA|BW|BV|BR|IO|BN|BG|BF|BI|KH|CM|CA|CV|KY|CF|TD|CL|CN|CX|CC|CO|KM|CG|CD|CK|CR|CI|HR|CU|CW|CY|CZ|DK|DJ|DM|DO|EC|EG|SV|GQ|ER|EE|ET|FK|FO|FJ|FI|FR|GF|PF|TF|GA|GM|GE|DE|GH|GI|GR|GL|GD|GP|GU|GT|GG|GN|GW|GY|HT|HM|VA|HN|HK|HU|IS|IN|ID|IR|IQ|IE|IM|IL|IT|JM|JP|JE|JO|KZ|KE|KI|KP|KR|KW|KG|LA|LV|LB|LS|LR|LY|LI|LT|LU|MO|MK|MG|MW|MY|MV|ML|MT|MH|MQ|MR|MU|YT|MX|FM|MD|MC|MN|ME|MS|MA|MZ|MM|NA|NR|NP|NL|NC|NZ|NI|NE|NG|NU|NF|MP|NO|OM|PK|PW|PS|PA|PG|PY|PE|PH|PN|PL|PT|PR|QA|RE|RO|RU|RW|BL|SH|KN|LC|MF|PM|VC|WS|SM|ST|SA|SN|RS|SC|SL|SG|SX|SK|SI|SB|SO|ZA|GS|SS|ES|LK|SD|SR|SJ|SZ|SE|CH|SY|TW|TJ|TZ|TH|TL|TG|TK|TO|TT|TN|TR|TM|TC|TV|UG|UA|AE|GB|US|UM|UY|UZ|VU|VE|VN|VG|VI|WF|EH|YE|ZM|ZW|PT-AC|PT-MA)$";
        public static string RegexAlfaCountryCode3 = @"^(ABW|AFG|AGO|AIA|ALA|ALB|AND|ARE|ARG|ARM|ASM|ATA|ATF|ATG|AUS|AUT|AZE|BDI|BEL|BEN|BES|BFA|BGD|BGR|BHR|BHS|BIH|BLM|BLR|BLZ|BMU|BOL|BRA|BRB|BRN|BTN|BVT|BWA|CAF|CAN|CCK|CHE|CHL|CHN|CIV|CMR|COD|COG|COK|COL|COM|CPV|CRI|CUB|CUW|CXR|CYM|CYP|CZE|DEU|DJI|DMA|DNK|DOM|DZA|ECU|EGY|ERI|ESH|ESP|EST|ETH|FIN|FJI|FLK|FRA|FRO|FSM|GAB|GBR|GEO|GGY|GHA|GIB|GIN|GLP|GMB|GNB|GNQ|GRC|GRD|GRL|GTM|GUF|GUM|GUY|HKG|HMD|HND|HRV|HTI|HUN|IDN|IMN|IND|IOT|IRL|IRN|IRQ|ISL|ISR|ITA|JAM|JEY|JOR|JPN|KAZ|KEN|KGZ|KHM|KIR|KNA|KOR|KWT|LAO|LBN|LBR|LBY|LCA|LIE|LKA|LSO|LTU|LUX|LVA|MAC|MAF|MAR|MCO|MDA|MDG|MDV|MEX|MHL|MKD|MLI|MLT|MMR|MNE|MNG|MNP|MOZ|MRT|MSR|MTQ|MUS|MWI|MYS|MYT|NAM|NCL|NER|NFK|NGA|NIC|NIU|NLD|NOR|NPL|NRU|NZL|OMN|PAK|PAN|PCN|PER|PHL|PLW|PNG|POL|PRI|PRK|PRT|PRY|PSE|PYF|QAT|REU|ROU|RUS|RWA|SAU|SDN|SEN|SGP|SGS|SHN|SJM|SLB|SLE|SLV|SMR|SOM|SPM|SRB|SSD|STP|SUR|SVK|SVN|SWE|SWZ|SXM|SYC|SYR|TCA|TCD|TGO|THA|TJK|TKL|TKM|TLS|TON|TTO|TUN|TUR|TUV|TWN|TZA|UGA|UKR|UMI|URY|USA|UZB|VAT|VCT|VEN|VGB|VIR|VNM|VUT|WLF|WSM|YEM|ZAF|ZMB|ZWE|PRT-AC|PRT-MA)$";
        //Acronyms
        public static string RegexAcronym2Chars = @"^[0-9A-Za-z]{2}$";
        public static string RegexAcronym3Chars = @"^[0-9A-Za-z]{3}$";
        public static string RegexAcronym2Or3Chars = @"^[0-9A-Za-z]{2,3}$";

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Override From Config : Replaced by Country RegexFiscalNumber & RegexZipCode

        //Used only in DialogUserDetail, else are used from regexFiscalNumber from Country Object
        //public static string RegexFiscalNumber = (GlobalFramework.Settings["RegexFiscalNumber"] == String.Empty) ? @"^(PT)?[0-9]{9}$" : GlobalFramework.Settings["RegexFiscalNumber"];
        //Move to Country Table
        //public static string RegexZipCode = (GlobalFramework.Settings["RegexZipCode"] == String.Empty) ? @"^\d{4}(-\d{3})?$" : GlobalFramework.Settings["RegexZipCode"];

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Developer Config

        // Undefined Record
        public static Guid XpoOidUndefinedRecord = new Guid("00000000-0000-0000-0000-000000000001");

        //ArticleClass
        public static Guid XpoOidArticleClassCustomerCard = new Guid("49ea35ba-35f3-440f-946e-ab32578ed741");

        //Customer | Final Consumer | 999999990
        public static Guid XpoOidDocumentFinanceMasterFinalConsumerEntity = new Guid("0cf40622-578b-417d-b50f-e945fefb5d68");

        //ConfigurationPaymentMethod
        public static Guid XpoOidConfigurationPaymentMethodCurrentAccount = new Guid("6db009fd-6729-4353-a4d0-d599c4c19297");

        //ConfigurationPaymentCondition
        public static Guid XpoOidConfigurationPaymentMethodInstantPayment = new Guid("4261daa6-c0bd-4ac9-949a-cae0be2dd472");//Pronto Pagamento

        //DocumentFinanceType

        //SaftDocumentType 1
        public static Guid XpoOidDocumentFinanceTypeInvoice = new Guid("7af04618-74a6-42a3-aaba-454b7076f5a6");
        public static Guid XpoOidDocumentFinanceTypeSimplifiedInvoice = new Guid("2c69b109-318a-4375-a573-28e5984b6503");
        public static Guid XpoOidDocumentFinanceTypeInvoiceAndPayment = new Guid("09b6aa6e-dc0e-41fd-8dbe-8678a3d11cbc");
        public static Guid XpoOidDocumentFinanceTypeDebitNote = new Guid("3942d940-ed13-4a62-a352-97f1ce006d8a");
        public static Guid XpoOidDocumentFinanceTypeCreditNote = new Guid("fa924162-beed-4f2f-938d-919deafb7d47");
        //SaftDocumentType 2
        public static Guid XpoOidDocumentFinanceTypeDeliveryNote = new Guid("95f6a472-1b12-43aa-a215-a4b406b18924");
        public static Guid XpoOidDocumentFinanceTypeTransportationGuide = new Guid("96bcf534-0dab-48bb-a69e-166e81ae6f7b");
        public static Guid XpoOidDocumentFinanceTypeOwnAssetsDriveGuide = new Guid("f8e96786-fed8-4143-be9e-b03c3a984a2c");
        public static Guid XpoOidDocumentFinanceTypeConsignmentGuide = new Guid("63d8eb04-983c-4524-96de-979a240b362c");
        public static Guid XpoOidDocumentFinanceTypeReturnGuide = new Guid("f03d2788-bed6-41ab-8d44-100039103e83");
        //SaftDocumentType 3
        public static Guid XpoOidDocumentFinanceTypeConferenceDocument = new Guid("afed98d3-eae7-43a7-a7be-515753594c8f");
        public static Guid XpoOidDocumentFinanceTypeConsignationInvoice = new Guid("b8554d36-642a-4083-b608-8f1da35f0fec");
        public static Guid XpoOidDocumentFinanceTypeBudget = new Guid("005ac531-31a1-44bb-9346-058f9c9ad01a");
        public static Guid XpoOidDocumentFinanceTypeProformaInvoice = new Guid("6f4249d0-4aaf-4711-814f-7f9533a1ef7f");
        //SaftDocumentType 4
        public static Guid XpoOidDocumentFinanceTypePayment = new Guid("a009168d-fed1-4f52-b9e3-77e280b18ff5");
        //SaftDocumentType 0 : INTERNAL, not used in SAF-T PT
        public static Guid XpoOidDocumentFinanceTypeInvoiceWayBill = new Guid("f8878cf5-0f88-4270-8a55-1fc2488d81a2");
        public static Guid XpoOidDocumentFinanceTypeCurrentAccountInput = new Guid("235f06f3-5ec3-4e13-977b-325614b07e35");

        //Printers
        //Generic Printer, used in NewDocuments, to choose printer Target
        public static Guid XpoOidConfigurationPrinterGeneric = new Guid("7b7243d2-5009-4ac8-a96b-2d0d92aceea8");
        //Thermal Printer : Used in ThermalPrinterGeneric Class (Enum)
        public static Guid XpoOidConfigurationPrinterThermalWindows = new Guid("e7143ea5-391b-46ef-a28d-4843fd7e21ac");
        public static Guid XpoOidConfigurationPrinterThermalLinux = new Guid("3fab75ce-e81f-4488-b0a0-962c9336f3bb");
        public static Guid XpoOidConfigurationPrinterThermalSocket = new Guid("faeb45cd-2989-4e92-9907-3038444e4849");

        //PrintersType
        //THERMAL_PRINTER_WINDOWS
        public static Guid XpoOidConfigurationPrinterTypeThermalPrinterWindows = new Guid("e7143ea5-391b-46ef-a28d-4843fd7e21ac");
        //THERMAL_PRINTER_LINUX
        public static Guid XpoOidConfigurationPrinterTypeThermalPrinterLinux = new Guid("3fab75ce-e81f-4488-b0a0-962c9336f3bb");
        //THERMAL_PRINTER_SOCKET
        public static Guid XpoOidConfigurationPrinterTypeThermalPrinterSocket = new Guid("faeb45cd-2989-4e92-9907-3038444e4849");
        //GENERIC_PRINTER_WINDOWS
        public static Guid XpoOidConfigurationPrinterTypeGenericWindows = new Guid("4be662e6-67c9-4063-bd6c-574ae4df7f3f");
        //GENERIC_PRINTER_LINUX
        public static Guid XpoOidConfigurationPrinterTypeGenericLinux = new Guid("3d38a3c3-667b-4c0c-b487-f18ddd3c8a4a");
        //REPORT_EXPORT_PDF
        public static Guid XpoOidConfigurationPrinterTypeExportPdf = new Guid("e5e20cd0-d9d2-443d-9d3f-3478949db30f");

        //Modelo para impressão de Fecho de Dia/Caixa
        public static Guid XpoOidConfigurationPrintersTemplateWorkSessionMovement = new Guid("f6a25476-40b0-4287-9284-d5db3280d7f1");
        //Modelo para impressão de Entradas/Saidas de Numerário
        public static Guid XpoOidConfigurationPrintersTemplateCashDrawerOpenAndMoneyInOut = new Guid("f6565476-28b0-4287-9284-d5db3280d421");

        //Tax/VatRate
        public static Guid XpoOidConfigurationVatRateDutyFree = new Guid("e74faad7-f5c9-4206-a662-f95820014195");//0
        //(Continente)
        public static Guid XpoOidConfigurationVatRateNormalPT = new Guid("cee00590-7317-41b8-af46-66560401096b");//23
        public static Guid XpoOidConfigurationVatRateIntermediatePT = new Guid("f73e3b41-4967-48c6-9f9a-260abf2146e1");//13
        public static Guid XpoOidConfigurationVatRateReducedPT = new Guid("7e89eaed-ce56-4565-8eec-98f2e8d004a5");//6
        //(R.A.Madeira)
        public static Guid XpoOidConfigurationVatRateNormalPTMA = new Guid("ecd64d02-5249-4303-a35c-c662ffba4844");//22
        public static Guid XpoOidConfigurationVatRateIntermediatePTMA = new Guid("f0281b91-83d7-482f-bfd8-e52461983136");//12
        public static Guid XpoOidConfigurationVatRateReducedPTMA = new Guid("b57d85a5-843e-4b84-9660-9124006b9b05");//5
        //(R.A.Açores)
        public static Guid XpoOidConfigurationVatRateNormalPTAC = new Guid("389661c1-05f6-4830-bc06-176e2fdb3dc2");//18
        public static Guid XpoOidConfigurationVatRateIntermediatePTAC = new Guid("52c6ce3c-9246-4b8b-a143-b84733a074d4");//9
        public static Guid XpoOidConfigurationVatRateReducedPTAC = new Guid("e4478dea-9272-4090-a71a-df775b96c4b3");//4

        //VatExemptionReason
        public static Guid XpoOidConfigurationVatExemptionReasonM99 = new Guid("f60f97c0-390e-4d76-90d7-204b6ea57949");

        //Notifications
        //Notification Types
        public static Guid XpoOidSystemNotificationTypeNewTerminalRegistered = new Guid("bc1f6a82-fa8e-49c8-981c-46ff21aef8b4");
        public static Guid XpoOidSystemNotificationTypeCurrentAccountDocumentsToInvoice = new Guid("06319d46-e7b5-4cca-8257-55eff4cfe0fa");
        public static Guid XpoOidSystemNotificationTypeConsignationInvoiceDocumentsToInvoice = new Guid("a567578b-53e9-4b5c-848f-183c65194971");
        public static Guid XpoOidSystemNotificationTypeSaftDocumentTypeMovementOfGoods = new Guid("80a03838-0937-4ae3-921f-75a1e358f7bf");

        //ConfigurationPriceType
        //Retail Mode|New Document to get PriceType when customer is not Defined
        public static Guid XpoOidConfigurationPriceTypeDefault = new Guid("cf17a218-b687-4b82-a8f4-0905594ac1f5");

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Thermal Printer Ticket

        public static string PrinterThermalEncoding = GlobalFramework.Settings["PrinterThermalEncoding"];
        public static bool PrinterThermalPrintLogo = Convert.ToBoolean(GlobalFramework.Settings["PrinterThermalPrintLogo"]);
        public static string PrinterThermalImageCompanyLogo = GlobalFramework.Settings["PrinterThermalImageCompanyLogo"];
        public static int PrinterThermalMaxCharsPerLineNormal = Convert.ToInt16(GlobalFramework.Settings["PrinterThermalMaxCharsPerLineNormal"]);
        public static int PrinterThermalMaxCharsPerLineNormalBold = Convert.ToInt16(GlobalFramework.Settings["PrinterThermalMaxCharsPerLineNormalBold"]);
        public static int PrinterThermalMaxCharsPerLineSmall = Convert.ToInt16(GlobalFramework.Settings["PrinterThermalMaxCharsPerLineSmall"]);

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Reports

        //Used to Enable/Disable DoublePass, else Blank Page when DoublePass Enabled with One Page
        //used in ProcessReportFinanceDocument.customReport.DoublePass
        //When change value here, change script block in .frx too (_dataBandMaxRecs = 15;)
        public static uint CustomReportReportDocumentFinanceMaxDetail = 15;

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Helper Methods

        /// <summary>
        /// Assign OneTime AppSettings from SoftwareVendorPluginSettings
        /// </summary>
        public static void InitSoftwareVendorPluginSettings()
        {
            bool debug = false;

            // Override SettingsApp with Defaults from SoftwareVendor Plugin
            AppSoftwareName = FrameworkUtils.GetSoftwareVendorValueAsString(nameof(AppSoftwareName), debug);
            AppCompanyName = FrameworkUtils.GetSoftwareVendorValueAsString(nameof(AppCompanyName), debug);
            AppCompanyPhone = FrameworkUtils.GetSoftwareVendorValueAsString(nameof(AppCompanyPhone), debug);
            AppCompanyEmail = FrameworkUtils.GetSoftwareVendorValueAsString(nameof(AppCompanyEmail), debug);
            AppCompanyWeb = FrameworkUtils.GetSoftwareVendorValueAsString(nameof(AppCompanyWeb), debug);

            FileFormatDateTime = FrameworkUtils.GetSoftwareVendorValueAsString(nameof(FileFormatDateTime), debug);
            FileFormatSaftPT = FrameworkUtils.GetSoftwareVendorValueAsString(nameof(FileFormatSaftPT), debug);

            DocumentsPadLength = FrameworkUtils.GetSoftwareVendorValueAsInt(nameof(DocumentsPadLength), debug);
            DateTimeFormatDocumentDate = FrameworkUtils.GetSoftwareVendorValueAsString(nameof(DateTimeFormatDocumentDate), debug);
            DateTimeFormatCombinedDateTime = FrameworkUtils.GetSoftwareVendorValueAsString(nameof(DateTimeFormatCombinedDateTime), debug);
            FinanceFinalConsumerFiscalNumber = FrameworkUtils.GetSoftwareVendorValueAsString(nameof(FinanceFinalConsumerFiscalNumber), debug);
            FinanceFinalConsumerFiscalNumberDisplay = FrameworkUtils.GetSoftwareVendorValueAsString(nameof(FinanceFinalConsumerFiscalNumberDisplay), debug);
            DecimalFormatSAFTPT = FrameworkUtils.GetSoftwareVendorValueAsString(nameof(DecimalFormatSAFTPT), debug);
            DecimalFormatGrossTotalSAFTPT = FrameworkUtils.GetSoftwareVendorValueAsString(nameof(DecimalFormatGrossTotalSAFTPT), debug);
            DecimalRoundTo = FrameworkUtils.GetSoftwareVendorValueAsInt(nameof(DecimalRoundTo), debug);
            SaftProductCompanyTaxID = FrameworkUtils.GetSoftwareVendorValueAsString(nameof(SaftProductCompanyTaxID), debug);
            SaftSoftwareCertificateNumber = FrameworkUtils.GetSoftwareVendorValueAsString(nameof(SaftSoftwareCertificateNumber), debug);
            SaftVersionPrefix = FrameworkUtils.GetSoftwareVendorValueAsString(nameof(SaftVersionPrefix), debug);
            SaftVersion = FrameworkUtils.GetSoftwareVendorValueAsString(nameof(SaftVersion), debug);
            HashControl = FrameworkUtils.GetSoftwareVendorValueAsInt(nameof(HashControl), debug);
            TaxAccountingBasis = FrameworkUtils.GetSoftwareVendorValueAsString(nameof(TaxAccountingBasis), debug);
            SaftCurrencyCode = FrameworkUtils.GetSoftwareVendorValueAsString(nameof(SaftCurrencyCode), debug);

            DocumentFinanceSeriesGenerationFactoryUseRandomAcronymPrefix = FrameworkUtils.GetSoftwareVendorValueAsBool(nameof(DocumentFinanceSeriesGenerationFactoryUseRandomAcronymPrefix), debug);
            DocumentFinanceSeriesGenerationFactoryAcronymLastSerieFormat = FrameworkUtils.GetSoftwareVendorValueAsString(nameof(DocumentFinanceSeriesGenerationFactoryAcronymLastSerieFormat), debug);
        }

        private static string GetSaftProductID()
        {
            return string.Format("{0}/{1}", AppSoftwareName, AppCompanyName);
        }

        //Use to limit Simplified Invoices to have a total limit of 1000 (Articles Products + Services)
        private static int GetFinanceRuleSimplifiedInvoiceMaxTotal()
        {
            int result = 0;
            bool byPassValue = false;

            //PT : Override Defaults
            if (SettingsApp.ConfigurationSystemCountry.Oid == XpoOidConfigurationCountryPortugal)
            {
                if (Debugger.IsAttached && byPassValue)
                {
                    result = 999999999;
                }
                else
                {
                    result = (GlobalFramework.PluginSoftwareVendor != null)
                        // From Vendor Plugin
                        ? GlobalFramework.PluginSoftwareVendor.GetFinanceRuleSimplifiedInvoiceMaxTotal()
                        // Default
                        : 1000;
                }
            }
            //Defaults for all Other Countrys
            else
            {
                result = 999999999;
            }

            return result;
        }

        //Use to limit Simplified Invoices to have a total limit of 100 (Articles Services)
        private static int GetFinanceRuleSimplifiedInvoiceMaxTotalServices()
        {
            int result = 0;
            bool byPassValue = false;

            //PT : Override Defaults
            if (SettingsApp.ConfigurationSystemCountry.Oid == XpoOidConfigurationCountryPortugal)
            {
                if (Debugger.IsAttached && byPassValue)
                {
                    result = 999999999;
                }
                else
                {
                    result = (GlobalFramework.PluginSoftwareVendor != null)
                        // From Vendor Plugin
                        ? GlobalFramework.PluginSoftwareVendor.GetFinanceRuleSimplifiedInvoiceMaxTotalServices()
                        // Default
                        : 100;
                    ;
                }
            }
            //Defaults for all Other Countrys
            else
            {
                result = 999999999;
            }

            return result;
        }

        private static int GetFinanceRuleRequiredCustomerDetailsAboveValue()
        {
            int result = 0;

            //PT : Override Defaults
            if (SettingsApp.ConfigurationSystemCountry.Oid == XpoOidConfigurationCountryPortugal)
            {
                result = (GlobalFramework.PluginSoftwareVendor != null)
                    // From Vendor Plugin
                    ? GlobalFramework.PluginSoftwareVendor.GetFinanceRuleRequiredCustomerDetailsAboveValue()
                    // Default
                    : 1000;
                ;
            }
            //Defaults for all Other Countrys
            else
            {
                result = 999999999;
            }

            return result;
        }
    }
}
