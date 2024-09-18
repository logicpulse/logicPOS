using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.DocumentTypes.GetAllDocumentTypes
{
    public class GetAllDocumentTypesQueryHandler :
        RequestHandler<GetAllDocumentTypesQuery, ErrorOr<IEnumerable<DocumentType>>>
    {
        public GetAllDocumentTypesQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<DocumentType>>> Handle(GetAllDocumentTypesQuery query,
                                                                        CancellationToken cancellationToken = default)
        {
            return await HandleGetAllQueryAsync<DocumentType>("documents/types", cancellationToken);
        }
    }
}
