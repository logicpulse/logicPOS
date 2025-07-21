using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesByFamilyDetailedReportPdf
{
    public class GetSalesByFamilyDetailedReportPdfQueryHandler :
        RequestHandler<GetSalesByFamilyDetailedReportPdfQuery, ErrorOr<TempFile>>
    {
        public GetSalesByFamilyDetailedReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<TempFile>> Handle(GetSalesByFamilyDetailedReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-family/detailed/pdf{query.GetUrlQuery()}");
        }
    }
}
