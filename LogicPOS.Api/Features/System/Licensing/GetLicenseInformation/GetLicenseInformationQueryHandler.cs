using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.System.Licensing.GetLicenseInformation
{
    public class GetLicenseInformationQueryHandler : RequestHandler<GetLicenseInformationQuery, ErrorOr<GetLicenseInformationResponse>>
    {
        public GetLicenseInformationQueryHandler(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }
        public override async Task<ErrorOr<GetLicenseInformationResponse>> Handle(GetLicenseInformationQuery request, CancellationToken ct = default)
        {
            return await HandleGetQueryAsync<GetLicenseInformationResponse>("licensing/information", ct);
        }
    }
}
