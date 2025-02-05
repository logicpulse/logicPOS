using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.GetSalesByCountryDetailedReportPdf
{
    public class GetSalesByCountryDetailedReportPdfQuery : StartAndEndDateReportQuery
    {
        public GetSalesByCountryDetailedReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {
        }
    }
}
