using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.GetCompanyBillingReportPdf
{
    public class GetCompanyBillingReportPdfQuery : StartAndEndDateReportQuery
    {
        public GetCompanyBillingReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {
        }
    }
}
