using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesByVatAndArticleClassReportPdf
{
    public class GetSalesByVatAndArticleClassReportPdfQueryHandler
        : RequestHandler<GetSalesByVatAndArticleClassReportPdfQuery, ErrorOr<string>>
    {
        public GetSalesByVatAndArticleClassReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<string>> Handle(GetSalesByVatAndArticleClassReportPdfQuery request, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-vatrate-and-articleclass/{request.TaxId}/pdf{request.GetUrlQuery()}");
        }
    }
}
