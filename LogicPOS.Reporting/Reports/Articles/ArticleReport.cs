using LogicPOS.Reporting.Common;

namespace LogicPOS.Reporting.Reports.Articles
{
    [Report(Entity = "fin_article")]
    public class ArticleReport : ReportBase
    {
        public uint Ord { get; set; }
        public string Code { get; set; }
        public string CodeDealer { get; set; }
        public string Designation { get; set; }
        public string ButtonLabel { get; set; }
        public bool ButtonLabelHide { get; set; }
        public string ButtonImage { get; set; }
        public string ButtonIcon { get; set; }
        public decimal Price1 { get; set; }
        public decimal Price2 { get; set; }
        public decimal Price3 { get; set; }
        public decimal Price4 { get; set; }
        public decimal Price5 { get; set; }
        public decimal Price1Promotion { get; set; }
        public decimal Price2Promotion { get; set; }
        public decimal Price3Promotion { get; set; }
        public decimal Price4Promotion { get; set; }
        public decimal Price5Promotion { get; set; }
        public bool Price1UsePromotionPrice { get; set; }
        public bool Price2UsePromotionPrice { get; set; }
        public bool Price3UsePromotionPrice { get; set; }
        public bool Price4UsePromotionPrice { get; set; }
        public bool Price5UsePromotionPrice { get; set; }
        public bool PriceWithVat { get; set; }
        public decimal Discount { get; set; }
        public decimal DefaultQuantity { get; set; }
        public decimal Accounting { get; set; }
        public decimal Tare { get; set; }
        public decimal Weight { get; set; }
        public string BarCode { get; set; }
        public bool PVPVariable { get; set; }
        public bool Favorite { get; set; }
        public string Token1 { get; set; }
        public string Token2 { get; set; }
    }
}
