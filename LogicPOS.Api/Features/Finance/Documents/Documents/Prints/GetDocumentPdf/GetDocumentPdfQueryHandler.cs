using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Common.Responses;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Documents.Documents.Prints.GetDocumentPdf
{
    public class GetDocumentPdfQueryHandler :
        RequestHandler<GetDocumentPdfQuery, ErrorOr<TempFile>>
    {
        public GetDocumentPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<TempFile>> Handle(GetDocumentPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"documents/pdf{query.GetUrlQuery()}");
        }
    }
}
