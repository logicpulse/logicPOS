using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Common.Responses;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Customers.GetCurrentAccountPdf
{
    public class GetCustomerCurrentAccountPdfQueryHandler :
        RequestHandler<GetCustomerCurrentAccountPdfQuery, ErrorOr<TempFile>>
    {
        public GetCustomerCurrentAccountPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<TempFile>> Handle(GetCustomerCurrentAccountPdfQuery query, CancellationToken cancellationToken = default)
        {
            string endpoint = $"reports/customers/{query.CustomerId}/current-account/pdf{query.GetUrlQuery()}";
            return await HandleGetFileQueryAsync(endpoint, cancellationToken);
        }
    }
}
