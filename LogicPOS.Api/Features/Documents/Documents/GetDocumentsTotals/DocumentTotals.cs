using System;

namespace LogicPOS.Api.Features.Documents.GetDocumentsTotals
{
    public class DocumentTotals
    {
        public Guid DocumentId { get; set; }
        public decimal TotalFinal { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalToPay { get; set; }
    }
}
