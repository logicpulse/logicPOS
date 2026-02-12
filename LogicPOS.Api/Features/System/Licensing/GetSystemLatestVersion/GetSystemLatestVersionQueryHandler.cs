using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.System.Licensing.GetSystemLatestVersion
{
    public class GetSystemLatestVersionQueryHandler : RequestHandler<GetSystemLatestVersionQuery, ErrorOr<GetSystemLatestVersionResponse>>
    {
        public GetSystemLatestVersionQueryHandler(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }

        public override async Task<ErrorOr<GetSystemLatestVersionResponse>> Handle(GetSystemLatestVersionQuery request, CancellationToken ct = default)
        {
            return await HandleGetQueryAsync<GetSystemLatestVersionResponse>("licensing/system/lastest-version", ct);
        }
    }
}
