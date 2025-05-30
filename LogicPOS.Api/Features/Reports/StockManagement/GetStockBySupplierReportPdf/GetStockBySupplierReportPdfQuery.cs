using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.GetStockBySupplierReportPdfReportPdf
{
    public class GetStockBySupplierReportPdfQuery : StartAndEndDateReportQuery
    {
        public Guid SupplierId;
        public string DocumentNumber;

        public GetStockBySupplierReportPdfQuery(DateTime startDate, DateTime endDate, Guid supplierId= new Guid(), string documentNumber="") : base(startDate, endDate)
        {
            SupplierId = supplierId;
            DocumentNumber = documentNumber;
        }
    }
}
