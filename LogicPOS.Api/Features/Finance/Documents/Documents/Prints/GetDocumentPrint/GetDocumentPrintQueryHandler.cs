using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Documents.Documents.GetDocumentPrint
{
    public class GetDocumentPrintQueryHandler : RequestHandler<GetDocumentPrintQuery, ErrorOr<DocumentPrint>>
    {
        public GetDocumentPrintQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<DocumentPrint>> Handle(GetDocumentPrintQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetQueryAsync<DocumentPrint>($"documents/prints/{query.DocumentId}", cancellationToken);
        }
    }
}
