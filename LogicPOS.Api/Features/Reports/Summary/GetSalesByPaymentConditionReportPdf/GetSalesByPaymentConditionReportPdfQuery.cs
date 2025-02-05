using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.GetSalesByPaymentConditionReportPdf
{
    public class GetSalesByPaymentConditionReportPdfQuery : StartAndEndDateReportQuery
    {
        public GetSalesByPaymentConditionReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {
        }
    }
}
