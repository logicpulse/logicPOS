using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Agt.ListOnlineDocuments
{
    public class ListOnlieDocumentsQueryHandler : RequestHandler<ListOnlineDocumentsQuery, ErrorOr<IEnumerable<OnlineDocument>>>
    {
        public ListOnlieDocumentsQueryHandler(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }

        public async override Task<ErrorOr<IEnumerable<OnlineDocument>>> Handle(ListOnlineDocumentsQuery request, CancellationToken cancellationToken = default)
        {
            return await HandleGetListQueryAsync<OnlineDocument>("agt/online/documents/list"+request.GetUrlQuery(), cancellationToken);
        }
    }
}
