using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.System.Licensing.GetLicenseData
{
    public class GetLicenseDataQueryHandler : RequestHandler<GetLicenseDataQuery, ErrorOr<GetLicenseDataResponse>>
    {
        public GetLicenseDataQueryHandler(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }
        public override async Task<ErrorOr<GetLicenseDataResponse>> Handle(GetLicenseDataQuery request, CancellationToken ct = default)
        {
            return await HandleGetQueryAsync<GetLicenseDataResponse>("licensing/data", ct);
        }
    }
}
