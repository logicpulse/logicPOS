using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.POS.SalesByPlace.GetSalesByPlaceDetailedReportPdf
{
    public class GetSalesByPlaceDetailedReportPdfQuery : StartAndEndDateReportQuery
    {
        public GetSalesByPlaceDetailedReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {
        }
    }
}
