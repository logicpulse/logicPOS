using LogicPOS.Api.Features.Reports.Common;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Reports.POS.SalesByCommission.GetSalesByCommissionReportPdf
{
    public class GetSalesByCommissionReportPdfQuery : ReportFileQuery
    {
        public GetSalesByCommissionReportPdfQuery(DateTime startDate,
                                                  DateTime endDate,
                                                  string documentType = null,
                                                  Guid? terminalId = null) : base(startDate, endDate, documentType, terminalId)
        {

        }

        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {

        }
    }
}
