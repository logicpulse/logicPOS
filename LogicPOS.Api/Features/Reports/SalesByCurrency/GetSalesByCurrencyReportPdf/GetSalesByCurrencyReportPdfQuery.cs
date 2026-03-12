using LogicPOS.Api.Features.Reports.Common;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Reports.GetSalesByCurrencyReportPdf
{
    public class GetSalesByCurrencyReportPdfQuery : ReportQuery
    {
        public GetSalesByCurrencyReportPdfQuery(DateTime startDate,
                                                DateTime endDate,
                                                 string documentType,
                                                 Guid? terminalId) : base(startDate, endDate, documentType, terminalId)
        {
        }

        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {

        }
    }
}
