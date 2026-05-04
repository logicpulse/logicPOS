using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Common.Responses;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.Customers.GetSuppliersReportPdf
{
    public class GetSuppliersListReportPdfQueryHandler :
        RequestHandler<GetSuppliersListReportPdfQuery, ErrorOr<TempFile>>
    {
        public GetSuppliersListReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<TempFile>> Handle(GetSuppliersListReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/customers/suppliers/pdf");
        }
    }
}
