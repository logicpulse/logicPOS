using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.financial.library.App;
using logicpos.shared.Classes.Finance;
using logicpos.shared.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace logicpos.financial.library.Classes.Finance
{
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    // Finance Articles

    //Artigo 36.º Prazo de emissão e formalidades das facturas
    //http://info.portaldasfinancas.gov.pt/pt/informacao_fiscal/codigos_tributarios/civa_rep/iva36.htm
    //36.15 - A indicação na fatura da identificação e do domicílio do adquirente ou destinatário que não seja sujeito passivo 
    //        não é obrigatória nas faturas de valor inferior a (euro) 1000, salvo quando o adquirente ou destinatário solicite que a fatura contenha esses elementos. 

    //Artigo 40.º Faturas simplificadas 
    //http://info.portaldasfinancas.gov.pt/pt/informacao_fiscal/codigos_tributarios/civa_rep/iva40.htm
    //40.1.a) Transmissões de bens efetuadas por retalhistas ou vendedores ambulantes a não sujeitos passivos, 
    //        quando o valor da fatura não for superior a (euro) 1000; 
    //40.1.b) Outras transmissões de bens e prestações de serviços em que o montante da fatura não seja superior a (euro) 100. 

    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //Tests

    // [X] Simplified Invoices : with Total Value greater than FinanceRuleSimplifiedInvoiceMaxValue
    // [X] Non Simplified Invoice : With XpoOidDocumentFinanceMasterFinalConsumerEntity, only SimplifiedInvoices use FinalConsumer
    // [X] Valid Checksum FiscalNumber
    // [X] All Documents : Filled Name or NIF in all Documents
    // [X] All Documents : Filled All Customer Details when Total exceed FinanceRuleRequiredCustomerDetailsAboveValue
    // [X] Simplified Invoices with all Filled Customer details (Non SingularEntity)
    // [X] Documents with Required PaymentCondition without it
    // [X] Documents with Required PaymentMethod without it
    // [X] Documents with Required Parent Document without it and without Article Reason
    // [X] Documents with ExchangeRate
    // [X] Documents with valid ParentDocuments Types ex CreditNote > Invoice etc
    // [X] Empty ArticleBag
    // [X] Articles with TaxException Reason without Reason/Motive
    // [X] Articles Field Validation like Fields (Loop Articles and Return Error ex)
    // [X] Documents FinanceTypeConsignationInvoice required all Articles with DutyFree Tax and Reason M99
    // [X] Articles with Negative Prices
    // [X] Credit Note
    // [X] Discounts < 0 or > 100
    //     [X] ArticleBagKey.Discount (ArticleBag Discount)
    //     [X] Customer.Discount
    //     [X] ArticleBag.DiscountGlobal

    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //Inner Enum FinanceValidationError

    public enum FinanceValidationError
    {
        //Warnings
        WARNING_RULE_SYSTEM_DATE_GLOBAL,
        WARNING_RULE_SYSTEM_DATE_SERIE,
        //Global/Generic
        ERROR,
        //Errors
        ERROR_RULE_LOGGED_USER_INVALID,
        ERROR_RULE_LOGGED_TERMINAL_INVALID,
        //SourceMode
        ERROR_RULE_SOURCE_MODE_ORDERMAIN_EMPTY,
        ERROR_RULE_SOURCE_MODE_CUSTOM_ARTICLEBAG_EMPTY,
        ERROR_RULE_SOURCE_MODE_FINANCE_DOCUMENTS_EMPTY,
        //Series
        ERROR_RULE_SERIE_INVALID,
        //DocumentType
        ERROR_RULE_DOCUMENT_FINANCE_TYPE_INVALID,
        ERROR_RULE_DOCUMENT_FINANCE_TYPE_PAYMENT_CONDITION_INVALID,
        ERROR_RULE_DOCUMENT_FINANCE_TYPE_PAYMENT_METHOD_INVALID,
        ERROR_RULE_DOCUMENT_FINANCE_TYPE_PARENT_DOCUMENT_INVALID,
        ERROR_RULE_DOCUMENT_FINANCE_TYPE_WITH_FINAL_CONSUMER_INVALID,
        ERROR_RULE_DOCUMENT_FINANCE_TYPE_WITH_CUSTOMER_DETAILS_INVALID,
        //ParentDocument
        //ERROR_RULE_PARENT_DOCUMENT_DATE_AND_CURRENT_DOCUMENT_DATE_INVALID,
        ERROR_RULE_PARENT_DOCUMENT_CUSTOMER_AND_CURRENT_DOCUMENT_CUSTOMER_INVALID,
        ERROR_RULE_PARENT_DOCUMENT_CANCELLED_DETECTED,
        //DocumentType Simplified Invoice
        ERROR_RULE_DOCUMENT_FINANCE_TYPE_SIMPLIFIED_INVOICE_MAX_TOTAL_EXCEED,
        ERROR_RULE_DOCUMENT_FINANCE_TYPE_SIMPLIFIED_INVOICE_MAX_TOTAL_SERVICE_EXCEED,
        //DocumentType CreditNote
        ERROR_RULE_DOCUMENT_FINANCE_TYPE_CREDIT_NOTE_PARENT_DOCUMENT_ARTICLE_REFERENCE_INVALID,
        ERROR_RULE_DOCUMENT_FINANCE_TYPE_CREDIT_NOTE_PARENT_DOCUMENT_ARTICLE_REASON_INVALID,
        ERROR_RULE_DOCUMENT_FINANCE_TYPE_CREDIT_NOTE_ARTICLEBAG_ARTICLES_CREDITED_DETECTED,
        //DocumentType ConsignationInvoice, All Articles must be DutyFree and Have a Reason M99
        ERROR_RULE_ARTICLEBAG_ARTICLES_WITHOUT_TAX_DUTY_FREE_DETECTED,
        ERROR_RULE_ARTICLEBAG_ARTICLES_WITHOUT_TAX_EXCEPTION_REASON_M99_DETECTED,
        //ArticleBag
        ERROR_RULE_ARTICLEBAG_ARTICLES_WITHOUT_TAX_EXCEPTION_REASON_DETECTED,
        ERROR_RULE_ARTICLEBAG_GLOBAL_DISCOUNT_INVALID,
        //Validation Fields : Customer Card
        ERROR_RULE_CUSTOMER_CARD_AMOUNT_INVALID,
        ERROR_RULE_CUSTOMER_CARD_RECHARGE_CARD_ARTICLE_DETECTED_WITH_CUSTOMER_CARD_INVALID,
        //Validation Fields:P1
        ERROR_FIELD_DOCUMENT_TYPE_INVALID,
        ERROR_FIELD_PAYMENT_CONDITION_INVALID,
        ERROR_FIELD_PAYMENT_METHOD_INVALID,
        ERROR_FIELD_CURRENCY_INVALID,
        ERROR_FIELD_DOCUMENT_PARENT_INVALID,
        ERROR_FIELD_NOTES_INVALID,
        //Validation Fields:P2
        ERROR_FIELD_CUSTOMER_NAME_INVALID,
        ERROR_FIELD_CUSTOMER_ADDRESS_INVALID,
        ERROR_FIELD_CUSTOMER_LOCALITY_INVALID,
        ERROR_FIELD_CUSTOMER_ZIPCODE_INVALID,
        ERROR_FIELD_CUSTOMER_CITY_INVALID,
        ERROR_FIELD_CUSTOMER_COUNTRY_INVALID,
        ERROR_FIELD_CUSTOMER_FISCAL_NUMBER_INVALID,
        ERROR_FIELD_CUSTOMER_CARDNUMBER_INVALID,
        ERROR_FIELD_CUSTOMER_DISCOUNT_INVALID,
        //Validation Fields:P3
        //Validation Fields:P4
        ERROR_FIELD_SHIPTO_ADDRESS_DETAIL_INVALID,
        ERROR_FIELD_SHIPTO_REGION_INVALID,
        ERROR_FIELD_SHIPTO_POSTALCODE_INVALID,
        ERROR_FIELD_SHIPTO_CITY_INVALID,
        ERROR_FIELD_SHIPTO_COUNTRY_INVALID,
        ERROR_FIELD_SHIPTO_DELIVERYDATE_INVALID,
        ERROR_FIELD_SHIPTO_DELIVERYID_INVALID,
        ERROR_FIELD_SHIPTO_WAREHOUSEID_INVALID,
        ERROR_FIELD_SHIPTO_LOCATIONID_INVALID,
        //Validation Fields:P5
        ERROR_FIELD_SHIPFROM_ADDRESS_DETAIL_INVALID,
        ERROR_FIELD_SHIPFROM_REGION_INVALID,
        ERROR_FIELD_SHIPFROM_POSTALCODE_INVALID,
        ERROR_FIELD_SHIPFROM_CITY_INVALID,
        ERROR_FIELD_SHIPFROM_COUNTRY_INVALID,
        ERROR_FIELD_SHIPFROM_DELIVERYDATE_INVALID,
        ERROR_FIELD_SHIPFROM_DELIVERYID_INVALID,
        ERROR_FIELD_SHIPFROM_WAREHOUSEID_INVALID,
        ERROR_FIELD_SHIPFROM_LOCATIONID_INVALID,
        //Validation Fields : Article
        ERROR_FIELD_ARTICLE_OID_INVALID,
        ERROR_FIELD_ARTICLE_CODE_INVALID,
        ERROR_FIELD_ARTICLE_DESIGNATION_INVALID,
        ERROR_FIELD_ARTICLE_PRICE_INVALID,
        ERROR_FIELD_ARTICLE_QUANTITY_INVALID,
        //Removed : Framework LogicErp dont send ACRONYM : Search all ERROR_FIELD_ARTICLE_UNIT_MEASURE_ACRONYM_INVALID occurences
        //ERROR_FIELD_ARTICLE_UNIT_MEASURE_ACRONYM_INVALID,
        ERROR_FIELD_ARTICLE_DISCOUNT_INVALID,
        ERROR_FIELD_ARTICLE_VAT_RATE_INVALID,
        ERROR_FIELD_ARTICLE_VAT_EXEMPTION_REASON_INVALID,

        //Payments

        //Fields
        ERROR_FIELD_CUSTOMER_INVALID,
        ERROR_FIELD_PAYMENT_AMOUNT_INVALID,
        //Rules
        //Try to Pay more than Debt ex Documents Debt = 100, PaymentAmount 101, Trigger Error
        ERROR_RULE_PAYMENT_AMOUNT_GREATER_THAN_TOTAL_SETTLE_IN_DOCUMENTS_DETECTED,
        ERROR_RULE_PAYMENT_DOCUMENTS_WITH_DIFERENT_CUSTOMERS_DETECTED,
        ERROR_RULE_DOCUMENT_FINANCE_TYPE_INVOICE_CANCELLED_DETECTED,
        ERROR_RULE_DOCUMENT_FINANCE_TYPE_CREDIT_NOTE_CANCELLED_DETECTED,
        ERROR_RULE_DOCUMENT_FINANCE_TYPE_INVOICE_PAYED_DETECTED,
        ERROR_RULE_DOCUMENT_FINANCE_TYPE_CREDIT_NOTE_PAYED_DETECTED,
        //Valid documents for Customer (Includes Invoices, CreditNotes etc)
        ERROR_RULE_DOCUMENT_FINANCE_INVOICE_DOCUMENTS_DETECTED,
        ERROR_RULE_DOCUMENT_FINANCE_CREDIT_NOTE_DOCUMENTS_DETECTED,
        ERROR_FIELD_SHIPFROM_DELIVERYDATE_BEFORE_SHIPPINGDATE
    }

    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //Inner Enum ProcessFinanceDocumentValidationField

    public class ProcessFinanceDocumentValidationField
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string _name;
        public string Name
        {
            get { return _name; }
        }
        private object _value;
        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }
        private string _rule;
        public string Rule
        {
            get { return _rule; }
        }
        private bool _required;
        public bool Required
        {
            get { return _required; }
        }
        private Action _action;
        public Action Action
        {
            get { return _action; }
        }

        public ProcessFinanceDocumentValidationField(string pName, object pValue, string pRule, bool pRequired)
            : this(pName, pValue, pRule, pRequired, null)
        {
        }

        public ProcessFinanceDocumentValidationField(string pName, object pValue, string pRule, bool pRequired, Action pAction)
        {
            _name = pName;
            _value = pValue;
            _rule = pRule;
            _required = pRequired;
            _action = pAction;
        }

        public bool Validate()
        {
            bool result = false;

            try
            {

            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }
    }

    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //Main Class ProcessFinanceDocumentValidation

    public class ProcessFinanceDocumentValidation
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

#if (DEBUG)
        private static bool _debug = true;
#else
        private static bool _debug = false;
#endif
        private static string _countryCode2 = string.Empty;
        private static SortedDictionary<FinanceValidationError, object> _resultEnum;
        private static Dictionary<FinanceValidationError, ProcessFinanceDocumentValidationField> _fields;
        private static Dictionary<FinanceValidationError, ProcessFinanceDocumentValidationField> _fieldsArticle;
        private static bool _requireToChooseVatExemptionReason = true;

        //Constructors
        public static SortedDictionary<FinanceValidationError, object> ValidatePersistFinanceDocument(ProcessFinanceDocumentParameter pParameters, bool pIgnoreWarning = false)
        {
            Guid userDetailGuid = (GlobalFramework.LoggedUser != null) ? GlobalFramework.LoggedUser.Oid : Guid.Empty;
            return ValidatePersistFinanceDocument(pParameters, userDetailGuid, pIgnoreWarning);
        }

        public static SortedDictionary<FinanceValidationError, object> ValidatePersistFinanceDocument(ProcessFinanceDocumentParameter pParameters, Guid pLoggedUser, bool pIgnoreWarning = false)
        {
            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //Init Common Variables

            _resultEnum = new SortedDictionary<FinanceValidationError, object>();
            _fields = new Dictionary<FinanceValidationError, ProcessFinanceDocumentValidationField>();
            _fieldsArticle = new Dictionary<FinanceValidationError, ProcessFinanceDocumentValidationField>();

            try
            {
                // Override Default with Config Value
                _requireToChooseVatExemptionReason = Convert.ToBoolean(GlobalFramework.Settings["requireToChooseVatExemptionReason"]);
            }
            catch (Exception)
            {
                _log.Error("Error Missing Config Parameter Key: [requireToChooseVatExemptionReason]");
            }


            //Disable to go to AT or to Release
            if (!Debugger.IsAttached) return _resultEnum;

            try
            {
                //Get XPGuidObjects from Parameters
                fin_documentfinancetype documentType = (pParameters.DocumentType != null && pParameters.DocumentType != Guid.Empty)
                    ? (fin_documentfinancetype)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(fin_documentfinancetype), pParameters.DocumentType)
                    : null;
                erp_customer customer = (pParameters.Customer != null && pParameters.Customer != Guid.Empty)
                    ? (erp_customer)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(erp_customer), pParameters.Customer)
                    : null;
                fin_documentfinancemaster documentParent = (pParameters.DocumentParent != null && pParameters.DocumentParent != Guid.Empty)
                    ? (fin_documentfinancemaster)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(fin_documentfinancemaster), pParameters.DocumentParent)
                    : null;
                erp_customer customerParentDocument = (documentParent != null && documentParent.EntityOid != Guid.Empty)
                    ? (erp_customer)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(erp_customer), documentParent.EntityOid)
                    : null;
                cfg_configurationcurrency configurationCurrency = (pParameters.Currency != null && pParameters.Currency != Guid.Empty)
                    ? (cfg_configurationcurrency)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(cfg_configurationcurrency), pParameters.Currency)
                    : null;
                cfg_configurationcountry countryShipTo = (pParameters.ShipTo != null && pParameters.ShipTo.CountryGuid != Guid.Empty)
                    ? (cfg_configurationcountry)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(cfg_configurationcountry), pParameters.ShipTo.CountryGuid)
                    : null;
                cfg_configurationcountry countryShipFrom = (pParameters.ShipFrom != null && pParameters.ShipFrom.CountryGuid != Guid.Empty)
                    ? (cfg_configurationcountry)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(cfg_configurationcountry), pParameters.ShipFrom.CountryGuid)
                    : null;
                fin_configurationpaymentmethod configurationPaymentMethod = (pParameters.PaymentMethod != null && pParameters.PaymentMethod != Guid.Empty)
                    ? (fin_configurationpaymentmethod)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(fin_configurationpaymentmethod), pParameters.PaymentMethod)
                    : null;
                //Helper Variables
                if (customer != null) _countryCode2 = customer.Country.Code2;
                decimal totalDocumentForeignCurrency = (pParameters.ArticleBag.TotalFinal);
                decimal totalDocumentSystemCurrency = (configurationCurrency != null)
                    ? (pParameters.ArticleBag.TotalFinal * configurationCurrency.ExchangeRate)
                    : totalDocumentForeignCurrency
                ;
                bool customerIsSingularEntity = (customer != null && customer.Country != null)
                    ? FiscalNumber.IsSingularEntity(customer.FiscalNumber, customer.Country.Code2)
                    //Cant get valid value without a valid customer country, defaults to SingularEntity
                    : true;
                //RegEx
                //If not Saft Document Type 2, required greater than zero in Price, else we can have zero or greater from Document Type 2 (ex Transportation Guide)
                string regExArticlePrice = (documentType != null && documentType.SaftDocumentType != SaftDocumentType.MovementOfGoods) ? SettingsApp.RegexDecimalGreaterThanZero : SettingsApp.RegexDecimalGreaterEqualThanZero;
                //Never Override SettingsApp.FinanceRuleSimplifiedInvoiceMaxValue|FinanceRuleRequiredCustomerDetailsAboveValue values to be Sync with LogicPos UI
                int financeRuleSimplifiedInvoiceMaxTotal = SettingsApp.FinanceRuleSimplifiedInvoiceMaxTotal;
                int financeRuleSimplifiedInvoiceMaxTotalServices = SettingsApp.FinanceRuleSimplifiedInvoiceMaxTotalServices;
                int financeRuleRequiredCustomerDetailsAboveValue = SettingsApp.FinanceRuleRequiredCustomerDetailsAboveValue;

                //Required Fields

                //P1
                bool requiredPaymentCondition = (
                    pParameters.DocumentType == SettingsApp.XpoOidDocumentFinanceTypeInvoice ||
                    pParameters.DocumentType == SettingsApp.XpoOidDocumentFinanceTypeConsignationInvoice ||
                    pParameters.DocumentType == SettingsApp.XpoOidDocumentFinanceTypeBudget ||
                    pParameters.DocumentType == SettingsApp.XpoOidDocumentFinanceTypeProformaInvoice
                );
                bool requiredPaymentMethod = (
                    pParameters.DocumentType == SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice ||
                    pParameters.DocumentType == SettingsApp.XpoOidDocumentFinanceTypeInvoiceAndPayment
                );
                bool requireParentDocument = (pParameters.DocumentType == SettingsApp.XpoOidDocumentFinanceTypeCreditNote);

                //P2
                bool requireAllCustomerFields = (
                    //SimplifiedInvoice
                    (
                        pParameters.DocumentType == SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice &&
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
                _fields.Add(FinanceValidationError.ERROR_RULE_ARTICLEBAG_GLOBAL_DISCOUNT_INVALID,
                    new ProcessFinanceDocumentValidationField("ArticleBag.Discount", (pParameters.ArticleBag != null) ? pParameters.ArticleBag.DiscountGlobal : 0.0m, SettingsApp.RegexPercentage, true)
                );
                //P1
                _fields.Add(FinanceValidationError.ERROR_FIELD_DOCUMENT_TYPE_INVALID,
                    new ProcessFinanceDocumentValidationField("DocumentType", pParameters.DocumentType, SettingsApp.RegexGuid, true)
                );
                _fields.Add(FinanceValidationError.ERROR_FIELD_PAYMENT_CONDITION_INVALID,
                    new ProcessFinanceDocumentValidationField("PaymentCondition", pParameters.PaymentCondition, SettingsApp.RegexGuid, requiredPaymentCondition)
                );
                _fields.Add(FinanceValidationError.ERROR_FIELD_PAYMENT_METHOD_INVALID,
                    new ProcessFinanceDocumentValidationField("PaymentMethod", pParameters.PaymentMethod, SettingsApp.RegexGuid, requiredPaymentMethod)
                );
                _fields.Add(FinanceValidationError.ERROR_FIELD_CURRENCY_INVALID,
                    new ProcessFinanceDocumentValidationField("Currency", pParameters.Currency, SettingsApp.RegexGuid, true)
                );
                _fields.Add(FinanceValidationError.ERROR_FIELD_DOCUMENT_PARENT_INVALID,
                    new ProcessFinanceDocumentValidationField("DocumentParent", pParameters.DocumentParent, SettingsApp.RegexGuid, requireParentDocument)
                );
                _fields.Add(FinanceValidationError.ERROR_FIELD_NOTES_INVALID,
                    new ProcessFinanceDocumentValidationField("Notes", pParameters.Notes, SettingsApp.RegexAlfaNumericExtended, false)
                );
                //P2
                _fields.Add(FinanceValidationError.ERROR_FIELD_CUSTOMER_NAME_INVALID,
                    new ProcessFinanceDocumentValidationField("Customer.Name", (customer != null) ? customer.Name : string.Empty, SettingsApp.RegexAlfaNumericExtended, requireCustomerName)
                );
                _fields.Add(FinanceValidationError.ERROR_FIELD_CUSTOMER_ADDRESS_INVALID,
                    new ProcessFinanceDocumentValidationField("Customer.Address", (customer != null) ? customer.Address : string.Empty, SettingsApp.RegexAlfaNumericExtended, requireAllCustomerFields)
                );
                _fields.Add(FinanceValidationError.ERROR_FIELD_CUSTOMER_LOCALITY_INVALID,
                    new ProcessFinanceDocumentValidationField("Customer.Locality", (customer != null) ? customer.Locality : string.Empty, SettingsApp.RegexAlfaNumericExtended, false)
                );
                //If customer undefined Defaults to SettingsApp.ConfigurationSystemCountry.RegExZipCode
                _fields.Add(FinanceValidationError.ERROR_FIELD_CUSTOMER_ZIPCODE_INVALID,
                new ProcessFinanceDocumentValidationField("Customer.ZipCode", (customer != null && customer.ZipCode != null) ? customer.ZipCode : string.Empty, (customer != null && customer.Country != null) ? customer.Country.RegExZipCode : SettingsApp.ConfigurationSystemCountry.RegExZipCode, requireAllCustomerFields)
                );
                _fields.Add(FinanceValidationError.ERROR_FIELD_CUSTOMER_CITY_INVALID,
                    new ProcessFinanceDocumentValidationField("Customer.City", (customer != null && customer.City != null) ? customer.City : string.Empty, SettingsApp.RegexAlfa, requireAllCustomerFields)
                );
                _fields.Add(FinanceValidationError.ERROR_FIELD_CUSTOMER_COUNTRY_INVALID,
                    new ProcessFinanceDocumentValidationField("Customer.Country", (customer != null && customer.Country != null) ? customer.Country.Oid : Guid.Empty, SettingsApp.RegexGuid, true)
                );
                //If customer undefined Defaults to SettingsApp.ConfigurationSystemCountry.RegExFiscalNumber
                _fields.Add(FinanceValidationError.ERROR_FIELD_CUSTOMER_FISCAL_NUMBER_INVALID,
                    new ProcessFinanceDocumentValidationField("Customer.FiscalNumber", (customer != null && customer.FiscalNumber != null) ? customer.FiscalNumber : string.Empty, (customer != null && customer.Country != null) ? customer.Country.RegExFiscalNumber : SettingsApp.ConfigurationSystemCountry.RegExFiscalNumber, requireFiscalNumber)
                );
                _fields.Add(FinanceValidationError.ERROR_FIELD_CUSTOMER_CARDNUMBER_INVALID,
                    new ProcessFinanceDocumentValidationField("Customer.CardNumber", (customer != null && customer.CardNumber != null) ? customer.CardNumber : string.Empty, SettingsApp.RegexAlfaNumericExtended, false)
                );
                _fields.Add(FinanceValidationError.ERROR_FIELD_CUSTOMER_DISCOUNT_INVALID,
                    new ProcessFinanceDocumentValidationField("Customer.Discount", (customer != null) ? customer.Discount : 0.0m, SettingsApp.RegexPercentage, true)
                );
                //P4
                if (pParameters.ShipTo != null && countryShipTo != null)
                {
                    _fields.Add(FinanceValidationError.ERROR_FIELD_SHIPTO_ADDRESS_DETAIL_INVALID,
                        new ProcessFinanceDocumentValidationField("ShipTo.AddressDetail", (pParameters.ShipTo != null) ? pParameters.ShipTo.AddressDetail : string.Empty, SettingsApp.RegexAlfaNumericExtended, requiredWayBillModeFields)
                    );
                    _fields.Add(FinanceValidationError.ERROR_FIELD_SHIPTO_REGION_INVALID,
                        new ProcessFinanceDocumentValidationField("ShipTo.Region", (pParameters.ShipTo != null) ? pParameters.ShipTo.Region : string.Empty, SettingsApp.RegexAlfa, false)
                    );
                    _fields.Add(FinanceValidationError.ERROR_FIELD_SHIPTO_POSTALCODE_INVALID,
                        new ProcessFinanceDocumentValidationField("ShipTo.PostalCode", (pParameters.ShipTo != null) ? pParameters.ShipTo.PostalCode : string.Empty, countryShipTo.RegExZipCode, requiredWayBillModeFields)
                    );
                    _fields.Add(FinanceValidationError.ERROR_FIELD_SHIPTO_CITY_INVALID,
                        new ProcessFinanceDocumentValidationField("ShipTo.City", (pParameters.ShipTo != null) ? pParameters.ShipTo.City : string.Empty, SettingsApp.RegexAlfa, requiredWayBillModeFields)
                    );
                    _fields.Add(FinanceValidationError.ERROR_FIELD_SHIPTO_COUNTRY_INVALID,
                        new ProcessFinanceDocumentValidationField("ShipTo.Country", (pParameters.ShipTo != null) ? pParameters.ShipTo.CountryGuid : Guid.Empty, SettingsApp.RegexGuid, requiredWayBillModeFields)
                    );
                    _fields.Add(FinanceValidationError.ERROR_FIELD_SHIPTO_DELIVERYDATE_INVALID,
                        new ProcessFinanceDocumentValidationField("ShipTo.DeliveryDate", (pParameters.ShipTo != null && pParameters.ShipTo.DeliveryDate != DateTime.MinValue) ? pParameters.ShipTo.DeliveryDate.ToString(SettingsApp.DateTimeFormat) : string.Empty, SettingsApp.RegexDateTime, requiredWayBillModeFields)
                    );
                    _fields.Add(FinanceValidationError.ERROR_FIELD_SHIPTO_DELIVERYID_INVALID,
                        new ProcessFinanceDocumentValidationField("ShipTo.DeliveryID", (pParameters.ShipTo != null) ? pParameters.ShipTo.DeliveryID : string.Empty, SettingsApp.RegexAlfaNumericExtended, false)
                    );
                    _fields.Add(FinanceValidationError.ERROR_FIELD_SHIPTO_WAREHOUSEID_INVALID,
                        new ProcessFinanceDocumentValidationField("ShipTo.WarehouseID", (pParameters.ShipTo != null) ? pParameters.ShipTo.WarehouseID : string.Empty, SettingsApp.RegexAlfaNumericExtended, false)
                    );
                    _fields.Add(FinanceValidationError.ERROR_FIELD_SHIPTO_LOCATIONID_INVALID,
                        new ProcessFinanceDocumentValidationField("ShipTo.LocationID", (pParameters.ShipTo != null) ? pParameters.ShipTo.LocationID : string.Empty, SettingsApp.RegexAlfaNumericExtended, false)
                    );
                }
                //P5
                if (pParameters.ShipTo != null && countryShipFrom != null)
                {
                    _fields.Add(FinanceValidationError.ERROR_FIELD_SHIPFROM_ADDRESS_DETAIL_INVALID,
                        new ProcessFinanceDocumentValidationField("ShipFrom.AddressDetail", (pParameters.ShipFrom != null) ? pParameters.ShipFrom.AddressDetail : string.Empty, SettingsApp.RegexAlfaNumericExtended, requiredWayBillModeFields)
                    );
                    _fields.Add(FinanceValidationError.ERROR_FIELD_SHIPFROM_REGION_INVALID,
                        new ProcessFinanceDocumentValidationField("ShipFrom.Region", (pParameters.ShipFrom != null) ? pParameters.ShipFrom.Region : string.Empty, SettingsApp.RegexAlfa, false)
                    );
                    _fields.Add(FinanceValidationError.ERROR_FIELD_SHIPFROM_POSTALCODE_INVALID,
                        new ProcessFinanceDocumentValidationField("ShipFrom.PostalCode", (pParameters.ShipFrom != null) ? pParameters.ShipFrom.PostalCode : string.Empty, countryShipFrom.RegExZipCode, requiredWayBillModeFields)
                    );
                    _fields.Add(FinanceValidationError.ERROR_FIELD_SHIPFROM_CITY_INVALID,
                        new ProcessFinanceDocumentValidationField("ShipFrom.City", (pParameters.ShipFrom != null) ? pParameters.ShipFrom.City : string.Empty, SettingsApp.RegexAlfa, requiredWayBillModeFields)
                    );
                    _fields.Add(FinanceValidationError.ERROR_FIELD_SHIPFROM_COUNTRY_INVALID,
                        new ProcessFinanceDocumentValidationField("ShipFrom.Country", (pParameters.ShipFrom != null) ? pParameters.ShipFrom.CountryGuid : Guid.Empty, SettingsApp.RegexGuid, requiredWayBillModeFields)
                    );
                    _fields.Add(FinanceValidationError.ERROR_FIELD_SHIPFROM_DELIVERYDATE_INVALID,
                        new ProcessFinanceDocumentValidationField("ShipFrom.DeliveryDate", (pParameters.ShipFrom != null && pParameters.ShipFrom.DeliveryDate != DateTime.MinValue) ? pParameters.ShipFrom.DeliveryDate.ToString(SettingsApp.DateTimeFormat) : string.Empty, SettingsApp.RegexDateTime, requiredWayBillModeFields)
                    );
                    _fields.Add(FinanceValidationError.ERROR_FIELD_SHIPFROM_DELIVERYID_INVALID,
                        new ProcessFinanceDocumentValidationField("ShipFrom.DeliveryID", (pParameters.ShipFrom != null) ? pParameters.ShipFrom.DeliveryID : string.Empty, SettingsApp.RegexAlfaNumericExtended, false)
                    );
                    _fields.Add(FinanceValidationError.ERROR_FIELD_SHIPFROM_WAREHOUSEID_INVALID,
                        new ProcessFinanceDocumentValidationField("ShipFrom.WarehouseID", (pParameters.ShipFrom != null) ? pParameters.ShipFrom.WarehouseID : string.Empty, SettingsApp.RegexAlfaNumericExtended, false)
                    );
                    _fields.Add(FinanceValidationError.ERROR_FIELD_SHIPFROM_LOCATIONID_INVALID,
                        new ProcessFinanceDocumentValidationField("ShipFrom.LocationID", (pParameters.ShipFrom != null) ? pParameters.ShipFrom.LocationID : string.Empty, SettingsApp.RegexAlfaNumericExtended, false)
                    );
                }

                /* IN009109 - treatment for Transport Documents */
                if (requiredWayBillModeFields)
                {
                    DateTime shippingDate = pParameters.ShipFrom.DeliveryDate;
                    DateTime deliveryDate = pParameters.ShipTo.DeliveryDate;

                    if (deliveryDate < shippingDate)
                    {
                        _fields.Add(FinanceValidationError.ERROR_FIELD_SHIPFROM_DELIVERYDATE_BEFORE_SHIPPINGDATE,
                        new ProcessFinanceDocumentValidationField("ShipFrom.DeliveryDate", string.Empty, SettingsApp.RegexDateTime, true));
                    }
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                // Process Article Field Validation 
                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                _fieldsArticle.Add(FinanceValidationError.ERROR_FIELD_ARTICLE_OID_INVALID,
                    new ProcessFinanceDocumentValidationField("Oid", null, SettingsApp.RegexGuid, true)
                );
                _fieldsArticle.Add(FinanceValidationError.ERROR_FIELD_ARTICLE_CODE_INVALID,
                    new ProcessFinanceDocumentValidationField("Code", null, SettingsApp.RegexAlfaNumericArticleCode, true)
                );
                _fieldsArticle.Add(FinanceValidationError.ERROR_FIELD_ARTICLE_DESIGNATION_INVALID,
                    new ProcessFinanceDocumentValidationField("Designation", null, SettingsApp.RegexAlfaNumericExtended, true)
                );
                _fieldsArticle.Add(FinanceValidationError.ERROR_FIELD_ARTICLE_PRICE_INVALID,
                    new ProcessFinanceDocumentValidationField("Price", null, regExArticlePrice, true)
                );
                _fieldsArticle.Add(FinanceValidationError.ERROR_FIELD_ARTICLE_QUANTITY_INVALID,
                    new ProcessFinanceDocumentValidationField("Quantity", null, SettingsApp.RegexDecimalGreaterEqualThanZero, true)
                );
                //Removed : Framework LogicErp dont send ACRONYM : Search all ERROR_FIELD_ARTICLE_UNIT_MEASURE_ACRONYM_INVALID occurences
                //_fieldsArticle.Add(FinanceValidationError.ERROR_FIELD_ARTICLE_UNIT_MEASURE_ACRONYM_INVALID,
                //    new ProcessFinanceDocumentValidationField("UnitMeasure.Acronym", null, SettingsApp.RegexAlfaNumeric, true)
                //);
                _fieldsArticle.Add(FinanceValidationError.ERROR_FIELD_ARTICLE_DISCOUNT_INVALID,
                    new ProcessFinanceDocumentValidationField("Discount", null, SettingsApp.RegexPercentage, true)
                );
                _fieldsArticle.Add(FinanceValidationError.ERROR_FIELD_ARTICLE_VAT_RATE_INVALID,
                    new ProcessFinanceDocumentValidationField("VatRate", null, SettingsApp.RegexPercentage, true)
                );
                _fieldsArticle.Add(FinanceValidationError.ERROR_FIELD_ARTICLE_VAT_EXEMPTION_REASON_INVALID,
                    new ProcessFinanceDocumentValidationField("VatExemptionReason", null, SettingsApp.RegexGuid, false)
                );

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::            
                // Validation Rules
                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                if (pLoggedUser == Guid.Empty)
                {
                    ResultAdd(FinanceValidationError.ERROR_RULE_LOGGED_USER_INVALID);
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                if (GlobalFramework.LoggedTerminal == null)
                {
                    ResultAdd(FinanceValidationError.ERROR_RULE_LOGGED_TERMINAL_INVALID);
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                //Required a Valid LoggedTerminal

                fin_documentfinanceseries documentFinanceSerie = null;
                if (GlobalFramework.LoggedTerminal != null)
                {
                    documentFinanceSerie = ProcessFinanceDocumentSeries.GetDocumentFinanceYearSerieTerminal(pParameters.DocumentType).Serie;
                }

                if (documentFinanceSerie == null)
                {
                    ResultAdd(FinanceValidationError.ERROR_RULE_SERIE_INVALID);
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Check Source Mode
                switch (pParameters.SourceMode)
                {
                    case PersistFinanceDocumentSourceMode.CurrentOrderMain:
                        if (GlobalFramework.SessionApp == null
                            || GlobalFramework.SessionApp.OrdersMain == null
                            || GlobalFramework.SessionApp.OrdersMain[GlobalFramework.SessionApp.CurrentOrderMainOid] == null
                        )
                        {
                            ResultAdd(FinanceValidationError.ERROR_RULE_SOURCE_MODE_ORDERMAIN_EMPTY);
                        }
                        break;
                    case PersistFinanceDocumentSourceMode.CustomArticleBag:
                        if (pParameters.ArticleBag == null || pParameters.ArticleBag.Count <= 0)
                        {
                            ResultAdd(FinanceValidationError.ERROR_RULE_SOURCE_MODE_CUSTOM_ARTICLEBAG_EMPTY);
                        }
                        break;
                    case PersistFinanceDocumentSourceMode.CurrentAcountDocuments:
                        if (pParameters.FinanceDocuments == null | pParameters.FinanceDocuments.Count <= 0)
                        {
                            ResultAdd(FinanceValidationError.ERROR_RULE_SOURCE_MODE_FINANCE_DOCUMENTS_EMPTY);
                        }
                        break;
                    default:
                        break;
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                //Customer <> ParentDocument.Customer (And ConferenceDocument)

                if (customer != null && customerParentDocument != null && customer != customerParentDocument && documentParent.DocumentType.Oid != SettingsApp.XpoOidDocumentFinanceTypeConferenceDocument)
                {
                    ResultAdd(FinanceValidationError.ERROR_RULE_PARENT_DOCUMENT_CUSTOMER_AND_CURRENT_DOCUMENT_CUSTOMER_INVALID);
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                //ParentDocument.DocumentStatusStatus = Cancelled Document

                if (documentParent != null && documentParent.DocumentStatusStatus == "A")
                {
                    ResultAdd(FinanceValidationError.ERROR_RULE_PARENT_DOCUMENT_CANCELLED_DETECTED);
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                //All Series, Global System

                //This code is similar to : ShowMessageTouchCheckIfFinanceDocumentHasValidDocumentDate, change in both

                //Protection to Check if System.DateTime is < Last Document.DateTime (All Finance Documents)
                DateTime dateTimeLastDocument = ProcessFinanceDocument.GetLastDocumentDateTime();
                if (!pIgnoreWarning && pParameters.DocumentDateTime < dateTimeLastDocument && dateTimeLastDocument != DateTime.MinValue)
                {
                    ResultAdd(FinanceValidationError.WARNING_RULE_SYSTEM_DATE_GLOBAL);
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                //From current Serie

                //Protection to Check if SystemDate.Date is < Last DocumentDate.Date (From Document Serie/DocumentType)
                DateTime dateLastDocumentFromSerie = DateTime.MaxValue;
                if (documentFinanceSerie != null)
                {
                    dateLastDocumentFromSerie = ProcessFinanceDocument.GetLastDocumentDateTime(string.Format("DocumentSerie = '{0}'", documentFinanceSerie.Oid)).Date;
                }

                if (!pIgnoreWarning && pParameters.DocumentDateTime < dateLastDocumentFromSerie && dateLastDocumentFromSerie != DateTime.MinValue)
                {
                    ResultAdd(FinanceValidationError.WARNING_RULE_SYSTEM_DATE_SERIE);
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                //DocumentType

                if (pParameters.DocumentType == Guid.Empty)
                {
                    ResultAdd(FinanceValidationError.ERROR_RULE_DOCUMENT_FINANCE_TYPE_INVALID);
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                //PaymentCondition

                if (pParameters.PaymentCondition == Guid.Empty &&
                    (
                        pParameters.DocumentType == SettingsApp.XpoOidDocumentFinanceTypeInvoice ||
                        pParameters.DocumentType == SettingsApp.XpoOidDocumentFinanceTypeProformaInvoice ||
                        pParameters.DocumentType == SettingsApp.XpoOidDocumentFinanceTypeBudget
                    )
                )
                {
                    ResultAdd(FinanceValidationError.ERROR_RULE_DOCUMENT_FINANCE_TYPE_PAYMENT_CONDITION_INVALID);
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                //PaymentMethod

                if (pParameters.PaymentMethod == Guid.Empty &&
                    (
                        pParameters.DocumentType == SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice ||
                        pParameters.DocumentType == SettingsApp.XpoOidDocumentFinanceTypeInvoiceAndPayment
                    )
                )
                {
                    ResultAdd(FinanceValidationError.ERROR_RULE_DOCUMENT_FINANCE_TYPE_PAYMENT_METHOD_INVALID);
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                //SimplifiedInvoice

                if (pParameters.DocumentType == SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice)
                {
                    //Check if SimplifiedInvoice is Service Total > MaxTotalServices
                    if (pParameters.ArticleBag.GetClassTotals("S") > financeRuleSimplifiedInvoiceMaxTotalServices)
                    {
                        ResultAdd(FinanceValidationError.ERROR_RULE_DOCUMENT_FINANCE_TYPE_SIMPLIFIED_INVOICE_MAX_TOTAL_SERVICE_EXCEED);
                    }

                    //Check if SimplifiedInvoice Total > MaxValue 
                    if (pParameters.ArticleBag.TotalFinal > financeRuleSimplifiedInvoiceMaxTotal)
                    {
                        ResultAdd(FinanceValidationError.ERROR_RULE_DOCUMENT_FINANCE_TYPE_SIMPLIFIED_INVOICE_MAX_TOTAL_EXCEED);
                    }
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                //ParentDocuments

                //Get Valid Parent Documents
                Guid[] validParentDocuments = FrameworkUtils.GetDocumentTypeValidSourceDocuments(pParameters.DocumentType);

                //If has a ParentDocument, Check if is a Valid One for current document type, else trigger Error
                if (documentParent != null && !validParentDocuments.Contains<Guid>(documentParent.DocumentType.Oid))
                {
                    ResultAdd(FinanceValidationError.ERROR_RULE_DOCUMENT_FINANCE_TYPE_PARENT_DOCUMENT_INVALID);
                }

                //ParentDocuments: Credit Note
                if (pParameters.DocumentType == SettingsApp.XpoOidDocumentFinanceTypeCreditNote)
                {
                    if (documentParent == null)
                    {
                        //Require to Check if CreditNote has the Required ParentDocument added from back validations, if not and Add it
                        if (!_resultEnum.ContainsKey(FinanceValidationError.ERROR_RULE_DOCUMENT_FINANCE_TYPE_PARENT_DOCUMENT_INVALID))
                        {
                            ResultAdd(FinanceValidationError.ERROR_RULE_DOCUMENT_FINANCE_TYPE_PARENT_DOCUMENT_INVALID);
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
                                if (String.IsNullOrEmpty(item.Value.Reason))
                                {
                                    hasArticlesWithoutReason = true;
                                }
                            }
                        }

                        if (FrameworkUtils.GetCreditNoteValidation(documentParent, pParameters.ArticleBag))
                        {
                            hasArticlesQuantityGreaterThanUnCredit = true;
                        }

                        if (hasArticlesWithoutReference) { ResultAdd(FinanceValidationError.ERROR_RULE_DOCUMENT_FINANCE_TYPE_CREDIT_NOTE_PARENT_DOCUMENT_ARTICLE_REFERENCE_INVALID); }
                        if (hasArticlesWithoutReason) { ResultAdd(FinanceValidationError.ERROR_RULE_DOCUMENT_FINANCE_TYPE_CREDIT_NOTE_PARENT_DOCUMENT_ARTICLE_REASON_INVALID); }
                        if (hasArticlesQuantityGreaterThanUnCredit) { ResultAdd(FinanceValidationError.ERROR_RULE_DOCUMENT_FINANCE_TYPE_CREDIT_NOTE_ARTICLEBAG_ARTICLES_CREDITED_DETECTED); }
                    }
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                //ParentDocument : Customer <> ParentDocument Customer

                if (documentType != null && customer != null && customer.Oid == SettingsApp.XpoOidDocumentFinanceMasterFinalConsumerEntity
                    && (
                        documentType.Oid != SettingsApp.XpoOidDocumentFinanceTypeInvoice
                        && documentType.Oid != SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice
                        && documentType.Oid != SettingsApp.XpoOidDocumentFinanceTypeConferenceDocument
                        )
                    )
                {
                    //Has an Exception to base Rule, is Valid if is Derived from a Parent Document 
                    if (documentParent != null && documentParent.EntityOid != SettingsApp.XpoOidDocumentFinanceMasterFinalConsumerEntity)
                    {
                        ResultAdd(FinanceValidationError.ERROR_RULE_DOCUMENT_FINANCE_TYPE_WITH_FINAL_CONSUMER_INVALID);
                    }
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //DocumentType: Invoice|InvoiceAndPayment|Simplified Invoice cant emmit documents with FinalConsumer, or User with miss details above FinanceRuleRequiredCustomerDetailsAboveValue (1000)
                if (documentType != null && customer != null
                    &&
                    (
                        documentType.Oid == SettingsApp.XpoOidDocumentFinanceTypeInvoice ||
                        documentType.Oid == SettingsApp.XpoOidDocumentFinanceTypeInvoiceAndPayment ||
                        documentType.Oid == SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice
                    )
                    &&
                    FrameworkUtils.IsInValidFinanceDocumentCustomer(
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
                    ResultAdd(FinanceValidationError.ERROR_RULE_DOCUMENT_FINANCE_TYPE_WITH_CUSTOMER_DETAILS_INVALID);
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                //Customer Card: Amount Invalid and Recharge Card Invalid

                if (!FrameworkUtils.IsCustomerCardValidForArticleBag(pParameters.ArticleBag, customer))
                {
                    ResultAdd(FinanceValidationError.ERROR_RULE_CUSTOMER_CARD_RECHARGE_CARD_ARTICLE_DETECTED_WITH_CUSTOMER_CARD_INVALID);
                }

                if (configurationPaymentMethod != null
                    && configurationPaymentMethod.Token == "CUSTOMER_CARD"
                    && pParameters.ArticleBag.TotalFinal > customer.CardCredit
                )
                {
                    ResultAdd(FinanceValidationError.ERROR_RULE_CUSTOMER_CARD_AMOUNT_INVALID);
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
                _log.Error(ex.Message, ex);
            }

            return _resultEnum;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Process Payments/Recibos

        public static SortedDictionary<FinanceValidationError, object> ValidatePersistFinanceDocumentPayment(List<fin_documentfinancemaster> pInvoices, List<fin_documentfinancemaster> pCreditNotes, Guid pCustomer, Guid pPaymentMethod, Guid pConfigurationCurrency, decimal pPaymentAmount, string pPaymentNotes = "")
        {
            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //Init Common Variables

            _resultEnum = new SortedDictionary<FinanceValidationError, object>();
            _fields = new Dictionary<FinanceValidationError, ProcessFinanceDocumentValidationField>();

            //Disable to go to AT or to Release
            if (!Debugger.IsAttached) return _resultEnum;

            try
            {
                //Get XPGuidObjects from Parameters
                erp_customer customer = (pCustomer != null && pCustomer != Guid.Empty)
                    ? (erp_customer)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(erp_customer), pCustomer)
                    : null;
                fin_configurationpaymentmethod paymentMethod = (pPaymentMethod != null && pPaymentMethod != Guid.Empty)
                    ? (fin_configurationpaymentmethod)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(fin_configurationpaymentmethod), pPaymentMethod)
                    : null;
                cfg_configurationcurrency currency = (pConfigurationCurrency != null && pConfigurationCurrency != Guid.Empty)
                    ? (cfg_configurationcurrency)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(cfg_configurationcurrency), pConfigurationCurrency)
                    : null;

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                // Process Field Validation
                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                _fields.Add(FinanceValidationError.ERROR_FIELD_CUSTOMER_INVALID,
                    new ProcessFinanceDocumentValidationField("Customer", customer.Oid, SettingsApp.RegexGuid, true)
                );
                _fields.Add(FinanceValidationError.ERROR_FIELD_PAYMENT_METHOD_INVALID,
                    new ProcessFinanceDocumentValidationField("PaymentMethod", paymentMethod.Oid, SettingsApp.RegexGuid, true)
                );
                _fields.Add(FinanceValidationError.ERROR_FIELD_CURRENCY_INVALID,
                    new ProcessFinanceDocumentValidationField("Currency", currency.Oid, SettingsApp.RegexGuid, true)
                );
                _fields.Add(FinanceValidationError.ERROR_FIELD_PAYMENT_AMOUNT_INVALID,
                    new ProcessFinanceDocumentValidationField("PaymentAmount", pPaymentAmount, SettingsApp.RegexDecimalGreaterEqualThanZero, true)
                );
                _fields.Add(FinanceValidationError.ERROR_FIELD_NOTES_INVALID,
                    new ProcessFinanceDocumentValidationField("Notes", pPaymentNotes, SettingsApp.RegexAlfaNumericExtended, false)
                );

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::            
                // Validation Rules : Valid Documents
                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Validate Invoice Documents
                List<Guid> validCustomerPaymentInvoices = FrameworkUtils.GetValidDocumentsForPayment(customer.Oid, SettingsApp.XpoOidDocumentFinanceTypeInvoice);
                foreach (var item in pInvoices)
                {
                    if (!validCustomerPaymentInvoices.Contains(item.Oid)
                        && !_resultEnum.ContainsKey(FinanceValidationError.ERROR_RULE_DOCUMENT_FINANCE_INVOICE_DOCUMENTS_DETECTED)
                    )
                    {
                        ResultAdd(FinanceValidationError.ERROR_RULE_DOCUMENT_FINANCE_INVOICE_DOCUMENTS_DETECTED);
                    }
                }

                //Validate Invoice Documents
                List<Guid> validCustomerPaymentCreditNotes = FrameworkUtils.GetValidDocumentsForPayment(customer.Oid, SettingsApp.XpoOidDocumentFinanceTypeCreditNote);
                foreach (var item in pCreditNotes)
                {
                    if (!validCustomerPaymentCreditNotes.Contains(item.Oid)
                        && !_resultEnum.ContainsKey(FinanceValidationError.ERROR_RULE_DOCUMENT_FINANCE_CREDIT_NOTE_DOCUMENTS_DETECTED)
                    )
                    {
                        ResultAdd(FinanceValidationError.ERROR_RULE_DOCUMENT_FINANCE_CREDIT_NOTE_DOCUMENTS_DETECTED);
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
                    sqlResult = GlobalFramework.SessionXpo.ExecuteScalar(sqlInvoices);
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
                totalPaymentDiference = Math.Round(totalDocumentsDiference - paymentAmount, SettingsApp.DecimalRoundTo);

                if (_debug) _log.Debug(String.Format(
                    "PaymentAmount: [{0}], InvoicesDebit: [{1}], CreditNotes: [{2}], DocumentsDiference: [{3}], PaymentDiference:[{4}]",
                    paymentAmount, totalInvoicesDebit, totalCreditNotes, totalDocumentsDiference, totalPaymentDiference)
                );

                //Invoices
                if (hasInvoicePayed) { ResultAdd(FinanceValidationError.ERROR_RULE_DOCUMENT_FINANCE_TYPE_INVOICE_PAYED_DETECTED); }
                if (hasInvoiceCancelled) { ResultAdd(FinanceValidationError.ERROR_RULE_DOCUMENT_FINANCE_TYPE_INVOICE_CANCELLED_DETECTED); }
                //Credit Notes
                if (hasCreditNotePayed) { ResultAdd(FinanceValidationError.ERROR_RULE_DOCUMENT_FINANCE_TYPE_CREDIT_NOTE_PAYED_DETECTED); }
                if (hasCreditNoteCancelled) { ResultAdd(FinanceValidationError.ERROR_RULE_DOCUMENT_FINANCE_TYPE_CREDIT_NOTE_CANCELLED_DETECTED); }
                if (hasDocumentsDiferentFromTargetCustomer) { ResultAdd(FinanceValidationError.ERROR_RULE_PAYMENT_DOCUMENTS_WITH_DIFERENT_CUSTOMERS_DETECTED); }
                //Check Valid Payment : Diference must be greater or equal to zero
                if (totalPaymentDiference < 0) { ResultAdd(FinanceValidationError.ERROR_RULE_PAYMENT_AMOUNT_GREATER_THAN_TOTAL_SETTLE_IN_DOCUMENTS_DETECTED); }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Add FieldErrors
                GetFieldErrors();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return (_resultEnum);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Helper Methods

        private static void ResultAdd(FinanceValidationError pTokenEnum)
        {
            ResultAdd(pTokenEnum, null);
        }

        private static void ResultAdd(FinanceValidationError pTokenEnum, object pObject)
        {
            string value = string.Empty;

            //Add
            _resultEnum.Add(pTokenEnum, pObject);
            //String Value
            //value = Enum.GetName(typeof(FinanceValidationError), _resultEnum[_resultEnum.Count - 1]).ToUpper();
            value = Enum.GetName(typeof(FinanceValidationError), pTokenEnum).ToUpper();
            if (_debug) _log.Debug(value);
        }

        //Get List<string> form SortedDictionary<FinanceValidationError,object>
        private static List<string> ResultToString()
        {
            List<string> result = new List<string>();
            string key = string.Empty;
            string value = string.Empty;

            try
            {
                foreach (var item in _resultEnum)
                {
                    key = Enum.GetName(typeof(FinanceValidationError), item.Key).ToString().ToUpper();
                    if (item.Value != null && item.Value.GetType() == typeof(string))
                    {
                        value = Convert.ToString(item.Value);
                        if (value != string.Empty) key = string.Format("{0}_{1}", key, value);
                    }

                    result.Add(key);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        private static void GetFieldErrors()
        {
            bool result = false;
            string value = string.Empty;
            Type type = null;

            //Loop Validation Fields Rules
            foreach (var item in _fields)
            {
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
                    item.Key == FinanceValidationError.ERROR_FIELD_ARTICLE_PRICE_INVALID
                    //|| item.Key == FinanceValidationError.ERROR_FIELD_SHIPFROM_DELIVERYDATE_INVALID
                    )
                {
                    _log.Debug("BREAKPOINT");
                }

                result = FrameworkUtils.Validate(value, item.Value.Rule, item.Value.Required);

                //Extra Validation for types that have more than RegEx Requirements
                if (result && item.Key == FinanceValidationError.ERROR_FIELD_CUSTOMER_FISCAL_NUMBER_INVALID)
                {
                    result = FiscalNumber.IsValidFiscalNumber(value, _countryCode2);
                }

                if (!result)
                {
                    ResultAdd(item.Key);
                    if (_debug) _log.Debug(String.Format("Key: [{0}], Name: [{1}], Value: [{2}], Type: [{3}], Rule: [{4}], Required: [{5}]", item.Key, item.Value.Name, value, (type != null) ? type.ToString() : "NULL", item.Value.Rule, item.Value.Required));
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
            ProcessFinanceDocumentValidationField validationFieldOid;
            ProcessFinanceDocumentValidationField validationFieldCode;
            ProcessFinanceDocumentValidationField validationFieldDesignation;
            ProcessFinanceDocumentValidationField validationFieldPrice;
            ProcessFinanceDocumentValidationField validationFieldQuantity;
            //ProcessFinanceDocumentValidationField validationFieldUnitMeasureAcronym;
            ProcessFinanceDocumentValidationField validationFieldDiscount;
            ProcessFinanceDocumentValidationField validationFieldVat;
            ProcessFinanceDocumentValidationField validationFieldVatExemptionReason;

            //Loop ArticleBag
            if (pArticleBag != null)
            {
                foreach (var item in pArticleBag)
                {
                    //Get ValidationFields references from Dictionary
                    validationFieldOid = _fieldsArticle[FinanceValidationError.ERROR_FIELD_ARTICLE_OID_INVALID];
                    validationFieldCode = _fieldsArticle[FinanceValidationError.ERROR_FIELD_ARTICLE_CODE_INVALID];
                    validationFieldDesignation = _fieldsArticle[FinanceValidationError.ERROR_FIELD_ARTICLE_DESIGNATION_INVALID];
                    validationFieldPrice = _fieldsArticle[FinanceValidationError.ERROR_FIELD_ARTICLE_PRICE_INVALID];
                    validationFieldQuantity = _fieldsArticle[FinanceValidationError.ERROR_FIELD_ARTICLE_QUANTITY_INVALID];
                    //Removed : Framework LogicErp dont send ACRONYM : Search all ERROR_FIELD_ARTICLE_UNIT_MEASURE_ACRONYM_INVALID occurences
                    //validationFieldUnitMeasureAcronym = _fieldsArticle[FinanceValidationError.ERROR_FIELD_ARTICLE_UNIT_MEASURE_ACRONYM_INVALID];
                    validationFieldDiscount = _fieldsArticle[FinanceValidationError.ERROR_FIELD_ARTICLE_DISCOUNT_INVALID];
                    validationFieldVat = _fieldsArticle[FinanceValidationError.ERROR_FIELD_ARTICLE_VAT_RATE_INVALID];
                    validationFieldVatExemptionReason = _fieldsArticle[FinanceValidationError.ERROR_FIELD_ARTICLE_VAT_EXEMPTION_REASON_INVALID];
                    //Assign current article values to Validation Rules References (Optional), this way code is cleaner, all properties are in ProcessFinanceDocumentValidationField
                    validationFieldOid.Value = item.Key.ArticleOid;
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
                    if (!FrameworkUtils.Validate(validationFieldOid.Value.ToString(), validationFieldOid.Rule, validationFieldOid.Required))
                    {
                        hasArticlesWithInvalidOid = true;
                    }
                    //Validate ValidationField: Code
                    if (!FrameworkUtils.Validate(validationFieldCode.Value.ToString(), validationFieldCode.Rule, validationFieldCode.Required))
                    {
                        hasArticlesWithInvalidCode = true;
                    }
                    //Validate ValidationField: Designation
                    if (!FrameworkUtils.Validate(validationFieldDesignation.Value.ToString(), validationFieldDesignation.Rule, validationFieldDesignation.Required))
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
                    if (!FrameworkUtils.Validate(validationFieldDiscount.Value.ToString(), validationFieldDiscount.Rule, validationFieldDiscount.Required))
                    {
                        hasArticlesWithInvalidDiscount = true;
                    }
                    //Validate ValidationField: Vat
                    if (!FrameworkUtils.Validate(validationFieldVat.Value.ToString(), validationFieldVat.Rule, validationFieldVat.Required))
                    {
                        hasArticlesWithInvalidVat = true;
                    }
                    //Validate ValidationField: VatExemptionReason
                    if (!FrameworkUtils.Validate(validationFieldVatExemptionReason.Value.ToString(), validationFieldVatExemptionReason.Rule, (item.Key.Vat == 0.0m) ? true : false))
                    {
                        hasArticlesWithInvalidVatExemptionReason = true;
                    }

                    //Rules Validation

                    //If is a ConsignationInvoice all Articles must have VatRateDutyFree Vat
                    if (pDocumentType.Oid == SettingsApp.XpoOidDocumentFinanceTypeConsignationInvoice)
                    {
                        if (item.Key.Vat != 0.0m)
                        {
                            hasArticlesWithInvalidVat = true;
                            hasArticlesWithoutVatRateDutyFree = true;
                        }

                        if (item.Key.VatExemptionReasonOid != SettingsApp.XpoOidConfigurationVatExemptionReasonM99)
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
                if (hasArticlesWithInvalidOid) ResultAdd(FinanceValidationError.ERROR_FIELD_ARTICLE_OID_INVALID);
                if (hasArticlesWithInvalidCode) ResultAdd(FinanceValidationError.ERROR_FIELD_ARTICLE_CODE_INVALID);
                if (hasArticlesWithInvalidDesignation) ResultAdd(FinanceValidationError.ERROR_FIELD_ARTICLE_DESIGNATION_INVALID);
                if (hasArticlesWithInvalidPrice) ResultAdd(FinanceValidationError.ERROR_FIELD_ARTICLE_PRICE_INVALID);
                if (hasArticlesWithInvalidQuantity) ResultAdd(FinanceValidationError.ERROR_FIELD_ARTICLE_QUANTITY_INVALID);
                //Removed : Framework LogicErp dont send ACRONYM : Search all ERROR_FIELD_ARTICLE_UNIT_MEASURE_ACRONYM_INVALID occurences
                //if (hasArticlesWithInvalidUnitMeasureAcronym) ResultAdd(FinanceValidationError.ERROR_FIELD_ARTICLE_UNIT_MEASURE_ACRONYM_INVALID);
                if (hasArticlesWithInvalidDiscount) ResultAdd(FinanceValidationError.ERROR_FIELD_ARTICLE_DISCOUNT_INVALID);
                if (_requireToChooseVatExemptionReason && hasArticlesWithInvalidVat) ResultAdd(FinanceValidationError.ERROR_FIELD_ARTICLE_VAT_RATE_INVALID);
                if (_requireToChooseVatExemptionReason && hasArticlesWithInvalidVatExemptionReason) ResultAdd(FinanceValidationError.ERROR_FIELD_ARTICLE_VAT_EXEMPTION_REASON_INVALID);
                //Specific Rule
                if (hasArticlesWithoutVatRateDutyFree) ResultAdd(FinanceValidationError.ERROR_RULE_ARTICLEBAG_ARTICLES_WITHOUT_TAX_DUTY_FREE_DETECTED);
                if (hasArticlesWithoutVatExemptionReasonM99) ResultAdd(FinanceValidationError.ERROR_RULE_ARTICLEBAG_ARTICLES_WITHOUT_TAX_EXCEPTION_REASON_M99_DETECTED);
                if (_requireToChooseVatExemptionReason && hasArticlesWithoutRequiredTaxExemptionReason) ResultAdd(FinanceValidationError.ERROR_RULE_ARTICLEBAG_ARTICLES_WITHOUT_TAX_EXCEPTION_REASON_DETECTED);
            }
        }
    }
}
