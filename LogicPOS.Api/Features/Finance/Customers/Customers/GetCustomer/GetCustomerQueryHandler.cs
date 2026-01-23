using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.Customers.Customers.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Customers.Customers.GetCustomer
{
    public class GetCustomerQueryHandler :
        RequestHandler<GetCustomerQuery, ErrorOr<Customer>>
    {
        public GetCustomerQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Customer>> Handle(GetCustomerQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetQueryAsync<Customer>($"customers/customer{query.GetUrlQuery()}", cancellationToken);
        }


    }
}
