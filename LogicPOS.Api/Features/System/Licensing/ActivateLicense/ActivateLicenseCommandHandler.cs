using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.System.Licensing.ActivateLicense
{
    public class ActivateLicenseCommandHandler : RequestHandler<ActivateLicenseCommand, ErrorOr<ActivateLicenseResponse>>
    {
        public ActivateLicenseCommandHandler(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }

        public override async Task<ErrorOr<ActivateLicenseResponse>> Handle(ActivateLicenseCommand command, CancellationToken ct = default)
        {
            return await HandlePostCommandAsync<ActivateLicenseResponse>("licensing/activate", command, ct);
        }
    }
}
