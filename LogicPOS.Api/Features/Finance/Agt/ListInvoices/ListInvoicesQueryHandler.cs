using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Agt.ListInvoices
{
    public class ListInvoicesQueryHandler : RequestHandler<ListInvoicesQuery, ErrorOr<IEnumerable<AgtInvoice>>>
    {
        public ListInvoicesQueryHandler(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }

        public async override Task<ErrorOr<IEnumerable<AgtInvoice>>> Handle(ListInvoicesQuery request, CancellationToken cancellationToken = default)
        {
            return await HandleGetListQueryAsync<AgtInvoice>("agt/online/documents"+request.GetUrlQuery(), cancellationToken);
        }
    }
}
