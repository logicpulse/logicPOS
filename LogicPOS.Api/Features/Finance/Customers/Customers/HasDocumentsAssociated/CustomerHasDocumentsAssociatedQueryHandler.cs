using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Customers.HasDocumentsAssociated
{
    public class CustomerHasDocumentsAssociatedQueryHandler :
        RequestHandler<CustomerHasDocumentsAssociatedQuery, ErrorOr<bool>>
    {
        public CustomerHasDocumentsAssociatedQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<bool>> Handle(CustomerHasDocumentsAssociatedQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetEntityQueryAsync<bool>($"customers/{query.Id}/has-documents-associated", cancellationToken);
        }
    }
}
