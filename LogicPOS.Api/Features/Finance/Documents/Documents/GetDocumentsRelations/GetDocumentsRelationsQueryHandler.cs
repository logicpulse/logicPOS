using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Documents.GetDocumentsRelations
{
    public class GetDocumentsRelationsQueryHandler :
        RequestHandler<GetDocumentsRelationsQuery, ErrorOr<IEnumerable<DocumentRelation>>>
    {
        public GetDocumentsRelationsQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<DocumentRelation>>> Handle(GetDocumentsRelationsQuery query,
                                                                            CancellationToken cancellationToken = default)
        {
            var httpQuery = string.Join("&", query.DocumentIds.Select(id => $"documentIds={id}"));
            return await HandleGetListQueryAsync<DocumentRelation>($"documents/relations?{httpQuery}", cancellationToken);
        }
    }
}
