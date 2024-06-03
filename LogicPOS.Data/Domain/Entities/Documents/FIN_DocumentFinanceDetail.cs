using System;
using DevExpress.Xpo;
using LogicPOS.Domain.Enums;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class fin_documentfinancedetail : Entity
    {
        public fin_documentfinancedetail() : base() { }
        public fin_documentfinancedetail(Session session) : base(session) { }

        private uint fOrd;
        public uint Ord
        {
            get { return fOrd; }
            set { SetPropertyValue("Ord", ref fOrd, value); }
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

        private decimal fQuantity;
        public decimal Quantity
        {
            get { return fQuantity; }
            set { SetPropertyValue<decimal>("Quantity", ref fQuantity, value); }
        }

        private string fUnitMeasure;
        [Size(35)]
        public string UnitMeasure
        {
            get { return fUnitMeasure; }
            set { SetPropertyValue<string>("UnitMeasure", ref fUnitMeasure, value); }
        }

        private decimal fPrice;
        public decimal Price
        {
            get { return fPrice; }
            set { SetPropertyValue<decimal>("Price", ref fPrice, value); }
        }

        private decimal fVat;
        public decimal Vat
        {
            get { return fVat; }
            set { SetPropertyValue<decimal>("Vat", ref fVat, value); }
        }

        private string fVatExemptionReasonDesignation;
        [Size(255)]
        public string VatExemptionReasonDesignation
        {
            get { return fVatExemptionReasonDesignation; }
            set { SetPropertyValue<string>("VatExemptionReasonDesignation", ref fVatExemptionReasonDesignation, value); }
        }

        private decimal fDiscount;
        public decimal Discount
        {
            get { return fDiscount; }
            set { SetPropertyValue<decimal>("Discount", ref fDiscount, value); }
        }

        private decimal fTotalNet;
        public decimal TotalNet
        {
            get { return fTotalNet; }
            set { SetPropertyValue<decimal>("TotalNet", ref fTotalNet, value); }
        }

        private decimal fTotalGross;
        public decimal TotalGross
        {
            get { return fTotalGross; }
            set { SetPropertyValue<decimal>("TotalGross", ref fTotalGross, value); }
        }

        private decimal fTotalDiscount;
        public decimal TotalDiscount
        {
            get { return fTotalDiscount; }
            set { SetPropertyValue<decimal>("TotalDiscount", ref fTotalDiscount, value); }
        }

        private decimal fTotalTax;
        public decimal TotalTax
        {
            get { return fTotalTax; }
            set { SetPropertyValue<decimal>("TotalTax", ref fTotalTax, value); }
        }

        private decimal fTotalFinal;
        public decimal TotalFinal
        {
            get { return fTotalFinal; }
            set { SetPropertyValue<decimal>("TotalFinal", ref fTotalFinal, value); }
        }

        //Final Calculated Priced, Usefull to Clone Document ex From SourceDocuments in New Document Window
        private PriceType fPriceType;
        public PriceType PriceType
        {
            get { return fPriceType; }
            set { SetPropertyValue("PriceType", ref fPriceType, value); }
        }

        //Final Calculated Priced, Usefull to Clone Document ex From SourceDocuments in New Document Window
        private decimal fPriceFinal;
        public decimal PriceFinal
        {
            get { return fPriceFinal; }
            set { SetPropertyValue<decimal>("PriceFinal", ref fPriceFinal, value); }
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
            set { SetPropertyValue("DocumentMaster", ref fDocumentMaster, value); }
        }

        //Article One <> Many DocumentOrderDetail
        private fin_article fArticle;
        [Association(@"ArticleReferencesDocumentDocumentFinanceDetail")]
        public fin_article Article
        {
            get { return fArticle; }
            set { SetPropertyValue("Article", ref fArticle, value); }
        }

        //ConfigurationVatRate One <> Many DocumentOrderDetail
        private fin_configurationvatrate fVatRate;
        [Association(@"ConfigurationVatRateReferencesDocumentDocumentFinanceDetail")]
        public fin_configurationvatrate VatRate
        {
            get { return fVatRate; }
            set { SetPropertyValue("VatRate", ref fVatRate, value); }
        }

        //ConfigurationVatExemptionReason One <> Many DocumentOrderDetail
        private fin_configurationvatexemptionreason fVatExemptionReason;
        [Association(@"ConfigurationVatExemptionReasonReferencesDocumentDocumentFinanceDetail")]
        public fin_configurationvatexemptionreason VatExemptionReason
        {
            get { return fVatExemptionReason; }
            set { SetPropertyValue("VatExemptionReason", ref fVatExemptionReason, value); }
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
