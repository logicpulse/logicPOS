using LogicPOS.Reporting.Data.Common;

namespace LogicPOS.Reporting.Reports.Data
{
    //Now Entity is Required to be defined, since implementation of Table Prefix
    [ReportData(Entity = "fin_documentfinancedetail")]
    public class FinanceDetailReportData : ReportData
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
        [ReportData(Hide = true)]
        public decimal ArticlePriceWithDiscount
        {
            get
            {
                decimal totalDiscount = TotalGross * Discount / 100;
                return TotalGross - totalDiscount;
            }
            set { }
        } // TotalGross - Discount
        [ReportData(Hide = true)]
        public decimal ArticlePriceAfterTax
        {
            get
            {
                decimal totalTax = ArticlePriceWithDiscount * Vat / 100;
                return ArticlePriceWithDiscount + totalTax;
            }
            set { }
        }
        //Layout talões PT - Preço Unitário em vez de Preço sem IVA [IN:016509]
        [ReportData(Hide = true)]
        public decimal UnitPrice
        {
            get
            {
                decimal totalTax = ArticlePriceWithDiscount * Vat / 100;
                return (ArticlePriceWithDiscount + totalTax) / Quantity;
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
