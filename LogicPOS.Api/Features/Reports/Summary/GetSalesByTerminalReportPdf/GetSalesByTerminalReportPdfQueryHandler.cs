using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesByTerminalReportPdf
{
    public class GetSalesByTerminalReportPdfQueryHandler :
        RequestHandler<GetSalesByTerminalReportPdfQuery, ErrorOr<TempFile>>
    {
        public GetSalesByTerminalReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<TempFile>> Handle(GetSalesByTerminalReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-terminal/pdf{query.GetUrlQuery()}");
        }
    }
}
