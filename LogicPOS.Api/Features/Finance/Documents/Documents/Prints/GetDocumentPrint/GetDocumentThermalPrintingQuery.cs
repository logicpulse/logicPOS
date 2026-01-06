using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Documents.Documents.GetDocumentPrint
{
    public class GetDocumentThermalPrintingQuery : IRequest<ErrorOr<bool>>
    {
        public Guid DocumentId { get; set; }

        public GetDocumentThermalPrintingQuery(Guid id)
        {
            DocumentId = id;
        }
    }
}
