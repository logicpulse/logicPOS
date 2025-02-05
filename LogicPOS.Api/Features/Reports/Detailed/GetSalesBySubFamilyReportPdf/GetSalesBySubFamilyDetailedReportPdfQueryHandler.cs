using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesBySubFamilyDetailedReportPdf
{
    public class GetSalesBySubFamilyDetailedReportPdfQueryHandler :
        RequestHandler<GetSalesBySubFamilyDetailedReportPdfQuery, ErrorOr<string>>
    {
        public GetSalesBySubFamilyDetailedReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<string>> Handle(GetSalesBySubFamilyDetailedReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-subfamily/detailed/pdf{query.GetUrlQuery()}");
        }
    }
}
