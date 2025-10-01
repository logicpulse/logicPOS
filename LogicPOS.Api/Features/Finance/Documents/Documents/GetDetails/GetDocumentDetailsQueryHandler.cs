using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Documents.Documents.GetDetails
{

    public class GetDocumentDetailsQueryHandler :
       RequestHandler<GetDocumentDetailsQuery, ErrorOr<IEnumerable<DocumentDetail>>>
    {
        public GetDocumentDetailsQueryHandler(IHttpClientFactory factory) : base(factory)
        { }

        public async override Task<ErrorOr<IEnumerable<DocumentDetail>>> Handle(GetDocumentDetailsQuery query, CancellationToken ct = default)
        {
            return await HandleGetEntityQueryAsync<IEnumerable<DocumentDetail>>($"documents/{query.DocumentId}/details", ct);
        }
    }
}
