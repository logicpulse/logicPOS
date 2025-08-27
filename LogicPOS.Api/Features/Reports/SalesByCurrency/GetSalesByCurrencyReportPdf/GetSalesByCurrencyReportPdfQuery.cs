using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.GetSalesByCurrencyReportPdf
{
    public class GetSalesByCurrencyReportPdfQuery : StartAndEndDateReportQuery
    {
        public GetSalesByCurrencyReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {
        }
    }
}
