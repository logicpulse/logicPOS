using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.System.GetSystemInformations
{
    public class GetSystemInformationsQueryHandler :
        RequestHandler<GetSystemInformationsQuery, ErrorOr<SystemInformation>>
    {
        public GetSystemInformationsQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<SystemInformation>> Handle(GetSystemInformationsQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetQueryAsync<SystemInformation>("system/information", cancellationToken);
        }
    }
}
