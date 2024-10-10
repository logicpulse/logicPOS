using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Receipts.GetAllReceipts
{
    public class GetAllReceiptsQueryHandler :
        RequestHandler<GetAllReceiptsQuery, ErrorOr<IEnumerable<Receipt>>>
    {
        public GetAllReceiptsQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<IEnumerable<Receipt>>> Handle(GetAllReceiptsQuery request, CancellationToken cancellationToken = default)
        {
            return await HandleGetAllQueryAsync<Receipt>("receipts", cancellationToken);
        }
    }
}
