using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Documents.GetAllDocuments
{
    public class GetAllDocumentsQueryHandler :
        RequestHandler<GetAllDocumentsQuery, ErrorOr<IEnumerable<Document>>>
    {
        public GetAllDocumentsQueryHandler(IHttpClientFactory factory) : base(factory)
        { }

        public async override Task<ErrorOr<IEnumerable<Document>>> Handle(GetAllDocumentsQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetAllQueryAsync<Document>("documents", cancellationToken);
        }
    }
}
