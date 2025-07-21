using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Pagination;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Documents.GetDocuments
{
    public class GetDocumentsQueryHandler :
        RequestHandler<GetDocumentsQuery, ErrorOr<PaginatedResult<Document>>>
    {
        public GetDocumentsQueryHandler(IHttpClientFactory factory) : base(factory)
        { }

        public async override Task<ErrorOr<PaginatedResult<Document>>> Handle(GetDocumentsQuery query, CancellationToken cancellationToken = default)
        {
            var urlQuery = query.GetUrlQuery();
            return await HandleGetEntityQueryAsync<PaginatedResult<Document>>($"documents/{urlQuery}", cancellationToken);
        }
    }
}
