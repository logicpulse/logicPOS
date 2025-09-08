using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.SalesByDate.GetSalesTotalForDay
{
    public class GetTotalSalesForDayQueryHandler : RequestHandler<GetSalesTotalForDayQuery, ErrorOr<TotalSalesForDay>>
    {
        public GetTotalSalesForDayQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<TotalSalesForDay>> Handle(GetSalesTotalForDayQuery request, CancellationToken ct = default)
        {
            return await HandleGetEntityQueryAsync<TotalSalesForDay>($"reports/sales-for-day{request.GetUrlQuery()}", ct);
        }
    }
}
