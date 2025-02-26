using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Customers.GetAllCustomers
{
    public class GetAllCustomersQueryHandler :
        RequestHandler<GetAllCustomersQuery, ErrorOr<IEnumerable<Customer>>>
    {
        public GetAllCustomersQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<Customer>>> Handle(GetAllCustomersQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetEntitiesQueryAsync<Customer>("customers", cancellationToken);
        }
    }
}
