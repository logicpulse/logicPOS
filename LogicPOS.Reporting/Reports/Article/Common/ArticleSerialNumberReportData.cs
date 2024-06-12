using LogicPOS.Reporting.Data.Common;

namespace LogicPOS.Reporting.Reports.Data
{
    [ReportData(Entity = "fin_articleserialnumber")]
    public class ArticleSerialNumberReportData : ReportData
    {
        public string SerialNumber { get; set; }
        public string ArticleName { get; set; }
        public string ArticleRef { get; set; }
        public string footerText { get; set; }
    }
}
