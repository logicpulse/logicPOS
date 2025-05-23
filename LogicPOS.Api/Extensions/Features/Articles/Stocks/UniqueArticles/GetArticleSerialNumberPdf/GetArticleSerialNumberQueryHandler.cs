using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.StockManagement.GetArticleSerialNumberPdf
{
    public class GetArticleSerialNumberQueryHandler :
        RequestHandler<GetArticleSerialNumberPdfQuery, ErrorOr<string>>
    {
        public GetArticleSerialNumberQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<string>> Handle(GetArticleSerialNumberPdfQuery request,
                                                           CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"articles/stocks/{request.Id}/pdf");
        }
    }
}
