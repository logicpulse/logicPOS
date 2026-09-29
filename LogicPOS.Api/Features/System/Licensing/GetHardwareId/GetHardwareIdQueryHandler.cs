using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.System.Licensing.GetHardwareId
{
    public class GetHardwareIdQueryHandler : RequestHandler<GetHardwareIdQuery, ErrorOr<GetHardwareIdResponse>>
    {
        public GetHardwareIdQueryHandler(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }
        public override async Task<ErrorOr<GetHardwareIdResponse>> Handle(GetHardwareIdQuery request, CancellationToken ct = default)
        {
            return await HandleGetQueryAsync<GetHardwareIdResponse>("licensing/hardware-id", ct);
        }
    }
   
}
