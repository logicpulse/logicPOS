namespace logicpos.financial.library.Classes.Reports.BOs.Documents
{
    //Now Entity is Required to be defined, since implementation of Table Prefix
    [FRBO(Entity = "fin_documentfinancedetail")]
    public class FRBODocumentFinanceDetail : FRBOBaseObject
    {
        public string Code { get; set; }
        public string Designation { get; set; }
        public decimal Quantity { get; set; }
        public string UnitMeasure { get; set; }
        public decimal Price { get; set; }
        public decimal Vat { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalNet { get; set; }
        public decimal TotalGross { get; set; }
        public decimal TotalFinal { get; set; }
        public string Notes { get; set; }
        public string VatExemptionReasonDesignation { get; set; }

		/* IN009206 block */
        //[FRBO(Field = "fdPriceWithDiscount")]   // AVG((fdPrice - ((fdPrice * fdDiscount) / 100))) AS ArticlePriceWithDiscount
        // fdPriceWithDiscount
        [FRBO(Hide = true)]
        public decimal ArticlePriceWithDiscount {
            get{
                decimal totalDiscount = (this.TotalGross * this.Discount) / 100;
                return this.TotalGross - totalDiscount;
            }
            set { }
        } // TotalGross - Discount
        [FRBO(Hide = true)]
        public decimal ArticlePriceAfterTax
        {
            get
            {
                decimal totalTax = (this.ArticlePriceWithDiscount * this.Vat) / 100;
                return this.ArticlePriceWithDiscount + totalTax;
            }
            set { }
        }

        //// Enums
        //public PriceType PriceType { get; set; }
        //// Navigation Properties
        //public fin_documentfinancemaster DocumentMaster { get; set; }
        //public fin_article Article { get; set; }
        //public fin_configurationvatrate VatRate { get; set; }
    }
}
