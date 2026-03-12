using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Reports.GetSalesByEmployeeDetailedReportPdf
{
    public class GetSalesByEmployeeDetailedReportPdfQuery : ReportQuery
    {
        public GetSalesByEmployeeDetailedReportPdfQuery(DateTime startDate, 
                                                        DateTime endDate,
                                                        string documentType=null,
                                                        Guid? terminalId = null) : base(startDate, endDate,documentType,terminalId)
        {
        }

        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {

        }
    }
}
