using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.GetArticleTotalSoldReportPdf
{
    public class GetArticleTotalSoldReportPdfQuery : StartAndEndDateReportQuery
    {
        public GetArticleTotalSoldReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {
        }
    }
}
