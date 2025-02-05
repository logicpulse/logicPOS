using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.GetSalesByDateReportPdf
{
    public class GetSalesByDateReportPdfQuery : StartAndEndDateReportQuery
    {
        public GetSalesByDateReportPdfQuery(DateTime startDate,
                                            DateTime endDate) : base(startDate, endDate)
        {
        }
    }

}
