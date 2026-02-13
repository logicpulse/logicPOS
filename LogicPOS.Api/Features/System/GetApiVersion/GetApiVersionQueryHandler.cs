using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.System.GetApiVersion
{
    public class GetApiVersionQueryHandler : RequestHandler<GetApiVersionQuery, ErrorOr<string>>
    {
        public GetApiVersionQueryHandler(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }

        public override Task<ErrorOr<string>> Handle(GetApiVersionQuery request, CancellationToken cancellationToken = default)
        {
            return HandleGetQueryAsync<string>("system/api-version", cancellationToken);
        }
    }
}
