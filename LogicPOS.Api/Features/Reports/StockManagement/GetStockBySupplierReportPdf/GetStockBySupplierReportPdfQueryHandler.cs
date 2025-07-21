using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetStockBySupplierReportPdfReportPdf
{
    public class GetStockBySupplierReportPdfQueryHandler :
        RequestHandler<GetStockBySupplierReportPdfQuery, ErrorOr<TempFile>>
    {
        public GetStockBySupplierReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<TempFile>> Handle(GetStockBySupplierReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/stock-by-supplier/pdf{query.GetUrlQuery()}" +
                                                                    $"&supplierId={query.SupplierId}"+
                                                                    $"&documentNumber={query.DocumentNumber}");
        }
    }
}
