using ErrorOr;
using LogicPOS.Api.Extensions;
using LogicPOS.Api.Features.Common.Requests;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.SalesByDate.GetMonthlySalesReportData
{
    public class GetMonthlySalesReportDataQueryHandler : RequestHandler<GetMonthlySalesReportDataQuery, ErrorOr<MonthlySalesReportData>>
    {
        public GetMonthlySalesReportDataQueryHandler(IHttpClientFactory factory, IMemoryCache cache) : base(factory, cache)
        {
        }

        public override async Task<ErrorOr<MonthlySalesReportData>> Handle(GetMonthlySalesReportDataQuery request, CancellationToken ct = default)
        {
            var cacheOptions = GetCacheOptions();
            return await HandleGetEntityQueryAsync<MonthlySalesReportData>($"reports/monthly-sales{request.GetUrlQuery()}", ct, cacheOptions);
        }

        private MemoryCacheEntryOptions GetCacheOptions()
        {
            return new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
        }
    }
}
