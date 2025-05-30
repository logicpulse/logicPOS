using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.GetSalesByVatAndArticleTypeReportPdf
{
    public class GetSalesByVatAndArticleTypeReportPdfQuery : StartAndEndDateReportQuery
    {
        public Guid? TaxId {  get; set; }
        public GetSalesByVatAndArticleTypeReportPdfQuery(DateTime startDate,
                                            DateTime endDate, Guid taxId= new Guid()) : base(startDate, endDate)
        {
            TaxId = taxId;
        }
    }

}
