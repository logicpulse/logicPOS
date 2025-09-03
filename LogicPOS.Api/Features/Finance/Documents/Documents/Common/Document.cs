using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Documents;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Entities
{
    public class Document : ApiEntity
    {
        public DocumentSeries Series { get; set; }
        public Guid SeriesId { get; set; }
        public Guid CustomerId { get; set; }
        public string Type { get; set; }
        public string Number { get; set; }
        public ShipAddress ShipToAdress { get; set; }
        public ShipAddress ShipFromAdress { get; set; }
        public decimal Discount { get; set; }
        public DocumentCustomer Customer { get; set; }
        public IList<DocumentDetail> Details { get; set; }
        public DocumentTypeAnalyzer TypeAnalyzer => new DocumentTypeAnalyzer(Type);
    }
}
