using LogicPOS.Api.Features.Reports.Common;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Reports.GetSalesByVatAndArticleTypeReportPdf
{
    public class GetSalesByVatAndArticleTypeReportPdfQuery : ReportQuery
    {
        public Guid? TaxId { get; set; }
        public GetSalesByVatAndArticleTypeReportPdfQuery(DateTime startDate,
                                                         DateTime endDate,
                                                         Guid? taxId) : base(startDate, endDate,null,null)
        {
            TaxId = taxId;
        }

        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {
            if (TaxId.HasValue)
            {
                urlQueryBuilder.Append($"&TaxId={TaxId}");
            }
        }
    }

}
