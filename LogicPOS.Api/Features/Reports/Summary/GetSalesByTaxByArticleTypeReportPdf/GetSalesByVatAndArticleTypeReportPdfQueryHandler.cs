using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesByVatAndArticleTypeReportPdf
{
    public class GetSalesByVatAndArticleTypeReportPdfQueryHandler
        : RequestHandler<GetSalesByVatAndArticleTypeReportPdfQuery, ErrorOr<string>>
    {
        public GetSalesByVatAndArticleTypeReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<string>> Handle(GetSalesByVatAndArticleTypeReportPdfQuery request, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-vatrate-and-articletype/pdf/{request.TaxId}{request.GetUrlQuery()}");
        }
    }
}
