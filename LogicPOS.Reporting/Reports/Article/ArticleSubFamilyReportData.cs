using LogicPOS.Reporting.Data.Common;
using System.Collections.Generic;

namespace LogicPOS.Reporting.Reports.Data
{
    [ReportData(Entity = "fin_articlesubfamily")]
    public class ArticleSubFamilyReportData : ReportData
    {
        public uint Ord { get; set; }
        public uint Code { get; set; }
        public string Designation { get; set; }
        public string ButtonLabel { get; set; }
        public bool ButtonLabelHide { get; set; }
        public string ButtonImage { get; set; }
        public string ButtonIcon { get; set; }

        public List<ArticleReportData> Article { get; set; }
    }
}
