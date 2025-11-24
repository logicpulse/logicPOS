using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.System.Licensing.NeedToRegister
{
    public class NeedToRegisterQueryHandler :RequestHandler<NeedToRegisterQuery, ErrorOr<NeedToRegisterResponse>>
    {
        public NeedToRegisterQueryHandler(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }
        public override async Task<ErrorOr<NeedToRegisterResponse>> Handle(NeedToRegisterQuery request, CancellationToken ct = default)
        {
            return await HandleGetQueryAsync<NeedToRegisterResponse>("licensing/need-to-register", ct);
        }
    }

}
