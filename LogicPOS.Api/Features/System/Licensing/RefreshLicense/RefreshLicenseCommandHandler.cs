using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.System.Licensing.RefreshLicense
{
    public class RefreshLicenseCommandHandler : RequestHandler<RefreshLicenseCommand, ErrorOr<Success>>
    {
        public RefreshLicenseCommandHandler(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }
        public override async Task<ErrorOr<Success>> Handle(RefreshLicenseCommand request, CancellationToken ct = default)
        {
            return await HandleGetQueryAsync<Success>("licensing/refresh", ct);
        }
    }
}
