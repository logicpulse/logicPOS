using LogicPOS.Api.Features.Reports.Common;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Reports.GetStockReportPdf
{
    public class GetStockByWarehouseReportPdfQuery : ReportQuery
    {
        public Guid? ArticleId;
        public Guid? WarehouseId;
        public string SerialNumber;

        public GetStockByWarehouseReportPdfQuery(
            DateTime startDate,
            DateTime endDate,
            Guid? articleId,
            Guid? warehouseId,
            string serialNumber=null) : base(startDate, endDate, null, null)
        {
            ArticleId = articleId;
            WarehouseId = warehouseId;
            SerialNumber = serialNumber;
        }

        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {
            if (ArticleId.HasValue)
            {
                urlQueryBuilder.Append($"&ArticleId={ArticleId}");
            }

            if (WarehouseId.HasValue)
            {
                urlQueryBuilder.Append($"&WarehouseId={WarehouseId}");
            }

            if (!string.IsNullOrWhiteSpace(SerialNumber))
            {
                urlQueryBuilder.Append($"&SerialNumber={SerialNumber}");
            }
        }
    }
}
