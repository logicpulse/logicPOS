using ErrorOr;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesByDateReportPdf
{
    public class GetSalesByDateReportPdfQueryHandler
        : RequestHandler<GetSalesByDateReportPdfQuery, ErrorOr<TempFile>>
    {
        public GetSalesByDateReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<TempFile>> Handle(GetSalesByDateReportPdfQuery request, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-date/pdf{request.GetUrlQuery()}");
        }
    }
}
