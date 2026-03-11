using LogicPOS.Api.Features.Reports.Common;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Reports.GetStockBySupplierReportPdfReportPdf
{
    public class GetStockBySupplierReportPdfQuery : ReportQuery
    {
        public Guid? SupplierId { get; set; }
        public string DocumentNumber { get; set; }

        public GetStockBySupplierReportPdfQuery(
            DateTime startDate,
            DateTime endDate,
            Guid? supplierId,
            string documentNumber) : base(startDate, endDate, null, null)
        {
            SupplierId = supplierId;
            DocumentNumber = documentNumber;
        }

        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {
            if (SupplierId.HasValue)
            {
                urlQueryBuilder.Append($"&SupplierId={SupplierId}");
            }
            if (!string.IsNullOrWhiteSpace(DocumentNumber))
            {
                urlQueryBuilder.Append($"&DocumentNumber={DocumentNumber}");
            }
        }
    }
}
