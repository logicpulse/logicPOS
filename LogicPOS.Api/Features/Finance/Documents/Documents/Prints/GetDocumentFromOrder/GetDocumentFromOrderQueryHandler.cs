using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Documents.Documents.Prints.GetDocumentFromOrder
{
    public class GetDocumentFromOrderQueryHandler :
        RequestHandler<GetDocumentFromOrderQuery, ErrorOr<bool>>
    {
        public GetDocumentFromOrderQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override Task<ErrorOr<bool>> Handle(
            GetDocumentFromOrderQuery query,
            CancellationToken cancellationToken = default)
        {
            return HandleGetQueryAsync<bool>($"documents/{query.DocumentId}/from-order", cancellationToken);
        }
    }
}
