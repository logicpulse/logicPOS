using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.System.Licensing.GetCurrentVersion
{
    public class GetCurrentVersionQueryHandler: RequestHandler<GetCurrentVersionQuery, ErrorOr<GetCurrentVersionResponse>>
    {
        public GetCurrentVersionQueryHandler(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }
        public override async Task<ErrorOr<GetCurrentVersionResponse>> Handle(GetCurrentVersionQuery request, CancellationToken ct = default)
        {
            return await HandleGetEntityQueryAsync<GetCurrentVersionResponse>("licensing/current-version", ct);
        }
    }
}
