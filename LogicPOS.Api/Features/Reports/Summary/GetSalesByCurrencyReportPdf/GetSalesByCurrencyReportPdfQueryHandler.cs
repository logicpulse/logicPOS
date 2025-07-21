using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesByCurrencyReportPdf
{
    public class GetSalesByCurrencyReportPdfQueryHandler :
        RequestHandler<GetSalesByCurrencyReportPdfQuery, ErrorOr<TempFile>>
    {
        public GetSalesByCurrencyReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<TempFile>> Handle(GetSalesByCurrencyReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-currency/pdf{query.GetUrlQuery()}");
        }
    }
}
