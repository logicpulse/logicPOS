using LogicPOS.Api.Features.Reports.Common;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Reports.POS.SalesByCommission.GetSalesByCommissionReportPdf
{
    public class GetSalesByCommissionReportPdfQuery : ReportQuery
    {
        public GetSalesByCommissionReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate, null, null)
        {

        }

        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {

        }
    }
}
