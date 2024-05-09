using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Xpo;
using LogicPOS.Settings;
using System;
using System.Diagnostics;
using static LogicPOS.Settings.CultureSettings;
using static LogicPOS.Settings.PluginSettings;

namespace logicpos.shared.App
{
    public static class SharedSettings
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public static string AppSessionFile { get; set; } = "appsession_{0}.json";
        public static bool AppSessionFileJsonIndented { get; set; } = true;

        public static cfg_configurationcurrency ConfigurationSystemCurrency { get; set; } = null;

        public static int HashControl { get; set; }
       

        public static int FinanceRuleSimplifiedInvoiceMaxTotal { get { return GetFinanceRuleSimplifiedInvoiceMaxTotal(); } }
        //Services
        public static int FinanceRuleSimplifiedInvoiceMaxTotalServices { get { return GetFinanceRuleSimplifiedInvoiceMaxTotalServices(); } }
        //This rule is to force fill Customer details if total document value is Greater or Equal to
        public static int FinanceRuleRequiredCustomerDetailsAboveValue { get { return GetFinanceRuleRequiredCustomerDetailsAboveValue(); } }

       

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Developer Config

        // Undefined Record
        public static Guid XpoOidUndefinedRecord { get; set; } = new Guid("00000000-0000-0000-0000-000000000001");
        public static Guid XpoOidUserRecord { get; set; } = new Guid("00000000-0000-0000-0000-000000000002");
        public static string XpoOidHiddenRecordsFilter { get; set; } = "00000000-0000-0000-0000-000000000%";

        //ArticleClass
        public static Guid XpoOidArticleClassCustomerCard { get; set; } = new Guid("49ea35ba-35f3-440f-946e-ab32578ed741");

        //Notifications
        /// <summary>
        /// Default value for when filtering documents to be presented in Notification window
        /// </summary>
        public static int XpoOidSystemNotificationDaysBackWhenFiltering { get; set; } = 5;
        //Notification Types
        public static Guid XpoOidSystemNotificationTypeNewTerminalRegistered { get; set; } = new Guid("bc1f6a82-fa8e-49c8-981c-46ff21aef8b4");
        public static Guid XpoOidSystemNotificationTypeCurrentAccountDocumentsToInvoice { get; set; } = new Guid("06319d46-e7b5-4cca-8257-55eff4cfe0fa");
        public static Guid XpoOidSystemNotificationTypeConsignationInvoiceDocumentsToInvoice { get; set; } = new Guid("a567578b-53e9-4b5c-848f-183c65194971");
        public static Guid XpoOidSystemNotificationTypeSaftDocumentTypeMovementOfGoods { get; set; } = new Guid("80a03838-0937-4ae3-921f-75a1e358f7bf");

        //ConfigurationPriceType
        //Retail Mode|New Document to get PriceType when customer is not Defined
        public static Guid XpoOidConfigurationPriceTypeDefault { get; set; } = new Guid("cf17a218-b687-4b82-a8f4-0905594ac1f5");

        /* TK013134 - Parking Ticket */
        public static Guid XpoOidArticleParkingTicket { get; set; } = new Guid("f4c6294d-0a57-4f36-951d-87ab2e076ef1");
        public static Guid XpoOidArticleParkingCard { get; set; } = new Guid("32829702-33fa-48d5-917c-4c1db8720777");

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Reports

        //Used to Enable/Disable DoublePass, else Blank Page when DoublePass Enabled with One Page
        //used in ProcessReportFinanceDocument.customReport.DoublePass
        //When change value here, change script block in .frx too (_dataBandMaxRecs = 15;)
        public static uint CustomReportReportDocumentFinanceMaxDetail { get; set; } = 15;

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Helper Methods

        /// <summary>
        /// Assign OneTime AppSettings from SoftwareVendorPluginSettings
        /// </summary>
        public static void InitSoftwareVendorPluginSettings()
        {
            AppSoftwareName = GetSoftwareVendorValueAsString(nameof(AppSoftwareName));
            AppCompanyName = GetSoftwareVendorValueAsString(nameof(AppCompanyName));
            AppCompanyPhone = GetSoftwareVendorValueAsString(nameof(AppCompanyPhone));
            AppCompanyEmail = GetSoftwareVendorValueAsString(nameof(AppCompanyEmail));
            AppCompanyWeb = GetSoftwareVendorValueAsString(nameof(AppCompanyWeb));
            AppSoftwareVersionFormat = GetSoftwareVendorValueAsString(nameof(AppSoftwareVersionFormat));

            FileFormatDateTime = GetSoftwareVendorValueAsString(nameof(FileFormatDateTime));
            FileFormatSaftPT = GetSoftwareVendorValueAsString(nameof(FileFormatSaftPT));
            FileFormatSaftAO = GetSoftwareVendorValueAsString(nameof(FileFormatSaftAO));

            SaftSettings.DocumentsPadLength = GetSoftwareVendorValueAsInt(nameof(SaftSettings.DocumentsPadLength));
            DateTimeFormatDocumentDate = GetSoftwareVendorValueAsString(nameof(DateTimeFormatDocumentDate));
            DateTimeFormatCombinedDateTime = GetSoftwareVendorValueAsString(nameof(DateTimeFormatCombinedDateTime));
            SaftSettings.FinanceFinalConsumerFiscalNumber = GetSoftwareVendorValueAsString(nameof(SaftSettings.FinanceFinalConsumerFiscalNumber));
            SaftSettings.FinanceFinalConsumerFiscalNumberDisplay = GetSoftwareVendorValueAsString(nameof(SaftSettings.FinanceFinalConsumerFiscalNumberDisplay));
            DecimalFormatSAFTPT = GetSoftwareVendorValueAsString(nameof(DecimalFormatSAFTPT));
            DecimalFormatSAFTAO = GetSoftwareVendorValueAsString(nameof(DecimalFormatSAFTAO));
            DecimalFormatGrossTotalSAFTPT = GetSoftwareVendorValueAsString(nameof(DecimalFormatGrossTotalSAFTPT));
            DecimalRoundTo = GetSoftwareVendorValueAsInt(nameof(DecimalRoundTo));
            SaftSettings.SaftProductCompanyTaxID = GetSoftwareVendorValueAsString(nameof(SaftSettings.SaftProductCompanyTaxID));
            SaftSettings.SaftSoftwareCertificateNumber = GetSoftwareVendorValueAsString(nameof(SaftSettings.SaftSoftwareCertificateNumber));
            SaftSettings.SaftSoftwareCertificateNumberAO = GetSoftwareVendorValueAsString(nameof(SaftSettings.SaftSoftwareCertificateNumberAO));
            SaftSettings.SaftVersionPrefix = GetSoftwareVendorValueAsString(nameof(SaftSettings.SaftVersionPrefix));
            SaftSettings.SaftVersionPrefixAO = GetSoftwareVendorValueAsString(nameof(SaftSettings.SaftVersionPrefixAO));
            SaftSettings.SaftVersion = GetSoftwareVendorValueAsString(nameof(SaftSettings.SaftVersion));
            SaftSettings.SaftVersionAO = GetSoftwareVendorValueAsString(nameof(SaftSettings.SaftVersionAO));
            HashControl = GetSoftwareVendorValueAsInt(nameof(HashControl));
            SaftSettings.TaxAccountingBasis = GetSoftwareVendorValueAsString(nameof(SaftSettings.TaxAccountingBasis));
            SaftCurrencyCode = GetSoftwareVendorValueAsString(nameof(SaftCurrencyCode));
            SaftCurrencyCodeAO = GetSoftwareVendorValueAsString(nameof(SaftCurrencyCodeAO));

            DocumentSettings.DocumentFinanceSeriesGenerationFactoryUseRandomAcronymPrefix = GetSoftwareVendorValueAsBool(nameof(DocumentSettings.DocumentFinanceSeriesGenerationFactoryUseRandomAcronymPrefix));
            DocumentSettings.DocumentFinanceSeriesGenerationFactoryAcronymLastSerieFormat = GetSoftwareVendorValueAsString(nameof(DocumentSettings.DocumentFinanceSeriesGenerationFactoryAcronymLastSerieFormat));
        }

    
        //Use to limit Simplified Invoices to have a total limit of 1000 (Articles Products + Services)
        private static int GetFinanceRuleSimplifiedInvoiceMaxTotal()
        {
            bool byPassValue = false;

            int result;
            //PT : Override Defaults
            if (XPOSettings.ConfigurationSystemCountry.Oid == XpoOidConfigurationCountryPortugal)
            {
                if (Debugger.IsAttached && byPassValue)
                {
                    result = 999999999;
                }
                else
                {
                    result = (PluginSoftwareVendor != null)
                        // From Vendor Plugin
                        ? PluginSoftwareVendor.GetFinanceRuleSimplifiedInvoiceMaxTotal()
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
            if (XPOSettings.ConfigurationSystemCountry.Oid == XpoOidConfigurationCountryPortugal)
            {
                if (Debugger.IsAttached && byPassValue)
                {
                    result = 999999999;
                }
                else
                {
                    result = (PluginSoftwareVendor != null)
                        // From Vendor Plugin
                        ? PluginSoftwareVendor.GetFinanceRuleSimplifiedInvoiceMaxTotalServices()
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
            if (XPOSettings.ConfigurationSystemCountry.Oid == XpoOidConfigurationCountryPortugal)
            {
                result = (PluginSoftwareVendor != null)
                    // From Vendor Plugin
                    ? PluginSoftwareVendor.GetFinanceRuleRequiredCustomerDetailsAboveValue()
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