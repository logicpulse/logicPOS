using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesByDocumentTypeDetailedReportPdf
{
    public class GetSalesByDocumentTypeDetailedReportPdfQueryHandler :
        RequestHandler<GetSalesByDocumentTypeDetailedReportPdfQuery, ErrorOr<string>>
    {
        public GetSalesByDocumentTypeDetailedReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<string>> Handle(GetSalesByDocumentTypeDetailedReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-document-type/detailed/pdf{query.GetUrlQuery()}");
        }
    }
}
