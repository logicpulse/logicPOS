using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.System.Licensing.ConnectToWs
{
    public class ConnectToWsQueryHandler : RequestHandler<ConnectToWsQuery, ErrorOr<ConnectToWsResponse>>
    {
        public ConnectToWsQueryHandler(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }
        public override async Task<ErrorOr<ConnectToWsResponse>> Handle(ConnectToWsQuery request, CancellationToken ct = default)
        {
            return await HandleGetQueryAsync<ConnectToWsResponse>("licensing/connect", ct);
        }
    }
}
