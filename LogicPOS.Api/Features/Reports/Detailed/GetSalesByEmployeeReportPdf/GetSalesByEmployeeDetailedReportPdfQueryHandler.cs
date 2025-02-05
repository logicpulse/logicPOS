using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesByEmployeeDetailedReportPdf
{
    public class GetSalesByEmployeeDetailedReportPdfQueryHandler :
        RequestHandler<GetSalesByEmployeeDetailedReportPdfQuery, ErrorOr<string>>
    {
        public GetSalesByEmployeeDetailedReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<string>> Handle(GetSalesByEmployeeDetailedReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-employee/detailed/pdf{query.GetUrlQuery()}");
        }
    }
}
