using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class fin_documentorderdetail : XPGuidObject
    {
        public fin_documentorderdetail() : base() { }
        public fin_documentorderdetail(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(fin_documentfinanceyearserieterminal), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(fin_documentfinanceyearserieterminal), "Code").ToString();
        }

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

        private Decimal fDiscount;
        public Decimal Discount
        {
            get { return fDiscount; }
            set { SetPropertyValue<Decimal>("Discount", ref fDiscount, value); }
        }

        //Decimal fDiscountGlobal;
        //public Decimal DiscountGlobal
        //{
        //  get { return fDiscountGlobal; }
        //  set { SetPropertyValue<Decimal>("DiscountGlobal", ref fDiscountGlobal, value); }
        //}

        private Decimal fVat;
        public Decimal Vat
        {
            get { return fVat; }
            set { SetPropertyValue<Decimal>("Vat", ref fVat, value); }
        }

        private Guid fVatExemptionReason;
        public Guid VatExemptionReason
        {
            get { return fVatExemptionReason; }
            set { SetPropertyValue<Guid>("VatExemptionReason", ref fVatExemptionReason, value); }
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

        private string fToken1;
        [Size(255)]
        public string Token1
        {
            get { return fToken1; }
            set { SetPropertyValue<string>("Token1", ref fToken1, value); }
        }

        private string fToken2;
        [Size(255)]
        public string Token2
        {
            get { return fToken2; }
            set { SetPropertyValue<string>("Token2", ref fToken2, value); }
        }

        //DocumentOrderTicket One <> Many DocumentOrderDetail
        private fin_documentorderticket fOrderTicket;
        [Association(@"DocumentOrderTicketReferencesDocumentOrderDetail")]
        public fin_documentorderticket OrderTicket
        {
            get { return fOrderTicket; }
            set { SetPropertyValue<fin_documentorderticket>("OrderTicket", ref fOrderTicket, value); }
        }

        //Article One <> Many DocumentOrderDetail
        private fin_article fArticle;
        [Association(@"ArticleReferencesDocumentOrderDetail")]
        public fin_article Article
        {
            get { return fArticle; }
            set { SetPropertyValue<fin_article>("Article", ref fArticle, value); }
        }
    }
}
