using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.GetSalesByDateDetailedReportPdf
{
    public class GetSalesByDateDetailedReportPdfQuery : StartAndEndDateReportQuery
    {
        public GetSalesByDateDetailedReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {
        }
    }
}
