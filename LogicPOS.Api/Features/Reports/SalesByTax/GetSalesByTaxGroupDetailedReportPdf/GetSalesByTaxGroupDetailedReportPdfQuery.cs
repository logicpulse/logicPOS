using LogicPOS.Api.Features.Reports.Common;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Reports.GetSalesByTaxGroupDetailedReportPdf
{
    public class GetSalesByTaxGroupDetailedReportPdfQuery : ReportQuery
    {
        public Guid? TaxId { get; set; }
        public GetSalesByTaxGroupDetailedReportPdfQuery(DateTime startDate, DateTime endDate, Guid? taxId) : base(startDate, endDate, null, null)
        {
            TaxId = taxId;
        }

        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {
            if (TaxId != null)
            {
                urlQueryBuilder.Append($"&TaxId={TaxId}");
            }
        }
    }

}
