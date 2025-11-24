using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.System.Licensing.GetLicenseFilename
{
    public class GetLicenseFilenameQueryHandler : RequestHandler<GetLicenseFilenameQuery, ErrorOr<GetLicenseFilenameResponse>>
    {
        public GetLicenseFilenameQueryHandler(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }

        public override async Task<ErrorOr<GetLicenseFilenameResponse>> Handle(GetLicenseFilenameQuery request, CancellationToken ct = default)
        {
            return await HandleGetQueryAsync<GetLicenseFilenameResponse>("licensing/filename", ct);
        }
    }
}
