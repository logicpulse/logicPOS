namespace LogicPOS.DTOs.Reporting
{

    public class FinanceDetailReportDto
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

        public decimal ArticlePriceWithDiscount
        {
            get
            {
                decimal totalDiscount = TotalGross * Discount / 100;
                return TotalGross - totalDiscount;
            }

        } 

        public decimal ArticlePriceAfterTax
        {
            get
            {
                decimal totalTax = ArticlePriceWithDiscount * Vat / 100;
                return ArticlePriceWithDiscount + totalTax;
            }
        }

        public decimal UnitPrice
        {
            get
            {
                decimal totalTax = ArticlePriceWithDiscount * Vat / 100;
                return (ArticlePriceWithDiscount + totalTax) / Quantity;
            }
        }

       
    }
}
