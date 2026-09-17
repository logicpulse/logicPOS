using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Articles.StockManagement.UpdateStockMovement
{
    public class UpdateStockMovementCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public Guid? SupplierId { get; set; }
        public DateTime? Date { get; set; }
        public string DocumentNumber { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        public string ExternalDocument { get; set; }
    }
}
