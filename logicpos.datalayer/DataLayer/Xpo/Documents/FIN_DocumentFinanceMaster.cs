using DevExpress.Xpo;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    //Todo : Change File to Work with Encrypted Attributed
    // 1. change string fEntityCountry; > to [Size(50)] / EntityCountry varchar(50) DEFAULT NULL (All Dbs)
    // 2. uncomment InitEncryptedAttributes<FIN_DocumentFinanceMaster>(); in Constructor and OnAfterConstruction
    // 3. uncomment //[XPGuidObject(Encrypted = true)]
    // 4. saft and print documents are with encrypted values from db, require extra work

    [DeferredDeletion(false)]
    public class FIN_DocumentFinanceMaster : XPGuidObject
    {
        public FIN_DocumentFinanceMaster() : base() { }
        public FIN_DocumentFinanceMaster(Session session) : base(session)
        {
            // Init EncryptedAttributes - Load Encrypted Attributes Fields if Exist
            InitEncryptedAttributes<FIN_DocumentFinanceMaster>();
        }

        protected override void OnAfterConstruction()
        {
            // Init EncryptedAttributes - Load Encrypted Attributes Fields if Exist - Required for New Records to have InitEncryptedAttributes else it Triggers Exception on Save
            InitEncryptedAttributes<FIN_DocumentFinanceMaster>();
        }

        DateTime fDate;
        public DateTime Date
        {
            get { return fDate; }
            set { SetPropertyValue<DateTime>("Date", ref fDate, value); }
        }

        /*BOF: SAF-T(PT)*/
        //used for InvoiceNo (SalesInvoices) DocumentNumber(MovementOfGoods), DocumentNumber(WorkingDocuments)
        string fDocumentNumber;
        [Size(50)]
        [Indexed(Unique = true)]
        public string DocumentNumber
        {
            get { return fDocumentNumber; }
            set { SetPropertyValue<string>("DocumentNumber", ref fDocumentNumber, value); }
        }

        //N - Normal
        //S - Auto Faturação
        //A - Documento Anulado
        //R - Documento de Resumo doutros documentos criados noutras aplicações e gerado nesta aplicaçãoi
        //F - Documento Faturado
        string fDocumentStatusStatus;
        [Size(1)]
        public string DocumentStatusStatus
        {
            get { return fDocumentStatusStatus; }
            set { SetPropertyValue<string>("DocumentStatusStatus", ref fDocumentStatusStatus, value); }
        }

        string fDocumentStatusDate;
        [Size(19)]
        public string DocumentStatusDate
        {
            get { return fDocumentStatusDate; }
            set { SetPropertyValue<string>("DocumentStatusDate", ref fDocumentStatusDate, value); }
        }

        string fDocumentStatusReason;
        [Size(50)]
        public string DocumentStatusReason
        {
            get { return fDocumentStatusReason; }
            set { SetPropertyValue<string>("DocumentStatusReason", ref fDocumentStatusReason, value); }
        }

        //Used to Store 30Chars Codes for SAF-T
        string fDocumentStatusUser;
        [Size(30)]
        public string DocumentStatusUser
        {
            get { return fDocumentStatusUser; }
            set { SetPropertyValue<string>("DocumentStatusUser", ref fDocumentStatusUser, value); }
        }

        //P - Documento Produzido na Aplicação
        //I - Documento Integrado e produzido noutra aplicação 
        //M - Documento proveniente de recuperação ou de emissão manual
        string fSourceBilling;
        [Size(1)]
        public string SourceBilling
        {
            get { return fSourceBilling; }
            set { SetPropertyValue<string>("SourceBilling", ref fSourceBilling, value); }
        }

        string fHash;
        [Size(172)]
        public string Hash
        {
            get { return fHash; }
            set { SetPropertyValue<string>("Hash", ref fHash, value); }
        }

        string fHashControl;
        [Size(40)]
        public string HashControl
        {
            get { return fHashControl; }
            set { SetPropertyValue<string>("HashControl", ref fHashControl, value); }
        }

        //used for InvoiceDate (SalesInvoices) MovementDate(MovementOfGoods), WorkDate(WorkingDocuments)
        string fDocumentDate;
        [Size(19)]
        public string DocumentDate
        {
            get { return fDocumentDate; }
            set { SetPropertyValue<string>("DocumentDate", ref fDocumentDate, value); }
        }

        int fSelfBillingIndicator;
        public int SelfBillingIndicator
        {
            get { return fSelfBillingIndicator; }
            set { SetPropertyValue<int>("SelfBillingIndicator", ref fSelfBillingIndicator, value); }
        }

        int fCashVATSchemeIndicator;
        public int CashVatSchemeIndicator
        {
            get { return fCashVATSchemeIndicator; }
            set { SetPropertyValue<int>("CashVATSchemeIndicator", ref fCashVATSchemeIndicator, value); }
        }

        int fThirdPartiesBillingIndicator;
        public int ThirdPartiesBillingIndicator
        {
            get { return fThirdPartiesBillingIndicator; }
            set { SetPropertyValue<int>("ThirdPartiesBillingIndicator", ref fThirdPartiesBillingIndicator, value); }
        }

        //Used to Store 30Chars Codes for SAF-T
        string fDocumentCreatorUser;
        [Size(30)]
        public string DocumentCreatorUser
        {
            get { return fDocumentCreatorUser; }
            set { SetPropertyValue<string>("DocumentCreatorUser", ref fDocumentCreatorUser, value); }
        }

        string fEACCode;
        [Size(5)]
        public string EACCode
        {
            get { return fEACCode; }
            set { SetPropertyValue<string>("EACCode", ref fEACCode, value); }
        }

        string fSystemEntryDate;
        [Size(50)]
        public string SystemEntryDate
        {
            get { return fSystemEntryDate; }
            set { SetPropertyValue<string>("SystemEntryDate", ref fSystemEntryDate, value); }
        }

        string fTransactionID;
        [Size(70)]
        public string TransactionID
        {
            get { return fTransactionID; }
            set { SetPropertyValue<string>("TransactionID", ref fTransactionID, value); }
        }

        //MovementOfGoods: ShipTo
        string fShipToDeliveryID;
        [Size(255)]
        public string ShipToDeliveryID
        {
            get { return fShipToDeliveryID; }
            set { SetPropertyValue<string>("ShipToDeliveryID", ref fShipToDeliveryID, value); }
        }

        DateTime fShipToDeliveryDate;
        public DateTime ShipToDeliveryDate
        {
            get { return fShipToDeliveryDate; }
            set { SetPropertyValue<DateTime>("ShipToDeliveryDate", ref fShipToDeliveryDate, value); }
        }

        string fShipToWarehouseID;
        [Size(50)]
        public string ShipToWarehouseID
        {
            get { return fShipToWarehouseID; }
            set { SetPropertyValue<string>("ShipToWarehouseID", ref fShipToWarehouseID, value); }
        }

        string fShipToLocationID;
        [Size(30)]
        public string ShipToLocationID
        {
            get { return fShipToLocationID; }
            set { SetPropertyValue<string>("ShipToLocationID", ref fShipToLocationID, value); }
        }

        string fShipToBuildingNumber;
        [Size(10)]
        public string ShipToBuildingNumber
        {
            get { return fShipToBuildingNumber; }
            set { SetPropertyValue<string>("ShipToBuildingNumber", ref fShipToBuildingNumber, value); }
        }

        string fShipToStreetName;
        [Size(90)]
        public string ShipToStreetName
        {
            get { return fShipToStreetName; }
            set { SetPropertyValue<string>("ShipToStreetName", ref fShipToStreetName, value); }
        }

        string fShipToAddressDetail;
        [Size(100)]
        public string ShipToAddressDetail
        {
            get { return fShipToAddressDetail; }
            set { SetPropertyValue<string>("ShipToAddressDetail", ref fShipToAddressDetail, value); }
        }

        string fShipToCity;
        [Size(50)]
        public string ShipToCity
        {
            get { return fShipToCity; }
            set { SetPropertyValue<string>("ShipToCity", ref fShipToCity, value); }
        }

        string fShipToPostalCode;
        [Size(20)]
        public string ShipToPostalCode
        {
            get { return fShipToPostalCode; }
            set { SetPropertyValue<string>("ShipToPostalCode", ref fShipToPostalCode, value); }
        }

        string fShipToRegion;
        [Size(50)]
        public string ShipToRegion
        {
            get { return fShipToRegion; }
            set { SetPropertyValue<string>("ShipToRegion", ref fShipToRegion, value); }
        }

        string fShipToCountry;
        [Size(5)]
        public string ShipToCountry
        {
            get { return fShipToCountry; }
            set { SetPropertyValue<string>("ShipToCountry", ref fShipToCountry, value); }
        }

        //MovementOfGoods: ShipFrom
        string fShipFromDeliveryID;
        [Size(255)]
        public string ShipFromDeliveryID
        {
            get { return fShipFromDeliveryID; }
            set { SetPropertyValue<string>("ShipFromDeliveryID", ref fShipFromDeliveryID, value); }
        }

        DateTime fShipFromDeliveryDate;
        public DateTime ShipFromDeliveryDate
        {
            get { return fShipFromDeliveryDate; }
            set { SetPropertyValue<DateTime>("ShipFromDeliveryDate", ref fShipFromDeliveryDate, value); }
        }

        string fShipFromWarehouseID;
        [Size(50)]
        public string ShipFromWarehouseID
        {
            get { return fShipFromWarehouseID; }
            set { SetPropertyValue<string>("ShipFromWarehouseID", ref fShipFromWarehouseID, value); }
        }

        string fShipFromLocationID;
        [Size(30)]
        public string ShipFromLocationID
        {
            get { return fShipFromLocationID; }
            set { SetPropertyValue<string>("ShipFromLocationID", ref fShipFromLocationID, value); }
        }

        string fShipFromBuildingNumber;
        [Size(10)]
        public string ShipFromBuildingNumber
        {
            get { return fShipFromBuildingNumber; }
            set { SetPropertyValue<string>("ShipFromBuildingNumber", ref fShipFromBuildingNumber, value); }
        }

        string fShipFromStreetName;
        [Size(90)]
        public string ShipFromStreetName
        {
            get { return fShipFromStreetName; }
            set { SetPropertyValue<string>("ShipFromStreetName", ref fShipFromStreetName, value); }
        }

        string fShipFromAddressDetail;
        [Size(100)]
        public string ShipFromAddressDetail
        {
            get { return fShipFromAddressDetail; }
            set { SetPropertyValue<string>("ShipFromAddressDetail", ref fShipFromAddressDetail, value); }
        }

        string fShipFromCity;
        [Size(50)]
        public string ShipFromCity
        {
            get { return fShipFromCity; }
            set { SetPropertyValue<string>("ShipFromCity", ref fShipFromCity, value); }
        }

        string fShipFromPostalCode;
        [Size(20)]
        public string ShipFromPostalCode
        {
            get { return fShipFromPostalCode; }
            set { SetPropertyValue<string>("ShipFromPostalCode", ref fShipFromPostalCode, value); }
        }

        string fShipFromRegion;
        [Size(50)]
        public string ShipFromRegion
        {
            get { return fShipFromRegion; }
            set { SetPropertyValue<string>("ShipFromRegion", ref fShipFromRegion, value); }
        }

        string fShipFromCountry;
        [Size(5)]
        public string ShipFromCountry
        {
            get { return fShipFromCountry; }
            set { SetPropertyValue<string>("ShipFromCountry", ref fShipFromCountry, value); }
        }

        //MovementOfGoods: Common to ShipTo/ShipFrom
        DateTime fMovementStartTime;
        public DateTime MovementStartTime
        {
            get { return fMovementStartTime; }
            set { SetPropertyValue<DateTime>("MovementStartTime", ref fMovementStartTime, value); }
        }

        DateTime fMovementEndTime;
        public DateTime MovementEndTime
        {
            get { return fMovementEndTime; }
            set { SetPropertyValue<DateTime>("MovementEndTime", ref fMovementEndTime, value); }
        }

        /*EOF: SAF-T(PT)*/

        decimal fTotalNet;
        public decimal TotalNet
        {
            get { return fTotalNet; }
            set { SetPropertyValue<decimal>("TotalNet", ref fTotalNet, value); }
        }

        decimal fTotalGross;
        public decimal TotalGross
        {
            get { return fTotalGross; }
            set { SetPropertyValue<decimal>("TotalGross", ref fTotalGross, value); }
        }

        decimal fTotalDiscount;
        public decimal TotalDiscount
        {
            get { return fTotalDiscount; }
            set { SetPropertyValue<decimal>("TotalDiscount", ref fTotalDiscount, value); }
        }

        decimal fTotalTax;
        public decimal TotalTax
        {
            get { return fTotalTax; }
            set { SetPropertyValue<decimal>("TotalTax", ref fTotalTax, value); }
        }

        decimal fTotalFinal;
        public decimal TotalFinal
        {
            get { return fTotalFinal; }
            set { SetPropertyValue<decimal>("TotalFinal", ref fTotalFinal, value); }
        }

        decimal fTotalFinalRound;
        public decimal TotalFinalRound
        {
            get { return fTotalFinalRound; }
            set { SetPropertyValue<decimal>("TotalFinalRound", ref fTotalFinalRound, value); }
        }

        decimal fTotalDelivery;
        public decimal TotalDelivery
        {
            get { return fTotalDelivery; }
            set { SetPropertyValue<decimal>("TotalDelivery", ref fTotalDelivery, value); }
        }

        decimal fTotalChange;
        public decimal TotalChange
        {
            get { return fTotalChange; }
            set { SetPropertyValue<decimal>("TotalChange", ref fTotalChange, value); }
        }

        string fExternalDocument;
        [Size(50)]
        public string ExternalDocument
        {
            get { return fExternalDocument; }
            set { SetPropertyValue<string>("ExternalDocument", ref fExternalDocument, value); }
        }

        decimal fDiscount;
        public decimal Discount
        {
            get { return fDiscount; }
            set { SetPropertyValue<decimal>("Discount", ref fDiscount, value); }
        }

        decimal fDiscountFinancial;
        public decimal DiscountFinancial
        {
            get { return fDiscountFinancial; }
            set { SetPropertyValue<decimal>("DiscountFinancial", ref fDiscountFinancial, value); }
        }

        decimal fExchangeRate;
        public decimal ExchangeRate
        {
            get { return fExchangeRate; }
            set { SetPropertyValue<decimal>("ExchangeRate", ref fExchangeRate, value); }
        }

        Guid fEntityOid;
        public Guid EntityOid
        {
            get { return fEntityOid; }
            set { SetPropertyValue<Guid>("EntityOid", ref fEntityOid, value); }
        }

        //Used to Store 30Chars Codes for SAF-T
        string fEntityInternalCode;
        [Size(30)]
        public string EntityInternalCode
        {
            get { return fEntityInternalCode; }
            set { SetPropertyValue<string>("EntityInternalCode", ref fEntityInternalCode, value); }
        }

        string fEntityName;
        //[XPGuidObject(Encrypted = true)]
        public string EntityName
        {
            get { return fEntityName; }
            set { SetPropertyValue<string>("EntityName", ref fEntityName, value); }
        }

        string fEntityAddress;
        //[XPGuidObject(Encrypted = true)]
        public string EntityAddress
        {
            get { return fEntityAddress; }
            set { SetPropertyValue<string>("EntityAddress", ref fEntityAddress, value); }
        }

        string fEntityLocality;
        //[XPGuidObject(Encrypted = true)]
        public string EntityLocality
        {
            get { return fEntityLocality; }
            set { SetPropertyValue<string>("EntityLocality", ref fEntityLocality, value); }
        }

        string fEntityZipCode;
        [Size(10)]
        //[XPGuidObject(Encrypted = true)]
        public string EntityZipCode
        {
            get { return fEntityZipCode; }
            set { SetPropertyValue<string>("EntityZipCode", ref fEntityZipCode, value); }
        }

        string fEntityCity;
        //[XPGuidObject(Encrypted = true)]
        public string EntityCity
        {
            get { return fEntityCity; }
            set { SetPropertyValue<string>("EntityCity", ref fEntityCity, value); }
        }

        string fEntityCountry;
        [Size(5)]
        //[XPGuidObject(Encrypted = true)]
        public string EntityCountry
        {
            get { return fEntityCountry; }
            set { SetPropertyValue<string>("EntityCountry", ref fEntityCountry, value); }
        }

        Guid fEntityCountryOid;
        public Guid EntityCountryOid
        {
            get { return fEntityCountryOid; }
            set { SetPropertyValue<Guid>("EntityCountryOid", ref fEntityCountryOid, value); }
        }

        string fEntityFiscalNumber;
        //[XPGuidObject(Encrypted = true)]
        public string EntityFiscalNumber
        {
            get { return fEntityFiscalNumber; }
            set { SetPropertyValue<string>("FiscalNumber", ref fEntityFiscalNumber, value); }
        }

        Boolean fPayed;
        public Boolean Payed
        {
            get { return fPayed; }
            set { SetPropertyValue<Boolean>("Payed", ref fPayed, value); }
        }

        DateTime fPayedDate;
        public DateTime PayedDate
        {
            get { return fPayedDate; }
            set { SetPropertyValue<DateTime>("PayedDate", ref fPayedDate, value); }
        }

        Boolean fPrinted;
        public Boolean Printed
        {
            get { return fPrinted; }
            set { SetPropertyValue<Boolean>("Printed", ref fPrinted, value); }
        }

        FIN_DocumentOrderMain fSourceOrderMain;
        public FIN_DocumentOrderMain SourceOrderMain
        {
            get { return fSourceOrderMain; }
            set { SetPropertyValue<FIN_DocumentOrderMain>("SourceOrderMain", ref fSourceOrderMain, value); }
        }

        FIN_DocumentFinanceMaster fDocumentParent;
        public FIN_DocumentFinanceMaster DocumentParent
        {
            get { return fDocumentParent; }
            set { SetPropertyValue<FIN_DocumentFinanceMaster>("DocumentParent", ref fDocumentParent, value); }
        }

        FIN_DocumentFinanceMaster fDocumentChild;
        public FIN_DocumentFinanceMaster DocumentChild
        {
            get { return fDocumentChild; }
            set { SetPropertyValue<FIN_DocumentFinanceMaster>("DocumentChild", ref fDocumentChild, value); }
        }

        //WayBill Code
        string fATDocCodeID;
        [Size(200)]
        public string ATDocCodeID
        {
            get { return fATDocCodeID; }
            set { SetPropertyValue<string>("ATDocCodeID", ref fATDocCodeID, value); }
        }

        //AT WebServices
        SYS_SystemAuditAT fATValidAuditResult;
        public SYS_SystemAuditAT ATValidAuditResult
        {
            get { return fATValidAuditResult; }
            set { SetPropertyValue<SYS_SystemAuditAT>("ATValidAuditResult", ref fATValidAuditResult, value); }
        }

        Boolean fATResendDocument;
        public Boolean ATResendDocument
        {
            get { return fATResendDocument; }
            set { SetPropertyValue<Boolean>("ATResendDocument", ref fATResendDocument, value); }
        }

        //DocumentFinanceMaster One <> Many SystemAuditATWS
        [Association(@"DocumentFinanceMasterReferencesSystemAuditAT", typeof(SYS_SystemAuditAT))]
        public XPCollection<SYS_SystemAuditAT> ATAudit
        {
            get { return GetCollection<SYS_SystemAuditAT>("ATAudit"); }
        }

        //DocumentFinanceMaster One <> Many DocumentFinanceDetail
        [Association(@"DocumentFinanceMasterReferencesDocumentFinanceDetail", typeof(FIN_DocumentFinanceDetail))]
        public XPCollection<FIN_DocumentFinanceDetail> DocumentDetail
        {
            get { return GetCollection<FIN_DocumentFinanceDetail>("DocumentDetail"); }
        }

        //DocumentFinanceMaster One <> Many DocumentFinanceMasterTotal
        [Association(@"DocumentFinanceMasterReferencesDocumentFinanceMasterTotal", typeof(FIN_DocumentFinanceMasterTotal))]
        public XPCollection<FIN_DocumentFinanceMasterTotal> Totals
        {
            get { return GetCollection<FIN_DocumentFinanceMasterTotal>("Totals"); }
        }

        //DocumentFinanceType One <> Many DocumentFinanceMaster
        FIN_DocumentFinanceType fDocumentType;
        [Association(@"DocumentFinanceTypeReferencesDocumentFinanceMaster")]
        public FIN_DocumentFinanceType DocumentType
        {
            get { return fDocumentType; }
            set { SetPropertyValue<FIN_DocumentFinanceType>("DocumentType", ref fDocumentType, value); }
        }

        //DocumentFinanceSeries One <> Many DocumentFinanceMaster
        FIN_DocumentFinanceSeries fDocumentSerie;
        [Association(@"DocumentFinanceSeriesReferencesDocumentFinanceMaster")]
        public FIN_DocumentFinanceSeries DocumentSerie
        {
            get { return fDocumentSerie; }
            set { SetPropertyValue<FIN_DocumentFinanceSeries>("DocumentSerie", ref fDocumentSerie, value); }
        }

        //ConfigurationPaymentMethod One <> Many DocumentFinanceMaster
        FIN_ConfigurationPaymentMethod fPaymentMethod;
        [Association(@"ConfigurationPaymentMethodReferencesDocumentFinanceMaster")]
        public FIN_ConfigurationPaymentMethod PaymentMethod
        {
            get { return fPaymentMethod; }
            set { SetPropertyValue<FIN_ConfigurationPaymentMethod>("PaymentMethod", ref fPaymentMethod, value); }
        }

        //ConfigurationPaymentCondition One <> Many DocumentFinanceMaster
        FIN_ConfigurationPaymentCondition fPaymentCondition;
        [Association(@"ConfigurationPaymentConditionReferencesDocumentFinanceMaster")]
        public FIN_ConfigurationPaymentCondition PaymentCondition
        {
            get { return fPaymentCondition; }
            set { SetPropertyValue<FIN_ConfigurationPaymentCondition>("PaymentCondition", ref fPaymentCondition, value); }
        }

        //ConfigurationCurrency One <> Many DocumentFinanceMaster
        CFG_ConfigurationCurrency fCurrency;
        [Association(@"ConfigurationCurrencyReferencesDocumentFinanceMaster")]
        public CFG_ConfigurationCurrency Currency
        {
            get { return fCurrency; }
            set { SetPropertyValue<CFG_ConfigurationCurrency>("Currency", ref fCurrency, value); }
        }

        //DocumentFinanceMasterPayment Many <> Many DocumentFinanceMaster
        [Association(@"DocumentFinanceMasterPaymentReferencesDocumentFinanceMaster", typeof(FIN_DocumentFinanceMasterPayment))]
        public XPCollection<FIN_DocumentFinanceMasterPayment> DocumentPayment
        {
            get { return GetCollection<FIN_DocumentFinanceMasterPayment>("DocumentPayment"); }
        }

        //DocumentFinanceMaster One <> Many SystemPrint
        [Association(@"DocumentFinanceMasterReferencesSystemPrint", typeof(SYS_SystemPrint))]
        public XPCollection<SYS_SystemPrint> SystemPrint
        {
            get { return GetCollection<SYS_SystemPrint>("SystemPrint"); }
        }

////SystemNotification One <> Many DocumentFinanceMaster
//SYS_SystemNotification fNotification;
//[Association(@"SystemNotificationReferencesDocumentFinanceMaster")]
//public SYS_SystemNotification Notification
//{
//    get { return fNotification; }
//    set { SetPropertyValue<SYS_SystemNotification>("Notification", ref fNotification, value); }
//}

////SystemNotification One <> Many DocumentFinanceMaster
//[Association(@"SystemNotificationReferencesDocumentFinanceMaster", typeof(SYS_SystemNotification))]
//public XPCollection<SYS_SystemNotification> Notification
//{
//    get { return GetCollection<SYS_SystemNotification>("Notification"); }
//}

////DocumentFinanceMaster Many <> Many SystemNotification
//[Association(@"DocumentFinanceMasterReferenceSystemNotification", typeof(SYS_SystemNotification))]
//public XPCollection<SYS_SystemNotification> Notification
//{
//    get { return GetCollection<SYS_SystemNotification>("Notification"); }
//}

        //SystemNotification One <> Many DocumentFinanceMaster
        [Association(@"DocumentFinanceMasterReferenceSystemNotification", typeof(SYS_SystemNotificationDocumentMaster))]
        public XPCollection<SYS_SystemNotificationDocumentMaster> Notifications
        {
            get { return GetCollection<SYS_SystemNotificationDocumentMaster>("Notifications"); }
        }
    }
}
