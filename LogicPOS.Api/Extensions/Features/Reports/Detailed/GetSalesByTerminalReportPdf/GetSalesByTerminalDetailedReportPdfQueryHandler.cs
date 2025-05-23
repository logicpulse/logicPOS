using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesByTerminalDetailedReportPdf
{
    public class GetSalesByTerminalDetailedReportPdfQueryHandler :
        RequestHandler<GetSalesByTerminalDetailedReportPdfQuery, ErrorOr<string>>
    {
        public GetSalesByTerminalDetailedReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<string>> Handle(GetSalesByTerminalDetailedReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-terminal/detailed/pdf{query.GetUrlQuery()}");
        }
    }
}