using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Reports.WorkSession.Common
{
    public class WorkSessionData
    {
        public WorkSessionData()
        {
            FamilyReportItems = new List<FamilyReportItem>();
            SubfamilyReportItems = new List<SubfamilyReportItem>();
            ArticleReportItems = new List<ArticleReportItem>();
            TaxReportItems = new List<VatRateReportItem>();
        }
        
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal OpenTotal { get; set; }
        public decimal Total { get; set; }
        public decimal TotalIn { get; set; }
        public decimal TotalOut { get; set; }
        public List<FamilyReportItem> FamilyReportItems { get; set; }
        public List<SubfamilyReportItem> SubfamilyReportItems { get; set; }
        public List<ArticleReportItem> ArticleReportItems { get; set; }
        public List<VatRateReportItem> TaxReportItems { get; set; }
    }
}
