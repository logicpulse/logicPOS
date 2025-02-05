using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesByFamilyDetailedReportPdf
{
    public class GetSalesByFamilyDetailedReportPdfQueryHandler :
        RequestHandler<GetSalesByFamilyDetailedReportPdfQuery, ErrorOr<string>>
    {
        public GetSalesByFamilyDetailedReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<string>> Handle(GetSalesByFamilyDetailedReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-family/detailed/pdf{query.GetUrlQuery()}");
        }
    }
}
