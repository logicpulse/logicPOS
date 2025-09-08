using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.POS.DeletedOrders.GetDeletedOrdersReportPdf
{
    public class GetDeletedOrdersReportPdfQueryHandler :
        RequestHandler<GetDeletedOrdersReportPdfQuery, ErrorOr<TempFile>>
    {
        public GetDeletedOrdersReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<TempFile>> Handle(GetDeletedOrdersReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/deleted-orders/pdf{query.GetUrlQuery()}");
        }
    }
}
