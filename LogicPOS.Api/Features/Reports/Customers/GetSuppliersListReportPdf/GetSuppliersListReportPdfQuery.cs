using ErrorOr;
using LogicPOS.Api.Features.Common.Responses;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;

namespace LogicPOS.Api.Features.Reports.Customers.GetSuppliersReportPdf
{
    public class GetSuppliersListReportPdfQuery : OptionalDateSpanReportQuery
    {
        public GetSuppliersListReportPdfQuery()
        {
        }
    }
}
