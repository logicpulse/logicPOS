using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Reports.GetSalesByCommissionReportPdf
{
    public class GetSalesByCommissionReportPdfQuery : StartAndEndDateReportQuery
    {
        public GetSalesByCommissionReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {
            
        }
    }
}
