using ErrorOr;
using LogicPOS.Api.Features.Common.Responses;
using LogicPOS.Api.Features.Documents.AddDocument;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Documents.Documents.GetDocumentPreviewPdf
{
    public class GetDocumentPreviewPdfQuery : IRequest<ErrorOr<TempFile>>
    {
        public Guid? CurrencyId { get; set; }
        public string Type { get; set; }
        public ShipAddress ShipToAdress { get; set; }
        public ShipAddress ShipFromAdress { get; set; }
        public decimal Discount { get; set; }
        public string Notes { get; set; }
        public List<DocumentDetail> Details { get; set; }
        public decimal? ExchangeRate { get; set; }
    }
}
