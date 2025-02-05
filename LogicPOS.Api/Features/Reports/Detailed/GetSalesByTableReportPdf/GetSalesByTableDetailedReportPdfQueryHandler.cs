using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesByTableDetailedReportPdf
{
    public class GetSalesByTableDetailedReportPdfQueryHandler :
        RequestHandler<GetSalesByTableDetailedReportPdfQuery, ErrorOr<string>>
    {
        public GetSalesByTableDetailedReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<string>> Handle(GetSalesByTableDetailedReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-table/detailed/pdf{query.GetUrlQuery()}");
        }
    }
}