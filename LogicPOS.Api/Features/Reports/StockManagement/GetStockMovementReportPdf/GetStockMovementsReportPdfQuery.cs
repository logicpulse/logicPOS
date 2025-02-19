using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.GetStockMovementReportPdf
{
    public class GetStockMovementsReportPdfQuery : StartAndEndDateReportQuery
    {
        public GetStockMovementsReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {
        }
    }
}
