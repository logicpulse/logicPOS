using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.GetSalesByPaymentMethodReportPdf
{
    public class GetSalesByPaymentMethodReportPdfQuery : StartAndEndDateReportQuery
    {
        public GetSalesByPaymentMethodReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {
        }
    }
}
