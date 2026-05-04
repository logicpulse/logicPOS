using ErrorOr;
using LogicPOS.Api.Features.Common.Responses;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.GetCustomerReportPdf
{
    public class GetCustomersListReportPdfQuery : IRequest<ErrorOr<TempFile>>
    {
        public GetCustomersListReportPdfQuery()
        {
        }
    }
}
