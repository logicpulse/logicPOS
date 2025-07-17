using ErrorOr;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesByCurrencyDetailedReportPdf
{
    public class GetSalesByCurrencyDetailedReportPdfQueryHandler :
        RequestHandler<GetSalesByCurrencyDetailedReportPdfQuery, ErrorOr<TempFile>>
    {
        public GetSalesByCurrencyDetailedReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<TempFile>> Handle(GetSalesByCurrencyDetailedReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-currency/detailed/pdf{query.GetUrlQuery()}");
        }
    }
}
