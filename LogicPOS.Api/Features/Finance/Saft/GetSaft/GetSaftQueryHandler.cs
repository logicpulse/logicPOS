using ErrorOr;
using LogicPOS.Api.Extensions;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Common.Responses;
using LogicPOS.Api.Features.Finance.Saft.GetSaft;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Saft.GetAngolaSaft
{
    public class GetSaftQueryHandler : RequestHandler<GetSaftQuery, ErrorOr<TempFile>>
    {
        public GetSaftQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<TempFile>> Handle(GetSaftQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"company/saft?startDate={query.StartDate.ToISO8601DateOnly()}&endDate={query.EndDate.ToISO8601DateOnly()}", cancellationToken);
        }
    }
}
