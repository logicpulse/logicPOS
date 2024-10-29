using ErrorOr;
using LogicPOS.Api.Extensions;
using LogicPOS.Api.Features.Common;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Customers.GetCurrentAccountPdf
{
    public class GetCustomerCurrentAccountPdfQueryHandler :
        RequestHandler<GetCustomerCurrentAccountPdfQuery, ErrorOr<string>>
    {
        public GetCustomerCurrentAccountPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<string>> Handle(GetCustomerCurrentAccountPdfQuery query, CancellationToken cancellationToken = default)
        {
            string endpoint = $"customers/{query.CustomerId}/currentaccount/pdf?startDate={query.StartDate.ToISO8601DateOnly()}&endDate={query.EndDate.ToISO8601DateOnly()}";
            return await HandleGetFileQueryAsync(endpoint, cancellationToken);
        }
    }
}
