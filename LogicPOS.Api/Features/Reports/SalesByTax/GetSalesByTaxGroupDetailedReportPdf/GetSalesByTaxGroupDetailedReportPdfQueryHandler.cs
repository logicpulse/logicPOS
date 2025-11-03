using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Common.Responses;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesByTaxGroupDetailedReportPdf
{
    public class GetSalesByTaxGroupDetailedReportPdfQueryHandler
        : RequestHandler<GetSalesByTaxGroupDetailedReportPdfQuery, ErrorOr<TempFile>>
    {
        public GetSalesByTaxGroupDetailedReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<TempFile>> Handle(GetSalesByTaxGroupDetailedReportPdfQuery request, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-vatrate-group/detailed/{request.TaxId}/pdf{request.GetUrlQuery()}");
        }
    }
}
