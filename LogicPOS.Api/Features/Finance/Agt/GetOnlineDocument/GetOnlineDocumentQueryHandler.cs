using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Agt.GetOnlineDocument
{
    public class GetOnlineDocumentQueryHandler : RequestHandler<GetOnlineDocumentQuery, ErrorOr<OnlineDocument>>
    {
        public GetOnlineDocumentQueryHandler(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }

        public override async Task<ErrorOr<OnlineDocument>> Handle(GetOnlineDocumentQuery request, CancellationToken ct = default)
        {
            return await HandleGetQueryAsync<OnlineDocument>($"agt/online/documents?DocumentNumber={request.DocumentNumber}", ct);
        }
    }
}
