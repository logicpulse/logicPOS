using ErrorOr;
using LogicPOS.Api.Extensions;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Company.GetAngolaSaft
{
    public class GetAngolaSaftQueryHandler : RequestHandler<GetAngolaSaftQuery, ErrorOr<byte[]>>
    {
        public GetAngolaSaftQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<byte[]>> Handle(GetAngolaSaftQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileInBytesQueryAsync($"company/saft/ao?startDate={query.StartDate.ToISO8601DateOnly()}&endDate={query.EndDate.ToISO8601DateOnly()}", cancellationToken);
        }
    }
}
