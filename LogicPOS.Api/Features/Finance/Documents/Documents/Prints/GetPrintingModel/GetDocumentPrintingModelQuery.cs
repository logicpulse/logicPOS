using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Finance.Documents.Documents.Prints.GetPrintingModel
{
    public class GetDocumentPrintingModelQuery : IRequest<ErrorOr<DocumentPrintingModel>>
    {
        public Guid Id { get; set; }

        public GetDocumentPrintingModelQuery(Guid id) => Id = id;
    }
}
