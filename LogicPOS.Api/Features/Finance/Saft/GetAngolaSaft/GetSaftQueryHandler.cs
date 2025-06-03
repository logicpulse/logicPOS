using ErrorOr;
using LogicPOS.Api.Extensions;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Finance.Saft.GetSaft;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Saft.GetAngolaSaft
{
    public class GetSaftQueryHandler : RequestHandler<GetSaftQuery, ErrorOr<byte[]>>
    {
        public GetSaftQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<byte[]>> Handle(GetSaftQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileInBytesQueryAsync($"company/saft?startDate={query.StartDate.ToISO8601DateOnly()}&endDate={query.EndDate.ToISO8601DateOnly()}", cancellationToken);
        }
    }
}
