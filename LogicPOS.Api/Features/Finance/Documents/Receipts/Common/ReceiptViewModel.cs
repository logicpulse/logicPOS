using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Finance.Documents.Common;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Entities
{
    public class ReceiptViewModel : ApiEntity
    {
        public string RefNo { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public string Customer { get; set; }
        public string CustomerFiscalNumber { get; set; }
        public bool IsCancelled => Status == "A";
        public List<string> RelatedDocuments { get; set; }
        public AgtDocumentInfo Agt { get; set; }
       
        public string GetAgtStatus()
        {
            if (string.IsNullOrWhiteSpace(Agt.RequestId))
            {
                return "Não submetido";
            }

            if (string.IsNullOrWhiteSpace(Agt.ValidationStatus))
            {
                return "Submetido (Não validado)";
            }

            return $"Submetido ({Agt.ValidationStatus})";
        }
        
        public bool HasPassed48Hours => CreatedAt.AddHours(48) < DateTime.Now;

    }
}
