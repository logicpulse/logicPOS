using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Common.Responses;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.POS.SalesByTerminal.GetSalesByTerminalDetailedReportPdf
{
    public class GetSalesByTerminalDetailedReportPdfQueryHandler :
        RequestHandler<GetSalesByTerminalDetailedReportPdfQuery, ErrorOr<TempFile>>
    {
        public GetSalesByTerminalDetailedReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<TempFile>> Handle(GetSalesByTerminalDetailedReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-terminal/detailed/pdf{query.GetUrlQuery()}");
        }
    }
}