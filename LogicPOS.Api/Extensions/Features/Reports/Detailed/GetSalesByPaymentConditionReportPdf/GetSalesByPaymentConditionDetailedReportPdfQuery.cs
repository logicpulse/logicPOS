using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.GetSalesByPaymentConditionDetailedReportPdf
{
    public class GetSalesByPaymentConditionDetailedReportPdfQuery : StartAndEndDateReportQuery
    {
        public GetSalesByPaymentConditionDetailedReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {
        }
    }
}
