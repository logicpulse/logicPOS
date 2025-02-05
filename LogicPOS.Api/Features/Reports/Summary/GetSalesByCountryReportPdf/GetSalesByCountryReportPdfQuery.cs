using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.GetSalesByCountryReportPdf
{
    public class GetSalesByCountryReportPdfQuery : StartAndEndDateReportQuery
    {
        public GetSalesByCountryReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {
        }
    }
}
