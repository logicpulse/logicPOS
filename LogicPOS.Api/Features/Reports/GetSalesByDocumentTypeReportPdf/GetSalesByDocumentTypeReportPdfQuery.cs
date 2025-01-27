using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogicPOS.Api.Features.Reports.GetSalesByDocumentTypeReportPdf
{
    public class GetSalesByDocumentTypeReportPdfQuery : StartAndEndDateReportQuery
    {
        public GetSalesByDocumentTypeReportPdfQuery(DateTime startDate,
                                                    DateTime endDate) : base(startDate, endDate)
        {
        }
    }
}
