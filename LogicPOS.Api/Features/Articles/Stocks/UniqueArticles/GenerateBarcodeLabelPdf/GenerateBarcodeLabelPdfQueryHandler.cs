using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Common.Responses;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.Stocks.UniqueArticles.GenerateBarcodeLabelPdf
{
    public class GenerateBarcodeLabelPdfQueryHandler :
        RequestHandler<GenerateBarcodeLabelPdfQuery, ErrorOr<TempFile>>
    {
        public GenerateBarcodeLabelPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<TempFile>> Handle(GenerateBarcodeLabelPdfQuery request,
                                                           CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"articles/uniques/labels{request.GetUrlQuery()}");
        }
    }
}
