using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Documents.Documents.Prints.AddDocumentPrint
{
    public class AddDocumentPrintCommandHandler :
        RequestHandler<AddDocumentPrintCommand, ErrorOr<Guid>>
    {
        public AddDocumentPrintCommandHandler(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }

        public override Task<ErrorOr<Guid>> Handle(AddDocumentPrintCommand request, CancellationToken ct = default)
        {
            return HandleAddCommandAsync("documents/prints", request, ct);
        }
    }
}
