using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Reports.POS.SalesByTable.GetSalesByTableDetailedReportPdf
{
    public class GetSalesByTableDetailedReportPdfQuery : ReportQuery
    {
        public GetSalesByTableDetailedReportPdfQuery(DateTime startDate, 
                                                     DateTime endDate,
                                                     string documentType=null,
                                                     Guid? terminalId=null) : base(startDate, endDate,documentType, terminalId)
        {
        }

        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {

        }
    }
}
