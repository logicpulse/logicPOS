using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.System.Licensing.UpdateCurrentVersion
{
    public class UpdateCurrentVersionCommandHandler : RequestHandler<UpdateCurrentVersionCommand, ErrorOr<UpdateCurrentVersionResponse>>
    {
        public UpdateCurrentVersionCommandHandler(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }

        public override async Task<ErrorOr<UpdateCurrentVersionResponse>> Handle(UpdateCurrentVersionCommand request, CancellationToken ct = default)
        {
            return await HandlePostCommandAsync<UpdateCurrentVersionResponse>("licensing/update-version", request, ct);
        }
    }
}
