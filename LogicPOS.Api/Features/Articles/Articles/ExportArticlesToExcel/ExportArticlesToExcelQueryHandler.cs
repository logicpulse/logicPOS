using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Common.Responses;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.Articles.ExportArticlesToExcel
{
    public class ExportArticlesToExcelQueryHandler :
        RequestHandler<ExportArticlesToExcelQuery, ErrorOr<TempFile>>
    {
        public ExportArticlesToExcelQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<TempFile>> Handle(ExportArticlesToExcelQuery query,
                                                           CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"articles/export-to-excel");
        }
    }
}
