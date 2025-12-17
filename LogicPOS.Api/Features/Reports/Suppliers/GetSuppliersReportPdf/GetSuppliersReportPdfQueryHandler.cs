using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Common.Responses;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSuppliersReportPdf
{
    public class GetSuppliersReportPdfQueryHandler :
        RequestHandler<GetSuppliersReportPdfQuery, ErrorOr<TempFile>>
    {
        public GetSuppliersReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<TempFile>> Handle(GetSuppliersReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/ofsupplier/pdf");
        }
    }
}
