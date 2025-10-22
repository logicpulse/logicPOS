using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Finance.Documents.Documents.GetDocumentPreviewData
{
    public class GetDocumentPreviewDataQuery : IRequest<ErrorOr<Document>>
    {
        public Guid? CurrencyId { get; set; }
        public string Type { get; set; }
        public ShipAddress ShipToAdress { get; set; }
        public ShipAddress ShipFromAdress { get; set; }
        public decimal Discount { get; set; }
        public string Notes { get; set; }
        public List<Features.Documents.AddDocument.DocumentDetail> Details { get; set; }
        public decimal? ExchangeRate { get; set; }
    }
}
