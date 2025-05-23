using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Entities
{
    public class Receipt : ApiEntity
    {
        public string RefNo { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public string  Customer { get; set; }
        public string CustomerFiscalNumber { get; set; }
        public bool IsCancelled => Status == "A";
        public bool HasPassed48Hours => CreatedAt.AddHours(48) < DateTime.Now;
    }
}
