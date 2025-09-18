using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.System.Licensing.IsLicensed
{
    public class IsLicensedQueryHandler : RequestHandler<IsLicensedQuery, ErrorOr<IsLicensedResponse>>
    {
        public IsLicensedQueryHandler(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }

        public override async Task<ErrorOr<IsLicensedResponse>> Handle(IsLicensedQuery request, CancellationToken cancellationToken = default)
        {
            return await HandleGetEntityQueryAsync<IsLicensedResponse>("licensing/is-licensed", cancellationToken);
        }
    }
}
