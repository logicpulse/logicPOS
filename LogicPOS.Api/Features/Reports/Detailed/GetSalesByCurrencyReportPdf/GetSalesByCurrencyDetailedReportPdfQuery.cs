using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.GetSalesByCurrencyDetailedReportPdf
{
    public class GetSalesByCurrencyDetailedReportPdfQuery : StartAndEndDateReportQuery
    {
        public GetSalesByCurrencyDetailedReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {
        }
    }
}
