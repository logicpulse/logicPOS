using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.GetSalesByTaxGroupDetailedReportPdf
{
    public class GetSalesByTaxGroupDetailedReportPdfQuery : StartAndEndDateReportQuery
    {
        public Guid? TaxId {  get; set; }
        public GetSalesByTaxGroupDetailedReportPdfQuery(DateTime startDate,
                                            DateTime endDate, Guid taxId) : base(startDate, endDate)
        {
            TaxId = taxId;
        }
    }

}
