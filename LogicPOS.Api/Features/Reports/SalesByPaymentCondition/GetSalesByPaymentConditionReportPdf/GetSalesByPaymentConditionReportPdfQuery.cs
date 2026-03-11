using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Reports.GetSalesByPaymentConditionReportPdf
{
    public class GetSalesByPaymentConditionReportPdfQuery : ReportQuery
    {
        public GetSalesByPaymentConditionReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate,null,null)
        {
        }

        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {

        }
    }
}
