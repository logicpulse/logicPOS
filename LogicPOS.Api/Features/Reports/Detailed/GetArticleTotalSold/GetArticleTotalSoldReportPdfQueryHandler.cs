using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetArticleTotalSoldReportPdf
{
    public class GetArticleTotalSoldReportPdfQueryHandler :
        RequestHandler<GetArticleTotalSoldReportPdfQuery, ErrorOr<string>>
    {
        public GetArticleTotalSoldReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<string>> Handle(GetArticleTotalSoldReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/article-total-sold/pdf{query.GetUrlQuery()}");
        }
    }
}
