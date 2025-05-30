using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesByTaxGroupDetailedReportPdf
{
    public class GetSalesByTaxGroupDetailedReportPdfQueryHandler
        : RequestHandler<GetSalesByTaxGroupDetailedReportPdfQuery, ErrorOr<string>>
    {
        public GetSalesByTaxGroupDetailedReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<string>> Handle(GetSalesByTaxGroupDetailedReportPdfQuery request, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-vatrate-group/detailed/{request.TaxId}/pdf{request.GetUrlQuery()}");
        }
    }
}
