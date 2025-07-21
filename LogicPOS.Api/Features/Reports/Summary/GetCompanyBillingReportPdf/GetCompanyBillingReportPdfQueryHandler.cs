using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetCompanyBillingReportPdf
{
    public class GetCompanyBillingReportPdfQueryHandler :
        RequestHandler<GetCompanyBillingReportPdfQuery, ErrorOr<TempFile>>
    {
        public GetCompanyBillingReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<TempFile>> Handle(GetCompanyBillingReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/company-billing/pdf{query.GetUrlQuery()}");
        }
    }
}
