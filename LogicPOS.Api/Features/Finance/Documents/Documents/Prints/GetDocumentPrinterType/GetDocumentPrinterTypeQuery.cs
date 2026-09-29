using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Documents.Documents.GetDocumentPrint
{
    public class GetDocumentPrinterTypeQuery : IRequest<ErrorOr<bool>>
    {
        public Guid DocumentId { get; set; }

        public GetDocumentPrinterTypeQuery(Guid id)
        {
            DocumentId = id;
        }
    }
}
