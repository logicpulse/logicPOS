using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.GetSalesByFamilyDetailedReportPdf
{
    public class GetSalesByFamilyDetailedReportPdfQuery : StartAndEndDateReportQuery
    {
        public GetSalesByFamilyDetailedReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {
        }
    }
}
