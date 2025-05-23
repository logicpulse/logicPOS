using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetArticleReportPdf
{
    public class GetArticleReportPdfQueryHandler :
        RequestHandler<GetArticleReportPdfQuery, ErrorOr<string>>
    {
        public GetArticleReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<string>> Handle(GetArticleReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/article/pdf");
        }
    }
}
