using ErrorOr;
using LogicPOS.Api.Features.Common.Responses;
using MediatR;

namespace LogicPOS.Api.Features.Reports.Customers.GetSuppliersReportPdf
{
    public class GetSuppliersListReportPdfQuery : IRequest<ErrorOr<TempFile>>
    {
        public GetSuppliersListReportPdfQuery()
        {
        }
    }
}
