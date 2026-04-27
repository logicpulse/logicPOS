using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Common.Responses;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.Customers.GetCurrentAccountSummaryPdf
{
    public class GetCustomersCurrentAccountSummaryReportPdfQueryHandler :
        RequestHandler<GetCustomersCurrentAccountSummaryReportPdfQuery, ErrorOr<TempFile>>
    {
        public GetCustomersCurrentAccountSummaryReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<TempFile>> Handle(GetCustomersCurrentAccountSummaryReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            string endpoint = $"reports/customers/current-account/summary/pdf{query.GetUrlQuery()}";
            return await HandleGetFileQueryAsync(endpoint, cancellationToken);
        }
    }
}
