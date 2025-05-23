using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.System.GetSystemInformations
{
    public class GetSystemInformationsQueryHandler :
        RequestHandler<GetSystemInformationsQuery, ErrorOr<SystemInformations>>
    {
        public GetSystemInformationsQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<SystemInformations>> Handle(GetSystemInformationsQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetEntityQueryAsync<SystemInformations>("system/informations", cancellationToken);
        }
    }
}
