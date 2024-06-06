using LogicPOS.Reporting.Common;

namespace LogicPOS.Reporting.Reports.Articles
{
    [Report(Entity = "fin_articleserialnumber")]
    public class ArticleSerialNumberReport : ReportData
    {
        public string SerialNumber { get; set; }
        public string ArticleName { get; set; }
        public string ArticleRef { get; set; }
        public string footerText { get; set; }
    }
}
