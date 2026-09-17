using System;

namespace LogicPOS.Api.Features.Documents.Details
{
    public class DocumentDetailTax
    {
        public Guid TaxId { get; set; }
        public string Designation { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal Percentage { get; set; }
        public string CountryRegion { get; set; }
    }
}
