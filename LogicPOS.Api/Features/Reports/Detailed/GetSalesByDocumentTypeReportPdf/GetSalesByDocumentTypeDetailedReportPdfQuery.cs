using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.GetSalesByDocumentTypeDetailedReportPdf
{
    public class GetSalesByDocumentTypeDetailedReportPdfQuery : StartAndEndDateReportQuery
    {
        public GetSalesByDocumentTypeDetailedReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {
        }
    }
}
