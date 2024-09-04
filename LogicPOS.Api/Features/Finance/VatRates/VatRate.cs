using LogicPOS.Api.Features.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogicPOS.Api.Entities
{
    public class VatRate : ApiEntity, IWithDesignation, IWithCode
    {
        public uint Order {  get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public decimal Value { get; set; }
        public string ReasonCode { get; set; }
        public string TaxType { get; set; }
        public string TaxCode { get; set; }
        public string CountryRegion { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Description { get; set; }
    }
}
