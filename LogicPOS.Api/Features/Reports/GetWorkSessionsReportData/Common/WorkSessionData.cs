using LogicPOS.Api.Entities;
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
            PaymentReportItems = new List<PaymentReportItem>();
            TaxReportItems = new List<VatRateReportItem>();
            DocumentTypeReportItems = new List<DocumentTypeReportItem>();
            HoursReportItems = new List<HoursReportItem>();
            UserReportItems = new List<UserReportItem>();
        }
        public WorkSessionPeriod WorkSession {  get; set; }
        public decimal OpenTotal { get; set; }
        public decimal Total { get; set; }
        public decimal TotalIn { get; set; }
        public decimal TotalOut { get; set; }
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
