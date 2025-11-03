using ErrorOr;
using LogicPOS.Api.Features.Common.Responses;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.GetCustomerReportPdf
{
    public class GetCustomerReportPdfQuery : IRequest<ErrorOr<TempFile>>
    {
        public GetCustomerReportPdfQuery()
        {
        }
    }
}
