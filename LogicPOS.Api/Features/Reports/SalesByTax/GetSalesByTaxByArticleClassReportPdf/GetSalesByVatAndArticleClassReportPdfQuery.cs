using LogicPOS.Api.Features.Reports.Common;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Reports.GetSalesByVatAndArticleClassReportPdf
{
    public class GetSalesByVatAndArticleClassReportPdfQuery : ReportQuery
    {
        public Guid? TaxId { get; set; }
        public GetSalesByVatAndArticleClassReportPdfQuery(DateTime startDate,
                                            DateTime endDate, Guid? taxId) : base(startDate, endDate, null, null)
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
