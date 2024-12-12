using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Entities
{
    public class Receipt : ApiEntity
    {
        public PaymentMethod PaymentMethod { get; set; }
        public Guid PaymentMethodId { get; set; }
        public string RefNo { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
     
        public bool IsCancelled => Status == "A";
        public bool HasPassed48Hours => CreatedAt.AddHours(48) < DateTime.Now;
    }
}
