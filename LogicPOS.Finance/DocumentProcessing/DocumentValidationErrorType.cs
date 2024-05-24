namespace LogicPOS.Finance.DocumentProcessing
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

    public enum DocumentValidationErrorType
    {
        //Warnings
        WARNING_RULE_SYSTEM_DATE_GLOBAL,
        WARNING_RULE_SYSTEM_DATE_SERIE,
        //Global/Generic
        ERROR,
        //Errors
        ERROR_RULE_loggerGED_USER_INVALID,
        ERROR_RULE_loggerGED_TERMINAL_INVALID,
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
}
