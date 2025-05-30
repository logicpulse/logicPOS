using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.GetSalesByVatAndArticleClassReportPdf
{
    public class GetSalesByVatAndArticleClassReportPdfQuery : StartAndEndDateReportQuery
    {
        public Guid? TaxId {  get; set; }
        public GetSalesByVatAndArticleClassReportPdfQuery(DateTime startDate,
                                            DateTime endDate, Guid taxId) : base(startDate, endDate)
        {
            TaxId = taxId;
        }
    }

}
