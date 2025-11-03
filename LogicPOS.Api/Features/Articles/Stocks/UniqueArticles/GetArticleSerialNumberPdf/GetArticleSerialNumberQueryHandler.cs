using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Common.Responses;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.StockManagement.GetArticleSerialNumberPdf
{
    public class GetArticleSerialNumberQueryHandler :
        RequestHandler<GetArticleSerialNumberPdfQuery, ErrorOr<TempFile>>
    {
        public GetArticleSerialNumberQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<TempFile>> Handle(GetArticleSerialNumberPdfQuery request,
                                                           CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"articles/stocks/{request.Id}/pdf");
        }
    }
}
