using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.System.Licensing.GetVersion
{
    public class GetVersionQueryHandler : RequestHandler<GetVersionQuery, ErrorOr<GetVersionResponse>>
    {
        public GetVersionQueryHandler(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }

        public override async Task<ErrorOr<GetVersionResponse>> Handle(GetVersionQuery request, CancellationToken ct = default)
        {
            return await HandleGetEntityQueryAsync<GetVersionResponse>("licensing/version", ct);
        }
    }
}
