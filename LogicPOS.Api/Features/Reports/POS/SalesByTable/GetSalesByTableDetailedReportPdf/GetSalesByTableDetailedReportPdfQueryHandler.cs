using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Common.Responses;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.POS.SalesByTable.GetSalesByTableDetailedReportPdf
{
    public class GetSalesByTableDetailedReportPdfQueryHandler :
        RequestHandler<GetSalesByTableDetailedReportPdfQuery, ErrorOr<TempFile>>
    {
        public GetSalesByTableDetailedReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<TempFile>> Handle(GetSalesByTableDetailedReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-table/detailed/pdf{query.GetUrlQuery()}");
        }
    }
}