using DevExpress.Xpo;
using LogicPOS.Data.XPO.Utility;
using System;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class fin_documentorderdetail : Entity
    {
        public fin_documentorderdetail() : base() { }
        public fin_documentorderdetail(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = XPOUtility.GetNextTableFieldID(nameof(fin_documentfinanceyearserieterminal), "Ord");
            Code = XPOUtility.GetNextTableFieldID(nameof(fin_documentfinanceyearserieterminal), "Code").ToString();
        }

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

        private decimal fDiscount;
        public decimal Discount
        {
            get { return fDiscount; }
            set { SetPropertyValue<decimal>("Discount", ref fDiscount, value); }
        }

        //Decimal fDiscountGlobal;
        //public Decimal DiscountGlobal
        //{
        //  get { return fDiscountGlobal; }
        //  set { SetPropertyValue<Decimal>("DiscountGlobal", ref fDiscountGlobal, value); }
        //}

        private decimal fVat;
        public decimal Vat
        {
            get { return fVat; }
            set { SetPropertyValue<decimal>("Vat", ref fVat, value); }
        }

        private Guid fVatExemptionReason;
        public Guid VatExemptionReason
        {
            get { return fVatExemptionReason; }
            set { SetPropertyValue<Guid>("VatExemptionReason", ref fVatExemptionReason, value); }
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
            set { SetPropertyValue("OrderTicket", ref fOrderTicket, value); }
        }

        //Article One <> Many DocumentOrderDetail
        private fin_article fArticle;
        [Association(@"ArticleReferencesDocumentOrderDetail")]
        public fin_article Article
        {
            get { return fArticle; }
            set { SetPropertyValue("Article", ref fArticle, value); }
        }
    }
}
