using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.GetSalesByPaymentMethodDetailedReportPdf
{
    public class GetSalesByPaymentMethodDetailedReportPdfQuery : StartAndEndDateReportQuery
    {
        public GetSalesByPaymentMethodDetailedReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {
        }
    }
}
