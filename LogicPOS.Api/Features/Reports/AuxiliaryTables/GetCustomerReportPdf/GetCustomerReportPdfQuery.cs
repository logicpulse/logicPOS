using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.GetCustomerReportPdf
{
    public class GetCustomerReportPdfQuery : IRequest<ErrorOr<string>>
    {
        public GetCustomerReportPdfQuery()
        {
        }
    }
}
