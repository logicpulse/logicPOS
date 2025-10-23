using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Finance.Documents.Common;
using System;

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
        public AgtInfo AgtInfo { get; set; }
       
        public string GetAgtStatus()
        {
            if (AgtInfo == null || string.IsNullOrWhiteSpace(AgtInfo.RequestId))
            {
                return "Não submetido";
            }

            if (string.IsNullOrWhiteSpace(AgtInfo.ValidationStatus))
            {
                return "Submetido (Não validado)";
            }

            return $"Submetido ({AgtInfo.ValidationStatus})";
        }
        
        public bool HasPassed48Hours => CreatedAt.AddHours(48) < DateTime.Now;

    }
}
