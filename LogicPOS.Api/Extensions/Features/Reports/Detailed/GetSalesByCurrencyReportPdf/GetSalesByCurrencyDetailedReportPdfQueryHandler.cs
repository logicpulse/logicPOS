using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesByCurrencyDetailedReportPdf
{
    public class GetSalesByCurrencyDetailedReportPdfQueryHandler :
        RequestHandler<GetSalesByCurrencyDetailedReportPdfQuery, ErrorOr<string>>
    {
        public GetSalesByCurrencyDetailedReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<string>> Handle(GetSalesByCurrencyDetailedReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-currency/detailed/pdf{query.GetUrlQuery()}");
        }
    }
}
