using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Reports.GetSalesByPaymentConditionDetailedReportPdf
{
    public class GetSalesByPaymentConditionDetailedReportPdfQuery : ReportQuery
    {
        public GetSalesByPaymentConditionDetailedReportPdfQuery(DateTime startDate, 
                                                                DateTime endDate,
                                                                string documentType=null,
                                                                Guid? terminalId=null) : base(startDate, endDate,null,null)
        {
        }

        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {

        }
    }
}
