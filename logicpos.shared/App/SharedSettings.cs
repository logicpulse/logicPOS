using logicpos.datalayer.App;
using logicpos.datalayer.DataLayer.Xpo;
using System;
using System.Diagnostics;

namespace logicpos.shared.App
{
    public static class SharedSettings 
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

#if DEBUG
        public static bool DeveloperMode { get; set; } = true;
        public static bool PrintPDFEnabled { get; set; } = false;
#else
        public static bool DeveloperMode {get;set;} = false;
        public static bool PrintPDFEnabled {get;set} = false;
#endif

        public static string AppSoftwareName { get; set; }
        public static string AppCompanyName { get; set; }
        public static string AppCompanyPhone { get; set; }
        public static string AppCompanyEmail { get; set; }
        public static string AppCompanyWeb { get; set; }
        public static string AppSoftwareVersionFormat { get; set; }
        public static string AppSessionFile { get; set; } = "appsession_{0}.json";
        public static bool AppSessionFileJsonIndented { get; set; } = true;

        public static Guid XpoOidConfigurationCountryPortugal { get; set; } = new Guid("e7e8c325-a0d4-4908-b148-508ed750676a");
        public static Guid XpoOidConfigurationCurrencyEuro { get; set; } = new Guid("28dd2a3a-0083-11e4-96ce-00ff2353398c");
        public static cfg_configurationcurrency ConfigurationSystemCurrency { get; set; } = null;

        public static Guid XpoOidConfigurationCountryMozambique { get; set; } = new Guid("16fcd7f2-e885-48d8-9f8e-9d224cc36f32");
        public static Guid XpoOidConfigurationCurrencyUSDollar { get; set; } = new Guid("28d692ad-0083-11e4-96ce-00ff2353398c");

        public static Guid XpoOidConfigurationCountryAngola { get; set; } = new Guid("9655510a-ff58-461e-9719-c037058f10ed");

        public static string DateFormat { get; set; } = "yyyy-MM-dd";
        public static string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";
        public static string DateTimeFormatHour { get; set; } = "HH:mm:ss";

        public static string DecimalFormat { get; set; } = "0.00";
        public static string DecimalFormatStockQuantity { get; set; } = "0.00000000";

        public static string FileFormatDateTime { get; set; }
        public static string FileFormatSaftPT { get; set; }
        public static string FileFormatSaftAO { get; set; }

        public static int DocumentsPadLength { get; set; }
        public static string DateTimeFormatDocumentDate { get; set; } = "yyyy-MM-dd";
        public static string DateTimeFormatCombinedDateTime { get; set; } = "yyyy-MM-ddTHH:mm:ss";
        public static string FinanceFinalConsumerFiscalNumber { get; set; }
        public static string FinanceFinalConsumerFiscalNumberDisplay { get; set; }
        public static string DecimalFormatSAFTPT { get; set; }
        public static string DecimalFormatSAFTAO { get; private set; }
        public static string DecimalFormatGrossTotalSAFTPT { get; set; }
        public static int DecimalRoundTo { get; set; }
        public static string SaftProductID { get { return GetSaftProductID(); } }
        public static string SaftProductCompanyTaxID { get; set; }
        public static string SaftSoftwareCertificateNumber { get; set; }
        public static string SaftVersionPrefix { get; set; }
        public static string SaftVersion { get; set; }
        public static int HashControl { get; set; }
        public static string TaxAccountingBasis { get; set; }

        public static string SaftCurrencyCode { get; set; }
        public static string SaftCurrencyCodeAO { get; private set; }

        public static int FinanceRuleSimplifiedInvoiceMaxTotal { get { return GetFinanceRuleSimplifiedInvoiceMaxTotal(); } }
        //Services
        public static int FinanceRuleSimplifiedInvoiceMaxTotalServices { get { return GetFinanceRuleSimplifiedInvoiceMaxTotalServices(); } }
        //This rule is to force fill Customer details if total document value is Greater or Equal to
        public static int FinanceRuleRequiredCustomerDetailsAboveValue { get { return GetFinanceRuleRequiredCustomerDetailsAboveValue(); } }

        //SAFT-T XML Export Header
        public static string SaftProductIDAO { get { return GetSaftProductID_AO(); } }

        //Overrided by SoftwareVendor Plugin - ex: "0000" : Your Company CertificateNumber;
        public static string SaftSoftwareCertificateNumberAO { get; set; }
        //Overrided by SoftwareVendor Plugin - ex: "PT"
        public static string SaftVersionPrefixAO { get; set; }
        //Overrided by SoftwareVendor Plugin - ex: "1.04_01"
        public static string SaftVersionAO { get; set; }

        public static bool DocumentFinanceSeriesGenerationFactoryUseRandomAcronymPrefix { get; set; }

        public static string DocumentFinanceSeriesGenerationFactoryAcronymLastSerieFormat { get; set; }
        
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Developer Config

        // Undefined Record
        public static Guid XpoOidUndefinedRecord = new Guid("00000000-0000-0000-0000-000000000001");
        public static Guid XpoOidUserRecord = new Guid("00000000-0000-0000-0000-000000000002");
        public static string XpoOidHiddenRecordsFilter = "00000000-0000-0000-0000-000000000%";

        //ArticleClass
        public static Guid XpoOidArticleClassCustomerCard = new Guid("49ea35ba-35f3-440f-946e-ab32578ed741");

        //Customer | Final Consumer | 999999990
        public static Guid FinalConsumerId { get; set; } = new Guid("0cf40622-578b-417d-b50f-e945fefb5d68");

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
        public static Guid XpoOidConfigurationPrinterGeneric = new Guid("b0c917c7-2ea1-4e08-afa5-4744c19e1c5c");
        //Thermal Printer : Used in ThermalPrinterGeneric Class (Enum)
        public static Guid XpoOidConfigurationPrinterThermalWindows = new Guid("e7143ea5-391b-46ef-a28d-4843fd7e21ac");
        public static Guid XpoOidConfigurationPrinterThermalSocket = new Guid("faeb45cd-2989-4e92-9907-3038444e4849");
        public static Guid XpoOidConfigurationPrinterThermalUsb = new Guid("39b58a4e-b860-49c1-81a5-8bb5f7186940");

        //PrintersType
        //THERMAL_PRINTER_WINDOWS
        public static Guid XpoOidConfigurationPrinterTypeThermalPrinterWindows = new Guid("e7143ea5-391b-46ef-a28d-4843fd7e21ac");
        //THERMAL_PRINTER_SOCKET
        public static Guid XpoOidConfigurationPrinterTypeThermalPrinterSocket = new Guid("faeb45cd-2989-4e92-9907-3038444e4849");
        //GENERIC_PRINTER_WINDOWS
        public static Guid XpoOidConfigurationPrinterTypeGenericWindows = new Guid("4be662e6-67c9-4063-bd6c-574ae4df7f3f");
        //REPORT_EXPORT_PDF
        public static Guid XpoOidConfigurationPrinterTypeExportPdf = new Guid("e5e20cd0-d9d2-443d-9d3f-3478949db30f");
        //THERMAL _PRINTER_USB
        public static Guid XpoOidConfigurationPrinterTypeThermalPrinterUsb = new Guid("39b58a4e-b860-49c1-81a5-8bb5f7186940");
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
        /// <summary>
        /// Default value for when filtering documents to be presented in Notification window
        /// </summary>
        public static int XpoOidSystemNotificationDaysBackWhenFiltering = 5;
        //Notification Types
        public static Guid XpoOidSystemNotificationTypeNewTerminalRegistered = new Guid("bc1f6a82-fa8e-49c8-981c-46ff21aef8b4");
        public static Guid XpoOidSystemNotificationTypeCurrentAccountDocumentsToInvoice = new Guid("06319d46-e7b5-4cca-8257-55eff4cfe0fa");
        public static Guid XpoOidSystemNotificationTypeConsignationInvoiceDocumentsToInvoice = new Guid("a567578b-53e9-4b5c-848f-183c65194971");
        public static Guid XpoOidSystemNotificationTypeSaftDocumentTypeMovementOfGoods = new Guid("80a03838-0937-4ae3-921f-75a1e358f7bf");

        //ConfigurationPriceType
        //Retail Mode|New Document to get PriceType when customer is not Defined
        public static Guid XpoOidConfigurationPriceTypeDefault = new Guid("cf17a218-b687-4b82-a8f4-0905594ac1f5");

        /* TK013134 - Parking Ticket */
        public static Guid XpoOidArticleParkingTicket    = new Guid("f4c6294d-0a57-4f36-951d-87ab2e076ef1");
        public static Guid XpoOidArticleParkingCard      = new Guid("32829702-33fa-48d5-917c-4c1db8720777");
        public static Guid XpoOidArticleParkingSubfamily = new Guid("d0c8169b-a5bc-46cb-b8ff-186b0ba39929");

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Thermal Printer Ticket

        // Defaults for New PrinterThermal Dialog
        public static string PrinterThermalEncoding = "PC860";
        public static bool PrinterThermalPrintLogo = false;
        public static string PrinterThermalImageCompanyLogo = "Images/Tickets/company_loggero_thermal.bmp";
        public static int PrinterThermalMaxCharsPerLineNormal = 48;
        public static int PrinterThermalMaxCharsPerLineNormalBold = 44;
        public static int PrinterThermalMaxCharsPerLineSmall = 64;
        public static string PrinterThermalCutCommand = "0x42,0x00";
        public static int PrinterThermalOpenDrawerValueM = 0;
        public static int PrinterThermalOpenDrawerValueT1 = 3;
        public static int PrinterThermalOpenDrawerValueT2 = 49;

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
            AppSoftwareName = SharedUtils.GetSoftwareVendorValueAsString(nameof(AppSoftwareName), debug);
            AppCompanyName = SharedUtils.GetSoftwareVendorValueAsString(nameof(AppCompanyName), debug);
            AppCompanyPhone = SharedUtils.GetSoftwareVendorValueAsString(nameof(AppCompanyPhone), debug);
            AppCompanyEmail = SharedUtils.GetSoftwareVendorValueAsString(nameof(AppCompanyEmail), debug);
            AppCompanyWeb = SharedUtils.GetSoftwareVendorValueAsString(nameof(AppCompanyWeb), debug);
            AppSoftwareVersionFormat = SharedUtils.GetSoftwareVendorValueAsString(nameof(AppSoftwareVersionFormat), debug);

            FileFormatDateTime = SharedUtils.GetSoftwareVendorValueAsString(nameof(FileFormatDateTime), debug);
            FileFormatSaftPT = SharedUtils.GetSoftwareVendorValueAsString(nameof(FileFormatSaftPT), debug);
            FileFormatSaftAO = SharedUtils.GetSoftwareVendorValueAsString(nameof(FileFormatSaftAO), debug);

            DocumentsPadLength = SharedUtils.GetSoftwareVendorValueAsInt(nameof(DocumentsPadLength), debug);
            DateTimeFormatDocumentDate = SharedUtils.GetSoftwareVendorValueAsString(nameof(DateTimeFormatDocumentDate), debug);
            DateTimeFormatCombinedDateTime = SharedUtils.GetSoftwareVendorValueAsString(nameof(DateTimeFormatCombinedDateTime), debug);
            FinanceFinalConsumerFiscalNumber = SharedUtils.GetSoftwareVendorValueAsString(nameof(FinanceFinalConsumerFiscalNumber), debug);
            FinanceFinalConsumerFiscalNumberDisplay = SharedUtils.GetSoftwareVendorValueAsString(nameof(FinanceFinalConsumerFiscalNumberDisplay), debug);
            DecimalFormatSAFTPT = SharedUtils.GetSoftwareVendorValueAsString(nameof(DecimalFormatSAFTPT), debug);
            DecimalFormatSAFTAO = SharedUtils.GetSoftwareVendorValueAsString(nameof(DecimalFormatSAFTAO), debug);
            DecimalFormatGrossTotalSAFTPT = SharedUtils.GetSoftwareVendorValueAsString(nameof(DecimalFormatGrossTotalSAFTPT), debug);
            DecimalRoundTo = SharedUtils.GetSoftwareVendorValueAsInt(nameof(DecimalRoundTo), debug);
            SaftProductCompanyTaxID = SharedUtils.GetSoftwareVendorValueAsString(nameof(SaftProductCompanyTaxID), debug);
            SaftSoftwareCertificateNumber = SharedUtils.GetSoftwareVendorValueAsString(nameof(SaftSoftwareCertificateNumber), debug);
            SaftSoftwareCertificateNumberAO = SharedUtils.GetSoftwareVendorValueAsString(nameof(SaftSoftwareCertificateNumberAO), debug);
            SaftVersionPrefix = SharedUtils.GetSoftwareVendorValueAsString(nameof(SaftVersionPrefix), debug);
            SaftVersionPrefixAO = SharedUtils.GetSoftwareVendorValueAsString(nameof(SaftVersionPrefixAO), debug);
            SaftVersion = SharedUtils.GetSoftwareVendorValueAsString(nameof(SaftVersion), debug);
            SaftVersionAO = SharedUtils.GetSoftwareVendorValueAsString(nameof(SaftVersionAO), debug);
            HashControl = SharedUtils.GetSoftwareVendorValueAsInt(nameof(HashControl), debug);
            TaxAccountingBasis = SharedUtils.GetSoftwareVendorValueAsString(nameof(TaxAccountingBasis), debug);
            SaftCurrencyCode = SharedUtils.GetSoftwareVendorValueAsString(nameof(SaftCurrencyCode), debug);
            SaftCurrencyCodeAO = SharedUtils.GetSoftwareVendorValueAsString(nameof(SaftCurrencyCodeAO), debug);

            DocumentFinanceSeriesGenerationFactoryUseRandomAcronymPrefix = SharedUtils.GetSoftwareVendorValueAsBool(nameof(DocumentFinanceSeriesGenerationFactoryUseRandomAcronymPrefix), debug);
            DocumentFinanceSeriesGenerationFactoryAcronymLastSerieFormat = SharedUtils.GetSoftwareVendorValueAsString(nameof(DocumentFinanceSeriesGenerationFactoryAcronymLastSerieFormat), debug);
        }

        private static string GetSaftProductID()
        {
            return string.Format("{0}/{1}", AppSoftwareName, AppCompanyName);
        }

        private static string GetSaftProductID_AO()
        {
            return string.Format("{0}", AppSoftwareName);
        }


        //Use to limit Simplified Invoices to have a total limit of 1000 (Articles Products + Services)
        private static int GetFinanceRuleSimplifiedInvoiceMaxTotal()
        {
            bool byPassValue = false;

            int result;
            //PT : Override Defaults
            if (DataLayerSettings.ConfigurationSystemCountry.Oid == XpoOidConfigurationCountryPortugal)
            {
                if (Debugger.IsAttached && byPassValue)
                {
                    result = 999999999;
                }
                else
                {
                    result = (LogicPOS.Settings.PluginSettings.PluginSoftwareVendor != null)
                        // From Vendor Plugin
                        ? LogicPOS.Settings.PluginSettings.PluginSoftwareVendor.GetFinanceRuleSimplifiedInvoiceMaxTotal()
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
            bool byPassValue = false;

            int result;
            //PT : Override Defaults
            if (DataLayerSettings.ConfigurationSystemCountry.Oid == XpoOidConfigurationCountryPortugal)
            {
                if (Debugger.IsAttached && byPassValue)
                {
                    result = 999999999;
                }
                else
                {
                    result = (LogicPOS.Settings.PluginSettings.PluginSoftwareVendor != null)
                        // From Vendor Plugin
                        ? LogicPOS.Settings.PluginSettings.PluginSoftwareVendor.GetFinanceRuleSimplifiedInvoiceMaxTotalServices()
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
            int result;

            //PT : Override Defaults
            if (DataLayerSettings.ConfigurationSystemCountry.Oid == XpoOidConfigurationCountryPortugal)
            {
                result = (LogicPOS.Settings.PluginSettings.PluginSoftwareVendor != null)
                    // From Vendor Plugin
                    ? LogicPOS.Settings.PluginSettings.PluginSoftwareVendor.GetFinanceRuleRequiredCustomerDetailsAboveValue()
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
