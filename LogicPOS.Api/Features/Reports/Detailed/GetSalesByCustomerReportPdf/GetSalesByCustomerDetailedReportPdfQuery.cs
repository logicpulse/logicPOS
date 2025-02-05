using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.GetSalesByCustomerDetailedReportPdf
{
    public class GetSalesByCustomerDetailedReportPdfQuery : StartAndEndDateReportQuery
    {
        public GetSalesByCustomerDetailedReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {
        }
    }
}
