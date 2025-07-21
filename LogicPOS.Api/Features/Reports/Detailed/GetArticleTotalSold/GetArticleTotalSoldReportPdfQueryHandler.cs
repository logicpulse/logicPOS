using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetArticleTotalSoldReportPdf
{
    public class GetArticleTotalSoldReportPdfQueryHandler :
        RequestHandler<GetArticleTotalSoldReportPdfQuery, ErrorOr<TempFile>>
    {
        public GetArticleTotalSoldReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<TempFile>> Handle(GetArticleTotalSoldReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/article-total-sold/pdf{query.GetUrlQuery()}");
        }
    }
}
