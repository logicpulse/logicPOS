using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Documents.GetDocumentById
{
    public class GetDocumentByIdQueryHandler :
        RequestHandler<GetDocumentByIdQuery, ErrorOr<Document>>
    {
        public GetDocumentByIdQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Document>> Handle(GetDocumentByIdQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetEntityQueryAsync<Document>($"documents/{query.Id}", cancellationToken);
        }
    }
}
