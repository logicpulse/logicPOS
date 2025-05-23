using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.GetSalesByCustomerReportPdf
{
    public class GetSalesByCustomerReportPdfQuery : StartAndEndDateReportQuery
    {
        public GetSalesByCustomerReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {
        }
    }
}
