using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesByDateDetailedReportPdf
{
    public class GetSalesByDateDetailedReportPdfQueryHandler :
        RequestHandler<GetSalesByDateDetailedReportPdfQuery, ErrorOr<string>>
    {
        public GetSalesByDateDetailedReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<string>> Handle(GetSalesByDateDetailedReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-document-date/detailed/pdf{query.GetUrlQuery()}");
        }
    }
}
