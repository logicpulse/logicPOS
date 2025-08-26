using LogicPOS.Api.Entities;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Reports.WorkSession.Common
{
    public class WorkSessionData
    {
        public WorkSessionPeriod WorkSession { get; set; }
        public decimal OpenTotal { get; set; }
        public decimal Total { get; set; }
        public decimal TotalIn { get; set; }
        public decimal TotalOut { get; set; }
        public string Currency { get; set; }
        public List<FamilyReportItem> FamilyReportItems { get; set; }
        public List<SubfamilyReportItem> SubfamilyReportItems { get; set; }
        public List<ArticleReportItem> ArticleReportItems { get; set; }
        public List<VatRateReportItem> TaxReportItems { get; set; }
        public List<PaymentReportItem> PaymentReportItems { get; set; }
        public List<DocumentTypeReportItem> DocumentTypeReportItems { get; set; }
        public List<HoursReportItem> HoursReportItems { get; set; }
        public List<UserReportItem> UserReportItems { get; set; }
    }
}
