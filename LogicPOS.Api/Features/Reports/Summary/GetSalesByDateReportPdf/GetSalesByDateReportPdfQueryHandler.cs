using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesByDateReportPdf
{
    public class GetSalesByDateReportPdfQueryHandler
        : RequestHandler<GetSalesByDateReportPdfQuery, ErrorOr<string>>
    {
        public GetSalesByDateReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<string>> Handle(GetSalesByDateReportPdfQuery request, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-date/pdf{request.GetUrlQuery()}");
        }
    }
}
