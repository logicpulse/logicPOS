using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Common.Responses;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesBySubFamilyDetailedReportPdf
{
    public class GetSalesBySubFamilyDetailedReportPdfQueryHandler :
        RequestHandler<GetSalesBySubFamilyDetailedReportPdfQuery, ErrorOr<TempFile>>
    {
        public GetSalesBySubFamilyDetailedReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<TempFile>> Handle(GetSalesBySubFamilyDetailedReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-subfamily/detailed/pdf{query.GetUrlQuery()}" +
                                                $"&familycode={query.FamilyCode}&subfamilycode={query.SubfamilyCode}&articlecode={query.ArticleCode}");
        }
    }
}
