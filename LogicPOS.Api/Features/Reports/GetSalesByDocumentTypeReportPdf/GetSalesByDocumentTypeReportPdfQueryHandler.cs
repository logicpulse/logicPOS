using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesByDocumentTypeReportPdf
{
    public class GetSalesByDocumentTypeReportPdfQueryHandler :
        RequestHandler<GetSalesByDocumentTypeReportPdfQuery, ErrorOr<string>>
    {
        public GetSalesByDocumentTypeReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<string>> Handle(GetSalesByDocumentTypeReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-document-type/pdf{query.GetUrlQuery()}");
        }
    }
}
