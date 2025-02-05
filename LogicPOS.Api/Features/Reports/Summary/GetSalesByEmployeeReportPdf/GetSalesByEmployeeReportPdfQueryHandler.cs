using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesByEmployeeReportPdf
{
    public class GetSalesByEmployeeReportPdfQueryHandler :
        RequestHandler<GetSalesByEmployeeReportPdfQuery, ErrorOr<string>>
    {
        public GetSalesByEmployeeReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<string>> Handle(GetSalesByEmployeeReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-employee/pdf{query.GetUrlQuery()}", cancellationToken);
        }
    }
}
