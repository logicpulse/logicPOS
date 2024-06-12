using logicpos.shared.Enums;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Finance.Utility;
using LogicPOS.Settings;
using LogicPOS.Shared;
using LogicPOS.Shared.Article;
using LogicPOS.Shared.CustomDocument;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LogicPOS.Finance.DocumentProcessing
{

    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //Main Class ProcessFinanceDocumentValidation

    public class DocumentProcessingValidationUtils
    {
        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

#if (DEBUG)
        private static readonly bool _debug = true;
#else
        private static bool _debug = false;
#endif
        private static string _countryCode2 = string.Empty;
        private static SortedDictionary<DocumentValidationErrorType, object> _resultEnum;
        private static Dictionary<DocumentValidationErrorType, DocumentProcessingValidationField> _fields;
        private static Dictionary<DocumentValidationErrorType, DocumentProcessingValidationField> _fieldsArticle;
        private static bool _requireToChooseVatExemptionReason = true;

        //Constructors
        public static SortedDictionary<DocumentValidationErrorType, object> ValidatePersistFinanceDocument(DocumentProcessingParameters pParameters, bool pIgnoreWarning = false)
        {
            Guid userDetailGuid = (XPOSettings.LoggedUser != null) ? XPOSettings.LoggedUser.Oid : Guid.Empty;
            return ValidatePersistFinanceDocument(pParameters, userDetailGuid, pIgnoreWarning);
        }

        public static SortedDictionary<DocumentValidationErrorType, object> ValidatePersistFinanceDocument(DocumentProcessingParameters pParameters, Guid pLoggedUser, bool pIgnoreWarning = false)
        {
            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //Init Common Variables

            _resultEnum = new SortedDictionary<DocumentValidationErrorType, object>();
            _fields = new Dictionary<DocumentValidationErrorType, DocumentProcessingValidationField>();
            _fieldsArticle = new Dictionary<DocumentValidationErrorType, DocumentProcessingValidationField>();

            try
            {
                // Override Default with Config Value
                _requireToChooseVatExemptionReason = Convert.ToBoolean(LogicPOS.Settings.GeneralSettings.Settings["requireToChooseVatExemptionReason"]);
            }
            catch (Exception)
            {
                _logger.Error("Error Missing Config Parameter Key: [requireToChooseVatExemptionReason]");
            }


            //Disable to go to AT or to Release
            if (!Debugger.IsAttached) return _resultEnum;

            try
            {
                //Get XPGuidObjects from Parameters
                fin_documentfinancetype documentType = (pParameters.DocumentType != null && pParameters.DocumentType != Guid.Empty)
                    ? (fin_documentfinancetype)XPOUtility.GetXPGuidObject(XPOSettings.Session, typeof(fin_documentfinancetype), pParameters.DocumentType)
                    : null;
                erp_customer customer = (pParameters.Customer != null && pParameters.Customer != Guid.Empty)
                    ? (erp_customer)XPOUtility.GetXPGuidObject(XPOSettings.Session, typeof(erp_customer), pParameters.Customer)
                    : null;
                fin_documentfinancemaster documentParent = (pParameters.DocumentParent != null && pParameters.DocumentParent != Guid.Empty)
                    ? (fin_documentfinancemaster)XPOUtility.GetXPGuidObject(XPOSettings.Session, typeof(fin_documentfinancemaster), pParameters.DocumentParent)
                    : null;
                erp_customer customerParentDocument = (documentParent != null && documentParent.EntityOid != Guid.Empty)
                    ? (erp_customer)XPOUtility.GetXPGuidObject(XPOSettings.Session, typeof(erp_customer), documentParent.EntityOid)
                    : null;
                cfg_configurationcurrency configurationCurrency = (pParameters.Currency != null && pParameters.Currency != Guid.Empty)
                    ? (cfg_configurationcurrency)XPOUtility.GetXPGuidObject(XPOSettings.Session, typeof(cfg_configurationcurrency), pParameters.Currency)
                    : null;
                cfg_configurationcountry countryShipTo = (pParameters.ShipTo != null && pParameters.ShipTo.CountryGuid != Guid.Empty)
                    ? (cfg_configurationcountry)XPOUtility.GetXPGuidObject(XPOSettings.Session, typeof(cfg_configurationcountry), pParameters.ShipTo.CountryGuid)
                    : null;
                cfg_configurationcountry countryShipFrom = (pParameters.ShipFrom != null && pParameters.ShipFrom.CountryGuid != Guid.Empty)
                    ? (cfg_configurationcountry)XPOUtility.GetXPGuidObject(XPOSettings.Session, typeof(cfg_configurationcountry), pParameters.ShipFrom.CountryGuid)
                    : null;
                fin_configurationpaymentmethod configurationPaymentMethod = (pParameters.PaymentMethod != null && pParameters.PaymentMethod != Guid.Empty)
                    ? (fin_configurationpaymentmethod)XPOUtility.GetXPGuidObject(XPOSettings.Session, typeof(fin_configurationpaymentmethod), pParameters.PaymentMethod)
                    : null;
                //Helper Variables
                if (customer != null) _countryCode2 = customer.Country.Code2;
                decimal totalDocumentForeignCurrency = (pParameters.ArticleBag.TotalFinal);
                decimal totalDocumentSystemCurrency = (configurationCurrency != null)
                    ? (pParameters.ArticleBag.TotalFinal * configurationCurrency.ExchangeRate)
                    : totalDocumentForeignCurrency
                ;
                bool customerIsSingularEntity = customer == null || customer.Country == null
|| FiscalNumberUtils.IsSingularEntity(customer.FiscalNumber, customer.Country.Code2);
                //RegEx
                //If not Saft Document Type 2, required greater than zero in Price, else we can have zero or greater from Document Type 2 (ex Transportation Guide)
                string regExArticlePrice = (documentType != null && documentType.SaftDocumentType != SaftDocumentType.MovementOfGoods) ? LogicPOS.Utility.RegexUtils.RegexDecimalGreaterThanZero : LogicPOS.Utility.RegexUtils.RegexDecimalGreaterEqualThanZero;
                //Never Override SettingsApp.FinanceRuleSimplifiedInvoiceMaxValue|FinanceRuleRequiredCustomerDetailsAboveValue values to be Sync with LogicPos UI
                int financeRuleSimplifiedInvoiceMaxTotal = InvoiceSettings.GetSimplifiedInvoiceMaxItems(XPOSettings.ConfigurationSystemCountry.Oid);
                int financeRuleSimplifiedInvoiceMaxTotalServices = InvoiceSettings.GetSimplifiedInvoiceMaxServices(XPOSettings.ConfigurationSystemCountry.Oid);
                int financeRuleRequiredCustomerDetailsAboveValue = GeneralSettings.GetRequiredCustomerDetailsAboveValue(XPOSettings.ConfigurationSystemCountry.Oid);


                //Required Fields

                //P1
                bool requiredPaymentCondition = (
                    pParameters.DocumentType == InvoiceSettings.InvoiceId ||
                    pParameters.DocumentType == DocumentSettings.ConsignationInvoiceId ||
                    pParameters.DocumentType == DocumentSettings.XpoOidDocumentFinanceTypeBudget ||
                    pParameters.DocumentType == DocumentSettings.XpoOidDocumentFinanceTypeProformaInvoice
                );
                bool requiredPaymentMethod = (
                    pParameters.DocumentType == DocumentSettings.SimplifiedInvoiceId ||
                    pParameters.DocumentType == DocumentSettings.InvoiceAndPaymentId
                );
                bool requireParentDocument = (pParameters.DocumentType == CustomDocumentSettings.CreditNoteId);

                //P2
                bool requireAllCustomerFields = (
                    //SimplifiedInvoice
                    (
                        pParameters.DocumentType == DocumentSettings.SimplifiedInvoiceId &&
                        pParameters.ArticleBag.TotalFinal > financeRuleRequiredCustomerDetailsAboveValue &&
                        !customerIsSingularEntity
                    )
                );
                bool requireCustomerName = (requireAllCustomerFields || (customer == null || customer.FiscalNumber == null || customer.FiscalNumber == string.Empty));
                bool requireFiscalNumber = (requireAllCustomerFields || (customer == null || customer.Name == null || customer.Name == string.Empty));

                //P3|P4
                bool requiredWayBillModeFields = (documentType != null && documentType.WayBill);

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                // Show Output
                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                Console.WriteLine(string.Format("Validate DocumentType: [{1}]{0}", Environment.NewLine, documentType.Designation));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                // Process Field Validation
                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Global
                _fields.Add(DocumentValidationErrorType.ERROR_RULE_ARTICLEBAG_GLOBAL_DISCOUNT_INVALID,
                    new DocumentProcessingValidationField("ArticleBag.Discount", (pParameters.ArticleBag != null) ? pParameters.ArticleBag.DiscountGlobal : 0.0m, LogicPOS.Utility.RegexUtils.RegexPercentage, true)
                );
                //P1
                _fields.Add(DocumentValidationErrorType.ERROR_FIELD_DOCUMENT_TYPE_INVALID,
                    new DocumentProcessingValidationField("DocumentType", pParameters.DocumentType, LogicPOS.Utility.RegexUtils.RegexGuid, true)
                );
                _fields.Add(DocumentValidationErrorType.ERROR_FIELD_PAYMENT_CONDITION_INVALID,
                    new DocumentProcessingValidationField("PaymentCondition", pParameters.PaymentCondition, LogicPOS.Utility.RegexUtils.RegexGuid, requiredPaymentCondition)
                );
                _fields.Add(DocumentValidationErrorType.ERROR_FIELD_PAYMENT_METHOD_INVALID,
                    new DocumentProcessingValidationField("PaymentMethod", pParameters.PaymentMethod, LogicPOS.Utility.RegexUtils.RegexGuid, requiredPaymentMethod)
                );
                _fields.Add(DocumentValidationErrorType.ERROR_FIELD_CURRENCY_INVALID,
                    new DocumentProcessingValidationField("Currency", pParameters.Currency, LogicPOS.Utility.RegexUtils.RegexGuid, true)
                );
                _fields.Add(DocumentValidationErrorType.ERROR_FIELD_DOCUMENT_PARENT_INVALID,
                    new DocumentProcessingValidationField("DocumentParent", pParameters.DocumentParent, LogicPOS.Utility.RegexUtils.RegexGuid, requireParentDocument)
                );
                _fields.Add(DocumentValidationErrorType.ERROR_FIELD_NOTES_INVALID,
                    new DocumentProcessingValidationField("Notes", pParameters.Notes, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false)
                );
                //P2
                _fields.Add(DocumentValidationErrorType.ERROR_FIELD_CUSTOMER_NAME_INVALID,
                    new DocumentProcessingValidationField("Customer.Name", (customer != null) ? customer.Name : string.Empty, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, requireCustomerName)
                );
                _fields.Add(DocumentValidationErrorType.ERROR_FIELD_CUSTOMER_ADDRESS_INVALID,
                    new DocumentProcessingValidationField("Customer.Address", (customer != null) ? customer.Address : string.Empty, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, requireAllCustomerFields)
                );
                _fields.Add(DocumentValidationErrorType.ERROR_FIELD_CUSTOMER_LOCALITY_INVALID,
                    new DocumentProcessingValidationField("Customer.Locality", (customer != null) ? customer.Locality : string.Empty, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false)
                );
                //If customer undefined Defaults to SettingsApp.ConfigurationSystemCountry.RegExZipCode
                _fields.Add(DocumentValidationErrorType.ERROR_FIELD_CUSTOMER_ZIPCODE_INVALID,
                new DocumentProcessingValidationField("Customer.ZipCode", (customer != null && customer.ZipCode != null) ? customer.ZipCode : string.Empty, (customer != null && customer.Country != null) ? customer.Country.RegExZipCode : XPOSettings.ConfigurationSystemCountry.RegExZipCode, requireAllCustomerFields)
                );
                _fields.Add(DocumentValidationErrorType.ERROR_FIELD_CUSTOMER_CITY_INVALID,
                    new DocumentProcessingValidationField("Customer.City", (customer != null && customer.City != null) ? customer.City : string.Empty, LogicPOS.Utility.RegexUtils.RegexAlfa, requireAllCustomerFields)
                );
                _fields.Add(DocumentValidationErrorType.ERROR_FIELD_CUSTOMER_COUNTRY_INVALID,
                    new DocumentProcessingValidationField("Customer.Country", (customer != null && customer.Country != null) ? customer.Country.Oid : Guid.Empty, LogicPOS.Utility.RegexUtils.RegexGuid, true)
                );
                //If customer undefined Defaults to SettingsApp.ConfigurationSystemCountry.RegExFiscalNumber
                _fields.Add(DocumentValidationErrorType.ERROR_FIELD_CUSTOMER_FISCAL_NUMBER_INVALID,
                    new DocumentProcessingValidationField("Customer.FiscalNumber", (customer != null && customer.FiscalNumber != null) ? customer.FiscalNumber : string.Empty, (customer != null && customer.Country != null) ? customer.Country.RegExFiscalNumber : XPOSettings.ConfigurationSystemCountry.RegExFiscalNumber, requireFiscalNumber)
                );
                _fields.Add(DocumentValidationErrorType.ERROR_FIELD_CUSTOMER_CARDNUMBER_INVALID,
                    new DocumentProcessingValidationField("Customer.CardNumber", (customer != null && customer.CardNumber != null) ? customer.CardNumber : string.Empty, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false)
                );
                _fields.Add(DocumentValidationErrorType.ERROR_FIELD_CUSTOMER_DISCOUNT_INVALID,
                    new DocumentProcessingValidationField("Customer.Discount", (customer != null) ? customer.Discount : 0.0m, LogicPOS.Utility.RegexUtils.RegexPercentage, true)
                );
                //P4
                if (pParameters.ShipTo != null && countryShipTo != null)
                {
                    _fields.Add(DocumentValidationErrorType.ERROR_FIELD_SHIPTO_ADDRESS_DETAIL_INVALID,
                        new DocumentProcessingValidationField("ShipTo.AddressDetail", (pParameters.ShipTo != null) ? pParameters.ShipTo.AddressDetail : string.Empty, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, requiredWayBillModeFields)
                    );
                    _fields.Add(DocumentValidationErrorType.ERROR_FIELD_SHIPTO_REGION_INVALID,
                        new DocumentProcessingValidationField("ShipTo.Region", (pParameters.ShipTo != null) ? pParameters.ShipTo.Region : string.Empty, LogicPOS.Utility.RegexUtils.RegexAlfa, false)
                    );
                    _fields.Add(DocumentValidationErrorType.ERROR_FIELD_SHIPTO_POSTALCODE_INVALID,
                        new DocumentProcessingValidationField("ShipTo.PostalCode", (pParameters.ShipTo != null) ? pParameters.ShipTo.PostalCode : string.Empty, countryShipTo.RegExZipCode, requiredWayBillModeFields)
                    );
                    _fields.Add(DocumentValidationErrorType.ERROR_FIELD_SHIPTO_CITY_INVALID,
                        new DocumentProcessingValidationField("ShipTo.City", (pParameters.ShipTo != null) ? pParameters.ShipTo.City : string.Empty, LogicPOS.Utility.RegexUtils.RegexAlfa, requiredWayBillModeFields)
                    );
                    _fields.Add(DocumentValidationErrorType.ERROR_FIELD_SHIPTO_COUNTRY_INVALID,
                        new DocumentProcessingValidationField("ShipTo.Country", (pParameters.ShipTo != null) ? pParameters.ShipTo.CountryGuid : Guid.Empty, LogicPOS.Utility.RegexUtils.RegexGuid, requiredWayBillModeFields)
                    );
                    _fields.Add(DocumentValidationErrorType.ERROR_FIELD_SHIPTO_DELIVERYDATE_INVALID,
                        new DocumentProcessingValidationField("ShipTo.DeliveryDate", (pParameters.ShipTo != null && pParameters.ShipTo.DeliveryDate != DateTime.MinValue) ? pParameters.ShipTo.DeliveryDate.ToString(LogicPOS.Settings.CultureSettings.DateTimeFormat) : string.Empty, LogicPOS.Utility.RegexUtils.RegexDateTime, requiredWayBillModeFields)
                    );
                    _fields.Add(DocumentValidationErrorType.ERROR_FIELD_SHIPTO_DELIVERYID_INVALID,
                        new DocumentProcessingValidationField("ShipTo.DeliveryID", (pParameters.ShipTo != null) ? pParameters.ShipTo.DeliveryID : string.Empty, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false)
                    );
                    _fields.Add(DocumentValidationErrorType.ERROR_FIELD_SHIPTO_WAREHOUSEID_INVALID,
                        new DocumentProcessingValidationField("ShipTo.WarehouseID", (pParameters.ShipTo != null) ? pParameters.ShipTo.WarehouseID : string.Empty, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false)
                    );
                    _fields.Add(DocumentValidationErrorType.ERROR_FIELD_SHIPTO_LOCATIONID_INVALID,
                        new DocumentProcessingValidationField("ShipTo.LocationID", (pParameters.ShipTo != null) ? pParameters.ShipTo.LocationID : string.Empty, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false)
                    );
                }
                //P5
                if (pParameters.ShipTo != null && countryShipFrom != null)
                {
                    _fields.Add(DocumentValidationErrorType.ERROR_FIELD_SHIPFROM_ADDRESS_DETAIL_INVALID,
                        new DocumentProcessingValidationField("ShipFrom.AddressDetail", (pParameters.ShipFrom != null) ? pParameters.ShipFrom.AddressDetail : string.Empty, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, requiredWayBillModeFields)
                    );
                    _fields.Add(DocumentValidationErrorType.ERROR_FIELD_SHIPFROM_REGION_INVALID,
                        new DocumentProcessingValidationField("ShipFrom.Region", (pParameters.ShipFrom != null) ? pParameters.ShipFrom.Region : string.Empty, LogicPOS.Utility.RegexUtils.RegexAlfa, false)
                    );
                    _fields.Add(DocumentValidationErrorType.ERROR_FIELD_SHIPFROM_POSTALCODE_INVALID,
                        new DocumentProcessingValidationField("ShipFrom.PostalCode", (pParameters.ShipFrom != null) ? pParameters.ShipFrom.PostalCode : string.Empty, countryShipFrom.RegExZipCode, requiredWayBillModeFields)
                    );
                    _fields.Add(DocumentValidationErrorType.ERROR_FIELD_SHIPFROM_CITY_INVALID,
                        new DocumentProcessingValidationField("ShipFrom.City", (pParameters.ShipFrom != null) ? pParameters.ShipFrom.City : string.Empty, LogicPOS.Utility.RegexUtils.RegexAlfa, requiredWayBillModeFields)
                    );
                    _fields.Add(DocumentValidationErrorType.ERROR_FIELD_SHIPFROM_COUNTRY_INVALID,
                        new DocumentProcessingValidationField("ShipFrom.Country", (pParameters.ShipFrom != null) ? pParameters.ShipFrom.CountryGuid : Guid.Empty, LogicPOS.Utility.RegexUtils.RegexGuid, requiredWayBillModeFields)
                    );
                    _fields.Add(DocumentValidationErrorType.ERROR_FIELD_SHIPFROM_DELIVERYDATE_INVALID,
                        new DocumentProcessingValidationField("ShipFrom.DeliveryDate", (pParameters.ShipFrom != null && pParameters.ShipFrom.DeliveryDate != DateTime.MinValue) ? pParameters.ShipFrom.DeliveryDate.ToString(LogicPOS.Settings.CultureSettings.DateTimeFormat) : string.Empty, LogicPOS.Utility.RegexUtils.RegexDateTime, requiredWayBillModeFields)
                    );
                    _fields.Add(DocumentValidationErrorType.ERROR_FIELD_SHIPFROM_DELIVERYID_INVALID,
                        new DocumentProcessingValidationField("ShipFrom.DeliveryID", (pParameters.ShipFrom != null) ? pParameters.ShipFrom.DeliveryID : string.Empty, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false)
                    );
                    _fields.Add(DocumentValidationErrorType.ERROR_FIELD_SHIPFROM_WAREHOUSEID_INVALID,
                        new DocumentProcessingValidationField("ShipFrom.WarehouseID", (pParameters.ShipFrom != null) ? pParameters.ShipFrom.WarehouseID : string.Empty, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false)
                    );
                    _fields.Add(DocumentValidationErrorType.ERROR_FIELD_SHIPFROM_LOCATIONID_INVALID,
                        new DocumentProcessingValidationField("ShipFrom.LocationID", (pParameters.ShipFrom != null) ? pParameters.ShipFrom.LocationID : string.Empty, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false)
                    );
                }

                /* IN009109 - treatment for Transport Documents */
                if (requiredWayBillModeFields)
                {
                    DateTime shippingDate = pParameters.ShipFrom.DeliveryDate;
                    DateTime deliveryDate = pParameters.ShipTo.DeliveryDate;

                    if (deliveryDate < shippingDate)
                    {
                        _fields.Add(DocumentValidationErrorType.ERROR_FIELD_SHIPFROM_DELIVERYDATE_BEFORE_SHIPPINGDATE,
                        new DocumentProcessingValidationField("ShipFrom.DeliveryDate", string.Empty, LogicPOS.Utility.RegexUtils.RegexDateTime, true));
                    }
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                // Process Article Field Validation 
                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                _fieldsArticle.Add(DocumentValidationErrorType.ERROR_FIELD_ARTICLE_OID_INVALID,
                    new DocumentProcessingValidationField("Oid", null, LogicPOS.Utility.RegexUtils.RegexGuid, true)
                );
                _fieldsArticle.Add(DocumentValidationErrorType.ERROR_FIELD_ARTICLE_CODE_INVALID,
                    new DocumentProcessingValidationField("Code", null, LogicPOS.Utility.RegexUtils.RegexAlfaNumericArticleCode, true)
                );
                _fieldsArticle.Add(DocumentValidationErrorType.ERROR_FIELD_ARTICLE_DESIGNATION_INVALID,
                    new DocumentProcessingValidationField("Designation", null, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true)
                );
                _fieldsArticle.Add(DocumentValidationErrorType.ERROR_FIELD_ARTICLE_PRICE_INVALID,
                    new DocumentProcessingValidationField("Price", null, regExArticlePrice, true)
                );
                _fieldsArticle.Add(DocumentValidationErrorType.ERROR_FIELD_ARTICLE_QUANTITY_INVALID,
                    new DocumentProcessingValidationField("Quantity", null, LogicPOS.Utility.RegexUtils.RegexDecimalGreaterEqualThanZero, true)
                );
                //Removed : Framework LogicErp dont send ACRONYM : Search all ERROR_FIELD_ARTICLE_UNIT_MEASURE_ACRONYM_INVALID occurences
                //_fieldsArticle.Add(FinanceValidationError.ERROR_FIELD_ARTICLE_UNIT_MEASURE_ACRONYM_INVALID,
                //    new ProcessFinanceDocumentValidationField("UnitMeasure.Acronym", null, SettingsApp.RegexAlfaNumeric, true)
                //);
                _fieldsArticle.Add(DocumentValidationErrorType.ERROR_FIELD_ARTICLE_DISCOUNT_INVALID,
                    new DocumentProcessingValidationField("Discount", null, LogicPOS.Utility.RegexUtils.RegexPercentage, true)
                );
                _fieldsArticle.Add(DocumentValidationErrorType.ERROR_FIELD_ARTICLE_VAT_RATE_INVALID,
                    new DocumentProcessingValidationField("VatRate", null, LogicPOS.Utility.RegexUtils.RegexPercentage, true)
                );
                _fieldsArticle.Add(DocumentValidationErrorType.ERROR_FIELD_ARTICLE_VAT_EXEMPTION_REASON_INVALID,
                    new DocumentProcessingValidationField("VatExemptionReason", null, LogicPOS.Utility.RegexUtils.RegexGuid, false)
                );

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::            
                // Validation Rules
                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                if (pLoggedUser == Guid.Empty)
                {
                    ResultAdd(DocumentValidationErrorType.ERROR_RULE_loggerGED_USER_INVALID);
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                if (TerminalSettings.LoggedTerminal == null)
                {
                    ResultAdd(DocumentValidationErrorType.ERROR_RULE_loggerGED_TERMINAL_INVALID);
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                //Required a Valid LoggedTerminal

                fin_documentfinanceseries documentFinanceSerie = null;
                if (TerminalSettings.LoggedTerminal != null)
                {
                    documentFinanceSerie = DocumentProcessingSeriesUtils.GetDocumentFinanceYearSerieTerminal(pParameters.DocumentType).Serie;
                }

                if (documentFinanceSerie == null)
                {
                    ResultAdd(DocumentValidationErrorType.ERROR_RULE_SERIE_INVALID);
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Check Source Mode
                switch (pParameters.SourceMode)
                {
                    case PersistFinanceDocumentSourceMode.CurrentOrderMain:
                        if (POSSession.CurrentSession == null
                            || POSSession.CurrentSession.OrderMains == null
                            || POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId] == null
                        )
                        {
                            ResultAdd(DocumentValidationErrorType.ERROR_RULE_SOURCE_MODE_ORDERMAIN_EMPTY);
                        }
                        break;
                    case PersistFinanceDocumentSourceMode.CustomArticleBag:
                        if (pParameters.ArticleBag == null || pParameters.ArticleBag.Count <= 0)
                        {
                            ResultAdd(DocumentValidationErrorType.ERROR_RULE_SOURCE_MODE_CUSTOM_ARTICLEBAG_EMPTY);
                        }
                        break;
                    case PersistFinanceDocumentSourceMode.CurrentAcountDocuments:
                        if (pParameters.FinanceDocuments == null | pParameters.FinanceDocuments.Count <= 0)
                        {
                            ResultAdd(DocumentValidationErrorType.ERROR_RULE_SOURCE_MODE_FINANCE_DOCUMENTS_EMPTY);
                        }
                        break;
                    default:
                        break;
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                //Customer <> ParentDocument.Customer (And ConferenceDocument)
                // Moçambique - Pedidos da reunião 13/10/2020 + Faturas no Front-Office [IN:014327]
                if (customer != null && customerParentDocument != null && customer != customerParentDocument && documentParent.DocumentType.Oid != DocumentSettings.XpoOidDocumentFinanceTypeConferenceDocument)
                {
                    if (!CultureSettings.MozambiqueCountryId.Equals(XPOSettings.ConfigurationSystemCountry.Oid) && (documentParent.DocumentType.Oid != InvoiceSettings.InvoiceId || documentParent.DocumentType.Oid != DocumentSettings.SimplifiedInvoiceId))
                    {
                        ResultAdd(DocumentValidationErrorType.ERROR_RULE_PARENT_DOCUMENT_CUSTOMER_AND_CURRENT_DOCUMENT_CUSTOMER_INVALID);
                    }
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                //ParentDocument.DocumentStatusStatus = Cancelled Document

                if (documentParent != null && documentParent.DocumentStatusStatus == "A")
                {
                    ResultAdd(DocumentValidationErrorType.ERROR_RULE_PARENT_DOCUMENT_CANCELLED_DETECTED);
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                //All Series, Global System

                //This code is similar to : ShowMessageTouchCheckIfFinanceDocumentHasValidDocumentDate, change in both

                //Protection to Check if System.DateTime is < Last Document.DateTime (All Finance Documents)
                DateTime dateTimeLastDocument = DocumentProcessingUtils.GetLastDocumentDateTime();
                if (!pIgnoreWarning && pParameters.DocumentDateTime < dateTimeLastDocument && dateTimeLastDocument != DateTime.MinValue)
                {
                    ResultAdd(DocumentValidationErrorType.WARNING_RULE_SYSTEM_DATE_GLOBAL);
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                //From current Serie

                //Protection to Check if SystemDate.Date is < Last DocumentDate.Date (From Document Serie/DocumentType)
                DateTime dateLastDocumentFromSerie = DateTime.MaxValue;
                if (documentFinanceSerie != null)
                {
                    dateLastDocumentFromSerie = DocumentProcessingUtils.GetLastDocumentDateTime(string.Format("DocumentSerie = '{0}'", documentFinanceSerie.Oid)).Date;
                }

                if (!pIgnoreWarning && pParameters.DocumentDateTime < dateLastDocumentFromSerie && dateLastDocumentFromSerie != DateTime.MinValue)
                {
                    ResultAdd(DocumentValidationErrorType.WARNING_RULE_SYSTEM_DATE_SERIE);
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                //DocumentType

                if (pParameters.DocumentType == Guid.Empty)
                {
                    ResultAdd(DocumentValidationErrorType.ERROR_RULE_DOCUMENT_FINANCE_TYPE_INVALID);
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                //PaymentCondition

                if (pParameters.PaymentCondition == Guid.Empty &&
                    (
                        pParameters.DocumentType == InvoiceSettings.InvoiceId ||
                        pParameters.DocumentType == DocumentSettings.XpoOidDocumentFinanceTypeProformaInvoice ||
                        pParameters.DocumentType == DocumentSettings.XpoOidDocumentFinanceTypeBudget
                    )
                )
                {
                    ResultAdd(DocumentValidationErrorType.ERROR_RULE_DOCUMENT_FINANCE_TYPE_PAYMENT_CONDITION_INVALID);
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                //PaymentMethod

                if (pParameters.PaymentMethod == Guid.Empty &&
                    (
                        pParameters.DocumentType == DocumentSettings.SimplifiedInvoiceId ||
                        pParameters.DocumentType == DocumentSettings.InvoiceAndPaymentId
                    )
                )
                {
                    ResultAdd(DocumentValidationErrorType.ERROR_RULE_DOCUMENT_FINANCE_TYPE_PAYMENT_METHOD_INVALID);
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                //SimplifiedInvoice

                if (pParameters.DocumentType == DocumentSettings.SimplifiedInvoiceId)
                {
                    //Check if SimplifiedInvoice is Service Total > MaxTotalServices
                    if (pParameters.ArticleBag.GetClassTotals("S") > financeRuleSimplifiedInvoiceMaxTotalServices)
                    {
                        ResultAdd(DocumentValidationErrorType.ERROR_RULE_DOCUMENT_FINANCE_TYPE_SIMPLIFIED_INVOICE_MAX_TOTAL_SERVICE_EXCEED);
                    }

                    //Check if SimplifiedInvoice Total > MaxValue 
                    if (pParameters.ArticleBag.TotalFinal > financeRuleSimplifiedInvoiceMaxTotal)
                    {
                        ResultAdd(DocumentValidationErrorType.ERROR_RULE_DOCUMENT_FINANCE_TYPE_SIMPLIFIED_INVOICE_MAX_TOTAL_EXCEED);
                    }
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                //ParentDocuments

                //Get Valid Parent Documents
                Guid[] validParentDocuments = DocumentProcessingUtils.GetDocumentTypeValidSourceDocuments(pParameters.DocumentType);

                //If has a ParentDocument, Check if is a Valid One for current document type, else trigger Error
                if (documentParent != null && !validParentDocuments.Contains<Guid>(documentParent.DocumentType.Oid))
                {
                    //If Moçambique Ignore - Moçambique - Pedidos da reunião 13/10/2020 [IN:014327]
                    if (CultureSettings.MozambiqueCountryId.Equals(XPOSettings.ConfigurationSystemCountry.Oid))
                    {
                        ResultAdd(DocumentValidationErrorType.ERROR_RULE_DOCUMENT_FINANCE_TYPE_PARENT_DOCUMENT_INVALID);
                    }

                }

                //ParentDocuments: Credit Note
                if (pParameters.DocumentType == CustomDocumentSettings.CreditNoteId)
                {
                    if (documentParent == null)
                    {
                        //Require to Check if CreditNote has the Required ParentDocument added from back validations, if not and Add it
                        if (!_resultEnum.ContainsKey(DocumentValidationErrorType.ERROR_RULE_DOCUMENT_FINANCE_TYPE_PARENT_DOCUMENT_INVALID))
                        {
                            ResultAdd(DocumentValidationErrorType.ERROR_RULE_DOCUMENT_FINANCE_TYPE_PARENT_DOCUMENT_INVALID);
                        }
                    }
                    else
                    {
                        //Check if all Articles has a Reason Defined, if not trigger Error
                        bool hasArticlesWithoutReference = false;
                        bool hasArticlesWithoutReason = false;
                        //If try, detects a CreditNote with a quantity greater than the (totalParentDocument - totalAlreadyCredited)
                        bool hasArticlesQuantityGreaterThanUnCredit = false;

                        if (pParameters.ArticleBag != null)
                        {
                            foreach (var item in pParameters.ArticleBag)
                            {
                                //Check if has articles without required reference
                                if (item.Value.Reference == null)
                                {
                                    hasArticlesWithoutReference = true;
                                }
                                //Chedcfk if has articles without required Reason
                                if (string.IsNullOrEmpty(item.Value.Reason))
                                {
                                    hasArticlesWithoutReason = true;
                                }
                            }
                        }

                        if (DocumentProcessingUtils.GetCreditNoteValidation(documentParent, pParameters.ArticleBag))
                        {
                            hasArticlesQuantityGreaterThanUnCredit = true;
                        }

                        if (hasArticlesWithoutReference) { ResultAdd(DocumentValidationErrorType.ERROR_RULE_DOCUMENT_FINANCE_TYPE_CREDIT_NOTE_PARENT_DOCUMENT_ARTICLE_REFERENCE_INVALID); }
                        if (hasArticlesWithoutReason) { ResultAdd(DocumentValidationErrorType.ERROR_RULE_DOCUMENT_FINANCE_TYPE_CREDIT_NOTE_PARENT_DOCUMENT_ARTICLE_REASON_INVALID); }
                        if (hasArticlesQuantityGreaterThanUnCredit) { ResultAdd(DocumentValidationErrorType.ERROR_RULE_DOCUMENT_FINANCE_TYPE_CREDIT_NOTE_ARTICLEBAG_ARTICLES_CREDITED_DETECTED); }
                    }
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                //ParentDocument : Customer <> ParentDocument Customer

                if (documentType != null && customer != null && customer.Oid == InvoiceSettings.FinalConsumerId
                    && (
                        documentType.Oid != InvoiceSettings.InvoiceId
                        && documentType.Oid != DocumentSettings.SimplifiedInvoiceId
                        && documentType.Oid != DocumentSettings.XpoOidDocumentFinanceTypeConferenceDocument
                        )
                    )
                {
                    //Has an Exception to base Rule, is Valid if is Derived from a Parent Document 
                    if (documentParent != null && documentParent.EntityOid != InvoiceSettings.FinalConsumerId)
                    {
                        ResultAdd(DocumentValidationErrorType.ERROR_RULE_DOCUMENT_FINANCE_TYPE_WITH_FINAL_CONSUMER_INVALID);
                    }
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //DocumentType: Invoice|InvoiceAndPayment|Simplified Invoice cant emmit documents with FinalConsumer, or User with miss details above FinanceRuleRequiredCustomerDetailsAboveValue (1000)
                if (documentType != null && customer != null
                    &&
                    (
                        documentType.Oid == InvoiceSettings.InvoiceId ||
                        documentType.Oid == DocumentSettings.InvoiceAndPaymentId ||
                        documentType.Oid == DocumentSettings.SimplifiedInvoiceId
                    )
                    &&
                    DocumentProcessingUtils.IsInValidFinanceDocumentCustomer(
                        pParameters.ArticleBag.TotalFinal,
                        customer.Name,
                        customer.Address,
                        customer.ZipCode,
                        customer.City,
                        customer.Country.Designation,
                        customer.FiscalNumber
                    )
                )
                {
                    ResultAdd(DocumentValidationErrorType.ERROR_RULE_DOCUMENT_FINANCE_TYPE_WITH_CUSTOMER_DETAILS_INVALID);
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                //Customer Card: Amount Invalid and Recharge Card Invalid

                if (!DocumentProcessingUtils.IsCustomerCardValidForArticleBag(pParameters.ArticleBag, customer))
                {
                    ResultAdd(DocumentValidationErrorType.ERROR_RULE_CUSTOMER_CARD_RECHARGE_CARD_ARTICLE_DETECTED_WITH_CUSTOMER_CARD_INVALID);
                }

                if (configurationPaymentMethod != null
                    && configurationPaymentMethod.Token == "CUSTOMER_CARD"
                    && pParameters.ArticleBag.TotalFinal > customer.CardCredit
                )
                {
                    ResultAdd(DocumentValidationErrorType.ERROR_RULE_CUSTOMER_CARD_AMOUNT_INVALID);
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                //ParentDocument : ParentDocument > CurrentDocument

                //Removed, Give Problems with DocumentConferences dates for few seconds (GetOrderMainLastDocumentConference)
                //if (documentParent != null && documentParent.Date > pParameters.DocumentDateTime)
                //{
                //    ResultAdd(FinanceValidationError.ERROR_RULE_PARENT_DOCUMENT_DATE_AND_CURRENT_DOCUMENT_DATE_INVALID);
                //}

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Page2 
                //Check Fill all Details :SettingsApp.FinanceRuleRequiredCustomerDetailsAboveValue

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Add FieldErrors
                GetFieldErrors();

                //Add FieldErrors
                GetFieldArticleErrors(documentType, pParameters.ArticleBag);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return _resultEnum;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Process Payments/Recibos

        public static SortedDictionary<DocumentValidationErrorType, object> ValidatePersistFinanceDocumentPayment(List<fin_documentfinancemaster> pInvoices, List<fin_documentfinancemaster> pCreditNotes, Guid pCustomer, Guid pPaymentMethod, Guid pConfigurationCurrency, decimal pPaymentAmount, string pPaymentNotes = "")
        {
            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //Init Common Variables

            _resultEnum = new SortedDictionary<DocumentValidationErrorType, object>();
            _fields = new Dictionary<DocumentValidationErrorType, DocumentProcessingValidationField>();

            //Disable to go to AT or to Release
            if (!Debugger.IsAttached) return _resultEnum;

            try
            {
                //Get XPGuidObjects from Parameters
                erp_customer customer = (pCustomer != null && pCustomer != Guid.Empty)
                    ? (erp_customer)XPOUtility.GetXPGuidObject(XPOSettings.Session, typeof(erp_customer), pCustomer)
                    : null;
                fin_configurationpaymentmethod paymentMethod = (pPaymentMethod != null && pPaymentMethod != Guid.Empty)
                    ? (fin_configurationpaymentmethod)XPOUtility.GetXPGuidObject(XPOSettings.Session, typeof(fin_configurationpaymentmethod), pPaymentMethod)
                    : null;
                cfg_configurationcurrency currency = (pConfigurationCurrency != null && pConfigurationCurrency != Guid.Empty)
                    ? (cfg_configurationcurrency)XPOUtility.GetXPGuidObject(XPOSettings.Session, typeof(cfg_configurationcurrency), pConfigurationCurrency)
                    : null;

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                // Process Field Validation
                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                _fields.Add(DocumentValidationErrorType.ERROR_FIELD_CUSTOMER_INVALID,
                    new DocumentProcessingValidationField("Customer", customer.Oid, LogicPOS.Utility.RegexUtils.RegexGuid, true)
                );
                _fields.Add(DocumentValidationErrorType.ERROR_FIELD_PAYMENT_METHOD_INVALID,
                    new DocumentProcessingValidationField("PaymentMethod", paymentMethod.Oid, LogicPOS.Utility.RegexUtils.RegexGuid, true)
                );
                _fields.Add(DocumentValidationErrorType.ERROR_FIELD_CURRENCY_INVALID,
                    new DocumentProcessingValidationField("Currency", currency.Oid, LogicPOS.Utility.RegexUtils.RegexGuid, true)
                );
                _fields.Add(DocumentValidationErrorType.ERROR_FIELD_PAYMENT_AMOUNT_INVALID,
                    new DocumentProcessingValidationField("PaymentAmount", pPaymentAmount, LogicPOS.Utility.RegexUtils.RegexDecimalGreaterEqualThanZero, true)
                );
                _fields.Add(DocumentValidationErrorType.ERROR_FIELD_NOTES_INVALID,
                    new DocumentProcessingValidationField("Notes", pPaymentNotes, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false)
                );

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::            
                // Validation Rules : Valid Documents
                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Validate Invoice Documents
                List<Guid> validCustomerPaymentInvoices = DocumentProcessingUtils.GetValidDocumentsForPayment(customer.Oid, InvoiceSettings.InvoiceId);
                foreach (var item in pInvoices)
                {
                    if (!validCustomerPaymentInvoices.Contains(item.Oid)
                        && !_resultEnum.ContainsKey(DocumentValidationErrorType.ERROR_RULE_DOCUMENT_FINANCE_INVOICE_DOCUMENTS_DETECTED)
                    )
                    {
                        ResultAdd(DocumentValidationErrorType.ERROR_RULE_DOCUMENT_FINANCE_INVOICE_DOCUMENTS_DETECTED);
                    }
                }

                //Validate Invoice Documents
                List<Guid> validCustomerPaymentCreditNotes = DocumentProcessingUtils.GetValidDocumentsForPayment(customer.Oid, CustomDocumentSettings.CreditNoteId);
                foreach (var item in pCreditNotes)
                {
                    if (!validCustomerPaymentCreditNotes.Contains(item.Oid)
                        && !_resultEnum.ContainsKey(DocumentValidationErrorType.ERROR_RULE_DOCUMENT_FINANCE_CREDIT_NOTE_DOCUMENTS_DETECTED)
                    )
                    {
                        ResultAdd(DocumentValidationErrorType.ERROR_RULE_DOCUMENT_FINANCE_CREDIT_NOTE_DOCUMENTS_DETECTED);
                    }
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::            
                // Validation Rules : Valid Documents
                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                object sqlResult;
                string sqlInvoices = string.Empty;
                string sqlInvoicesBase = "SELECT fmaTotalFinal - SUM(fmpCreditAmount) as Result FROM view_documentfinancepayment WHERE fmaOid = '{0}' AND fpaPaymentStatus <> 'A' GROUP BY fmaOid, fmaTotalFinal;";
                decimal totalDocumentsDiference = 0.0m;
                decimal totalPaymentDiference = 0.0m;
                decimal totalInvoicesDebit = 0.0m;
                decimal totalCreditNotes = 0.0m;
                bool hasDocumentsDiferentFromTargetCustomer = false;
                //decimal exchangeRate = (currency.ExchangeRate < 1 || currency.ExchangeRate > 1) ? currency.ExchangeRate : 1;
                decimal paymentAmount = pPaymentAmount;//*exchangeRate;

                //Check Invoices
                bool hasInvoicePayed = false;
                bool hasInvoiceCancelled = false;
                foreach (var item in pInvoices)
                {
                    if (item.Payed) hasInvoicePayed = true;
                    if (item.DocumentStatusStatus == "A") hasInvoiceCancelled = true;
                    //Check Customer
                    if (customer.Oid != item.EntityOid) hasDocumentsDiferentFromTargetCustomer = true;
                    //TotalInvoicesDebit
                    sqlInvoices = string.Format(sqlInvoicesBase, item.Oid);
                    sqlResult = XPOSettings.Session.ExecuteScalar(sqlInvoices);
                    totalInvoicesDebit += (sqlResult != null) ? Convert.ToDecimal(sqlResult) : item.TotalFinal;
                }

                //Check CreditNotes
                bool hasCreditNotePayed = false;
                bool hasCreditNoteCancelled = false;
                foreach (var item in pCreditNotes)
                {
                    if (item.Payed) hasCreditNotePayed = true;
                    if (item.DocumentStatusStatus == "A") hasCreditNoteCancelled = true;
                    //Check Customer
                    if (customer.Oid != item.EntityOid) hasDocumentsDiferentFromTargetCustomer = true;
                    //TotalCreditNotes
                    totalCreditNotes += item.TotalFinal;
                }

                //Calc Diferences
                totalDocumentsDiference = (totalInvoicesDebit - totalCreditNotes);
                totalPaymentDiference = Math.Round(totalDocumentsDiference - paymentAmount, LogicPOS.Settings.CultureSettings.DecimalRoundTo);

                if (_debug) _logger.Debug(string.Format(
                    "PaymentAmount: [{0}], InvoicesDebit: [{1}], CreditNotes: [{2}], DocumentsDiference: [{3}], PaymentDiference:[{4}]",
                    paymentAmount, totalInvoicesDebit, totalCreditNotes, totalDocumentsDiference, totalPaymentDiference)
                );

                //Invoices
                if (hasInvoicePayed) { ResultAdd(DocumentValidationErrorType.ERROR_RULE_DOCUMENT_FINANCE_TYPE_INVOICE_PAYED_DETECTED); }
                if (hasInvoiceCancelled) { ResultAdd(DocumentValidationErrorType.ERROR_RULE_DOCUMENT_FINANCE_TYPE_INVOICE_CANCELLED_DETECTED); }
                //Credit Notes
                if (hasCreditNotePayed) { ResultAdd(DocumentValidationErrorType.ERROR_RULE_DOCUMENT_FINANCE_TYPE_CREDIT_NOTE_PAYED_DETECTED); }
                if (hasCreditNoteCancelled) { ResultAdd(DocumentValidationErrorType.ERROR_RULE_DOCUMENT_FINANCE_TYPE_CREDIT_NOTE_CANCELLED_DETECTED); }
                if (hasDocumentsDiferentFromTargetCustomer) { ResultAdd(DocumentValidationErrorType.ERROR_RULE_PAYMENT_DOCUMENTS_WITH_DIFERENT_CUSTOMERS_DETECTED); }
                //Check Valid Payment : Diference must be greater or equal to zero
                if (totalPaymentDiference < 0) { ResultAdd(DocumentValidationErrorType.ERROR_RULE_PAYMENT_AMOUNT_GREATER_THAN_TOTAL_SETTLE_IN_DOCUMENTS_DETECTED); }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Add FieldErrors
                GetFieldErrors();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return (_resultEnum);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Helper Methods

        private static void ResultAdd(DocumentValidationErrorType pTokenEnum)
        {
            ResultAdd(pTokenEnum, null);
        }

        private static void ResultAdd(DocumentValidationErrorType pTokenEnum, object pObject)
        {

            //Add
            _resultEnum.Add(pTokenEnum, pObject);
            //String Value
            //value = Enum.GetName(typeof(FinanceValidationError), _resultEnum[_resultEnum.Count - 1]).ToUpper();
            string value = Enum.GetName(typeof(DocumentValidationErrorType), pTokenEnum).ToUpper();
            if (_debug) _logger.Debug(value);
        }

        private static void GetFieldErrors()
        {

            //Loop Validation Fields Rules
            foreach (var item in _fields)
            {
                string value;
                Type type;
                //Call Validate
                if (item.Value.Value != null)
                {
                    value = item.Value.Value.ToString();
                    type = item.Value.Value.GetType();
                }
                else
                {
                    value = string.Empty;
                    type = null;
                }

                //Use this to Debug individual FinanceValidationError Validations
                if (
                    item.Key == DocumentValidationErrorType.ERROR_FIELD_ARTICLE_PRICE_INVALID
                    //|| item.Key == FinanceValidationError.ERROR_FIELD_SHIPFROM_DELIVERYDATE_INVALID
                    )
                {
                    _logger.Debug("BREAKPOINT");
                }

                bool result = GeneralUtils.Validate(value, item.Value.Rule, item.Value.Required);

                //Extra Validation for types that have more than RegEx Requirements
                if (result && item.Key == DocumentValidationErrorType.ERROR_FIELD_CUSTOMER_FISCAL_NUMBER_INVALID)
                {
                    result = FiscalNumberUtils.IsValidFiscalNumber(value, _countryCode2);
                }

                if (!result)
                {
                    ResultAdd(item.Key);
                    if (_debug) _logger.Debug(string.Format("Key: [{0}], Name: [{1}], Value: [{2}], Type: [{3}], Rule: [{4}], Required: [{5}]", item.Key, item.Value.Name, value, (type != null) ? type.ToString() : "NULL", item.Value.Rule, item.Value.Required));
                }
            }
        }

        private static void GetFieldArticleErrors(fin_documentfinancetype pDocumentType, ArticleBag pArticleBag)
        {
            //Field Validation
            bool hasArticlesWithInvalidOid = false;
            bool hasArticlesWithInvalidCode = false;
            bool hasArticlesWithInvalidDesignation = false;
            bool hasArticlesWithInvalidPrice = false;
            bool hasArticlesWithInvalidQuantity = false;
            //bool hasArticlesWithInvalidUnitMeasureAcronym = false;
            bool hasArticlesWithInvalidDiscount = false;
            bool hasArticlesWithInvalidVat = false;
            bool hasArticlesWithInvalidVatExemptionReason = false;
            //Rules Validation
            //ConsignationInvoice
            bool hasArticlesWithoutVatRateDutyFree = false;
            bool hasArticlesWithoutVatExemptionReasonM99 = false;
            //Articles Without TaxExemptionReason Detected
            bool hasArticlesWithoutRequiredTaxExemptionReason = false;
            //Store ValidationField References
            DocumentProcessingValidationField validationFieldOid;
            DocumentProcessingValidationField validationFieldCode;
            DocumentProcessingValidationField validationFieldDesignation;
            DocumentProcessingValidationField validationFieldPrice;
            DocumentProcessingValidationField validationFieldQuantity;
            //ProcessFinanceDocumentValidationField validationFieldUnitMeasureAcronym;
            DocumentProcessingValidationField validationFieldDiscount;
            DocumentProcessingValidationField validationFieldVat;
            DocumentProcessingValidationField validationFieldVatExemptionReason;

            //Loop ArticleBag
            if (pArticleBag != null)
            {
                foreach (var item in pArticleBag)
                {
                    //Get ValidationFields references from Dictionary
                    validationFieldOid = _fieldsArticle[DocumentValidationErrorType.ERROR_FIELD_ARTICLE_OID_INVALID];
                    validationFieldCode = _fieldsArticle[DocumentValidationErrorType.ERROR_FIELD_ARTICLE_CODE_INVALID];
                    validationFieldDesignation = _fieldsArticle[DocumentValidationErrorType.ERROR_FIELD_ARTICLE_DESIGNATION_INVALID];
                    validationFieldPrice = _fieldsArticle[DocumentValidationErrorType.ERROR_FIELD_ARTICLE_PRICE_INVALID];
                    validationFieldQuantity = _fieldsArticle[DocumentValidationErrorType.ERROR_FIELD_ARTICLE_QUANTITY_INVALID];
                    //Removed : Framework LogicErp dont send ACRONYM : Search all ERROR_FIELD_ARTICLE_UNIT_MEASURE_ACRONYM_INVALID occurences
                    //validationFieldUnitMeasureAcronym = _fieldsArticle[FinanceValidationError.ERROR_FIELD_ARTICLE_UNIT_MEASURE_ACRONYM_INVALID];
                    validationFieldDiscount = _fieldsArticle[DocumentValidationErrorType.ERROR_FIELD_ARTICLE_DISCOUNT_INVALID];
                    validationFieldVat = _fieldsArticle[DocumentValidationErrorType.ERROR_FIELD_ARTICLE_VAT_RATE_INVALID];
                    validationFieldVatExemptionReason = _fieldsArticle[DocumentValidationErrorType.ERROR_FIELD_ARTICLE_VAT_EXEMPTION_REASON_INVALID];
                    //Assign current article values to Validation Rules References (Optional), this way code is cleaner, all properties are in ProcessFinanceDocumentValidationField
                    validationFieldOid.Value = item.Key.ArticleId;
                    validationFieldCode.Value = item.Value.Code;
                    validationFieldDesignation.Value = item.Key.Designation;
                    validationFieldPrice.Value = validationFieldPrice.Value = item.Key.Price;
                    validationFieldQuantity.Value = item.Value.Quantity;
                    //Removed : Framework LogicErp dont send ACRONYM : Search all ERROR_FIELD_ARTICLE_UNIT_MEASURE_ACRONYM_INVALID occurences
                    //validationFieldUnitMeasureAcronym.Value = item.Value.UnitMeasure;
                    validationFieldDiscount.Value = item.Key.Discount;
                    validationFieldVat.Value = item.Key.Vat;
                    validationFieldVatExemptionReason.Value = item.Key.VatExemptionReasonOid;
                    //Validate ValidationField: Oid
                    if (!GeneralUtils.Validate(validationFieldOid.Value.ToString(), validationFieldOid.Rule, validationFieldOid.Required))
                    {
                        hasArticlesWithInvalidOid = true;
                    }
                    //Validate ValidationField: Code
                    if (!GeneralUtils.Validate(validationFieldCode.Value.ToString(), validationFieldCode.Rule, validationFieldCode.Required))
                    {
                        hasArticlesWithInvalidCode = true;
                    }
                    //Validate ValidationField: Designation
                    if (!GeneralUtils.Validate(validationFieldDesignation.Value.ToString(), validationFieldDesignation.Rule, validationFieldDesignation.Required))
                    {
                        hasArticlesWithInvalidDesignation = true;
                    }
                    //Validate ValidationField: Price : Ignore Validation in ArticleSettlement Articles (Can have negative Values)
                    //if (!item.Key.ArticleOid.Equals(SettingsApp.XpoOidArticleSettlement) && !FrameworkUtils.Validate(validationFieldPrice.Value.ToString(), validationFieldPrice.Rule, validationFieldPrice.Required))
                    //{
                    //    hasArticlesWithInvalidPrice = true;
                    //}
                    //Validate ValidationField: Quantity
                    //if (!FrameworkUtils.Validate(validationFieldQuantity.Value.ToString(), validationFieldQuantity.Rule, validationFieldQuantity.Required))
                    //{
                    //    hasArticlesWithInvalidQuantity = true;
                    //}
                    //Removed : Framework LogicErp dont send ACRONYM : Search all ERROR_FIELD_ARTICLE_UNIT_MEASURE_ACRONYM_INVALID occurences
                    //if (!FrameworkUtils.Validate(validationFieldUnitMeasureAcronym.Value.ToString(), validationFieldUnitMeasureAcronym.Rule, validationFieldUnitMeasureAcronym.Required))
                    //{
                    //    hasArticlesWithInvalidUnitMeasureAcronym = true;
                    //}
                    //Validate ValidationField: Discount
                    if (!GeneralUtils.Validate(validationFieldDiscount.Value.ToString(), validationFieldDiscount.Rule, validationFieldDiscount.Required))
                    {
                        hasArticlesWithInvalidDiscount = true;
                    }
                    //Validate ValidationField: Vat
                    if (!GeneralUtils.Validate(validationFieldVat.Value.ToString(), validationFieldVat.Rule, validationFieldVat.Required))
                    {
                        hasArticlesWithInvalidVat = true;
                    }
                    //Validate ValidationField: VatExemptionReason
                    if (!GeneralUtils.Validate(validationFieldVatExemptionReason.Value.ToString(), validationFieldVatExemptionReason.Rule, (item.Key.Vat == 0.0m)))
                    {
                        hasArticlesWithInvalidVatExemptionReason = true;
                    }

                    //Rules Validation

                    //If is a ConsignationInvoice all Articles must have VatRateDutyFree Vat
                    if (pDocumentType.Oid == DocumentSettings.ConsignationInvoiceId)
                    {
                        if (item.Key.Vat != 0.0m)
                        {
                            hasArticlesWithInvalidVat = true;
                            hasArticlesWithoutVatRateDutyFree = true;
                        }

                        if (item.Key.VatExemptionReasonOid != InvoiceSettings.XpoOidConfigurationVatExemptionReasonM99)
                        {
                            hasArticlesWithInvalidVatExemptionReason = true;
                            hasArticlesWithoutVatExemptionReasonM99 = true;
                        }
                    }

                    //Detect Articles without Required TaxExceptionReason
                    if (item.Key.Vat == 0.0m && item.Key.VatExemptionReasonOid == Guid.Empty)
                    {
                        hasArticlesWithoutRequiredTaxExemptionReason = true;
                    }
                }

                //Trigger Errors if Detected
                if (hasArticlesWithInvalidOid) ResultAdd(DocumentValidationErrorType.ERROR_FIELD_ARTICLE_OID_INVALID);
                if (hasArticlesWithInvalidCode) ResultAdd(DocumentValidationErrorType.ERROR_FIELD_ARTICLE_CODE_INVALID);
                if (hasArticlesWithInvalidDesignation) ResultAdd(DocumentValidationErrorType.ERROR_FIELD_ARTICLE_DESIGNATION_INVALID);
                if (hasArticlesWithInvalidPrice) ResultAdd(DocumentValidationErrorType.ERROR_FIELD_ARTICLE_PRICE_INVALID);
                if (hasArticlesWithInvalidQuantity) ResultAdd(DocumentValidationErrorType.ERROR_FIELD_ARTICLE_QUANTITY_INVALID);
                //Removed : Framework LogicErp dont send ACRONYM : Search all ERROR_FIELD_ARTICLE_UNIT_MEASURE_ACRONYM_INVALID occurences
                //if (hasArticlesWithInvalidUnitMeasureAcronym) ResultAdd(FinanceValidationError.ERROR_FIELD_ARTICLE_UNIT_MEASURE_ACRONYM_INVALID);
                if (hasArticlesWithInvalidDiscount) ResultAdd(DocumentValidationErrorType.ERROR_FIELD_ARTICLE_DISCOUNT_INVALID);
                if (_requireToChooseVatExemptionReason && hasArticlesWithInvalidVat) ResultAdd(DocumentValidationErrorType.ERROR_FIELD_ARTICLE_VAT_RATE_INVALID);
                if (_requireToChooseVatExemptionReason && hasArticlesWithInvalidVatExemptionReason) ResultAdd(DocumentValidationErrorType.ERROR_FIELD_ARTICLE_VAT_EXEMPTION_REASON_INVALID);
                //Specific Rule
                if (hasArticlesWithoutVatRateDutyFree) ResultAdd(DocumentValidationErrorType.ERROR_RULE_ARTICLEBAG_ARTICLES_WITHOUT_TAX_DUTY_FREE_DETECTED);
                if (hasArticlesWithoutVatExemptionReasonM99) ResultAdd(DocumentValidationErrorType.ERROR_RULE_ARTICLEBAG_ARTICLES_WITHOUT_TAX_EXCEPTION_REASON_M99_DETECTED);
                if (_requireToChooseVatExemptionReason && hasArticlesWithoutRequiredTaxExemptionReason) ResultAdd(DocumentValidationErrorType.ERROR_RULE_ARTICLEBAG_ARTICLES_WITHOUT_TAX_EXCEPTION_REASON_DETECTED);
            }
        }
    }
}
