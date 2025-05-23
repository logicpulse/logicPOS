using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.GetStockReportPdf
{
    public class GetStockByWarehouseReportPdfQuery : StartAndEndDateReportQuery
    {
        public Guid ArticleId;
        public Guid WarehouseId;
        public string SerialNumber;

        public GetStockByWarehouseReportPdfQuery(DateTime startDate, DateTime endDate, Guid articleId= new Guid(), Guid warehouseId = new Guid(), string serialNumber="") : base(startDate, endDate)
        {
            ArticleId = articleId;
            WarehouseId = warehouseId;
            SerialNumber = serialNumber;
        }
    }
}
