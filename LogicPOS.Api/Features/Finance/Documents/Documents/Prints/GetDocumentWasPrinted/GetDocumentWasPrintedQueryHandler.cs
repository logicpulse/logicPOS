using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Documents.Documents.Prints.GetDocumentWasPrinted
{
    public class GetDocumentWasPrintedQueryHandler :
        RequestHandler<GetDocumentWasPrintedQuery, ErrorOr<bool>>
    {
        public GetDocumentWasPrintedQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override Task<ErrorOr<bool>> Handle(
            GetDocumentWasPrintedQuery query,
            CancellationToken cancellationToken = default)
        {
            return HandleGetQueryAsync<bool>($"documents/{query.DocumentId}/was-printed", cancellationToken);
        }
    }
}
