using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetCompanyBillingReportPdf
{
    public class GetCompanyBillingReportPdfQueryHandler :
        RequestHandler<GetCompanyBillingReportPdfQuery, ErrorOr<string>>
    {
        public GetCompanyBillingReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<string>> Handle(GetCompanyBillingReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/company-billing/pdf{query.GetUrlQuery()}");
        }
    }
}
