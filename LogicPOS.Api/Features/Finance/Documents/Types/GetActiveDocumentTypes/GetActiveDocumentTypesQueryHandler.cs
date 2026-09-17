using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.Documents.Types.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Documents.Types.GetActiveDocumentTypes
{
    public class GetActiveDocumentTypesQueryHandler :
        RequestHandler<GetActiveDocumentTypesQuery, ErrorOr<IEnumerable<DocumentType>>>
    {
        public GetActiveDocumentTypesQueryHandler(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {

        }

        public override async Task<ErrorOr<IEnumerable<DocumentType>>> Handle(GetActiveDocumentTypesQuery query,
                                                                        CancellationToken cancellationToken = default)
        {
            var result = await HandleGetListQueryAsync<DocumentType>("documents/types/active", cancellationToken);

            return result;
        }
    }
}
