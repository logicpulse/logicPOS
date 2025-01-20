using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Documents.Documents.GetDocumentPdf
{
    public class GetDocumentPdfQueryHandler :
        RequestHandler<GetDocumentPdfQuery, ErrorOr<string>>
    {
        public GetDocumentPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<string>> Handle(GetDocumentPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"documents/{query.Id}/pdf?copyNumber={query.CopyNumber}");
        }
    }
}
