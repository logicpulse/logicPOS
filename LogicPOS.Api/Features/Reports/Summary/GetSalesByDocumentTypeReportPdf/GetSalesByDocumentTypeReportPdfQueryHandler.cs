using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesByDocumentTypeReportPdf
{
    public class GetSalesByDocumentTypeReportPdfQueryHandler :
        RequestHandler<GetSalesByDocumentTypeReportPdfQuery, ErrorOr<TempFile>>
    {
        public GetSalesByDocumentTypeReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<TempFile>> Handle(GetSalesByDocumentTypeReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-document-type/pdf{query.GetUrlQuery()}");
        }
    }
}
