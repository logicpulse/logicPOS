using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.System.Licensing.GetLicense
{
    public class GetLicenseQueryHandler : RequestHandler<GetLicenseQuery, ErrorOr<GetLicenseResponse>>
    {
        public GetLicenseQueryHandler(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }
        public override async Task<ErrorOr<GetLicenseResponse>> Handle(GetLicenseQuery request, CancellationToken ct = default)
        {
            return await HandlePostCommandAsync<GetLicenseResponse>("licensing/get-license",request, ct);
        }
    }
}
