using System;
using DevExpress.Xpo;
using logicpos.datalayer.Enums;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class fin_documentfinancedetail : XPGuidObject
    {
        public fin_documentfinancedetail() : base() { }
        public fin_documentfinancedetail(Session session) : base(session) { }

        private UInt32 fOrd;
        public UInt32 Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        private string fCode;
        //[Indexed(Unique = true)] - Can have equal Article.Codes with Diferent Properties
        public string Code
        {
            get { return fCode; }
            set { SetPropertyValue<string>("Code", ref fCode, value); }
        }

        private string fDesignation;
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        private Decimal fQuantity;
        public Decimal Quantity
        {
            get { return fQuantity; }
            set { SetPropertyValue<Decimal>("Quantity", ref fQuantity, value); }
        }

        private string fUnitMeasure;
        [Size(35)]
        public string UnitMeasure
        {
            get { return fUnitMeasure; }
            set { SetPropertyValue<string>("UnitMeasure", ref fUnitMeasure, value); }
        }

        private Decimal fPrice;
        public Decimal Price
        {
            get { return fPrice; }
            set { SetPropertyValue<Decimal>("Price", ref fPrice, value); }
        }

        private Decimal fVat;
        public Decimal Vat
        {
            get { return fVat; }
            set { SetPropertyValue<Decimal>("Vat", ref fVat, value); }
        }

        private string fVatExemptionReasonDesignation;
        [Size(255)]
        public string VatExemptionReasonDesignation
        {
            get { return fVatExemptionReasonDesignation; }
            set { SetPropertyValue<string>("VatExemptionReasonDesignation", ref fVatExemptionReasonDesignation, value); }
        }

        private Decimal fDiscount;
        public Decimal Discount
        {
            get { return fDiscount; }
            set { SetPropertyValue<Decimal>("Discount", ref fDiscount, value); }
        }

        private Decimal fTotalNet;
        public Decimal TotalNet
        {
            get { return fTotalNet; }
            set { SetPropertyValue<Decimal>("TotalNet", ref fTotalNet, value); }
        }

        private Decimal fTotalGross;
        public Decimal TotalGross
        {
            get { return fTotalGross; }
            set { SetPropertyValue<Decimal>("TotalGross", ref fTotalGross, value); }
        }

        private Decimal fTotalDiscount;
        public Decimal TotalDiscount
        {
            get { return fTotalDiscount; }
            set { SetPropertyValue<Decimal>("TotalDiscount", ref fTotalDiscount, value); }
        }

        private Decimal fTotalTax;
        public Decimal TotalTax
        {
            get { return fTotalTax; }
            set { SetPropertyValue<Decimal>("TotalTax", ref fTotalTax, value); }
        }

        private Decimal fTotalFinal;
        public Decimal TotalFinal
        {
            get { return fTotalFinal; }
            set { SetPropertyValue<Decimal>("TotalFinal", ref fTotalFinal, value); }
        }

        //Final Calculated Priced, Usefull to Clone Document ex From SourceDocuments in New Document Window
        private PriceType fPriceType;
        public PriceType PriceType
        {
            get { return fPriceType; }
            set { SetPropertyValue<PriceType>("PriceType", ref fPriceType, value); }
        }

        //Final Calculated Priced, Usefull to Clone Document ex From SourceDocuments in New Document Window
        private Decimal fPriceFinal;
        public Decimal PriceFinal
        {
            get { return fPriceFinal; }
            set { SetPropertyValue<Decimal>("PriceFinal", ref fPriceFinal, value); }
        }

        //Custom Properties
        private string fToken1;
        [Size(255)]
        public string Token1
        {
            get { return fToken1; }
            set { SetPropertyValue<string>("Token1", ref fToken1, value); }
        }

        //Custom Properties
        private string fToken2;
        [Size(255)]
        public string Token2
        {
            get { return fToken2; }
            set { SetPropertyValue<string>("Token2", ref fToken2, value); }
        }

        //Serial Number
        private string fSerialNumber;
        [Size(255)]
        public string SerialNumber
        {
            get { return fSerialNumber; }
            set { SetPropertyValue<string>("SerialNumber", ref fSerialNumber, value); }
        }

        //Serial Number
        private string fWarehouse;
        [Size(255)]
        public string Warehouse
        {
            get { return fWarehouse; }
            set { SetPropertyValue<string>("Warehouse", ref fWarehouse, value); }
        }

        //DocumentFinanceMaster One <> Many DocumentFinanceDetail
        private fin_documentfinancemaster fDocumentMaster;
        [Association(@"DocumentFinanceMasterReferencesDocumentFinanceDetail")]
        public fin_documentfinancemaster DocumentMaster
        {
            get { return fDocumentMaster; }
            set { SetPropertyValue<fin_documentfinancemaster>("DocumentMaster", ref fDocumentMaster, value); }
        }

        //Article One <> Many DocumentOrderDetail
        private fin_article fArticle;
        [Association(@"ArticleReferencesDocumentDocumentFinanceDetail")]
        public fin_article Article
        {
            get { return fArticle; }
            set { SetPropertyValue<fin_article>("Article", ref fArticle, value); }
        }

        //ConfigurationVatRate One <> Many DocumentOrderDetail
        private fin_configurationvatrate fVatRate;
        [Association(@"ConfigurationVatRateReferencesDocumentDocumentFinanceDetail")]
        public fin_configurationvatrate VatRate
        {
            get { return fVatRate; }
            set { SetPropertyValue<fin_configurationvatrate>("VatRate", ref fVatRate, value); }
        }

        //ConfigurationVatExemptionReason One <> Many DocumentOrderDetail
        private fin_configurationvatexemptionreason fVatExemptionReason;
        [Association(@"ConfigurationVatExemptionReasonReferencesDocumentDocumentFinanceDetail")]
        public fin_configurationvatexemptionreason VatExemptionReason
        {
            get { return fVatExemptionReason; }
            set { SetPropertyValue<fin_configurationvatexemptionreason>("VatExemptionReason", ref fVatExemptionReason, value); }
        }

        //DocumentFinanceDetail One <> Many DocumentFinanceDetailReferencesDocumentFinanceDetailOrderReference
        [Association(@"DocumentFinanceDetailReferencesDocumentFinanceDetailOrderReference", typeof(fin_documentfinancedetailorderreference))]
        public XPCollection<fin_documentfinancedetailorderreference> OrderReferences
        {
            get { return GetCollection<fin_documentfinancedetailorderreference>("OrderReferences"); }
        }

        //DocumentFinanceDetail One <> Many DocumentFinanceDetailReference
        [Association(@"DocumentFinanceDetailReferencesDocumentFinanceDetailReference", typeof(fin_documentfinancedetailreference))]
        public XPCollection<fin_documentfinancedetailreference> References
        {
            get { return GetCollection<fin_documentfinancedetailreference>("References"); }
        }
    }
}
