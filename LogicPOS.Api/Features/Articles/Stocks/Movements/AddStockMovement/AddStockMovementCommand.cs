using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Articles.StockManagement.AddStockMovement
{
    public class AddStockMovementCommand : IRequest<ErrorOr<Success>>
    {
        public Guid SupplierId { get; set; }
        public DateTime Date { get; set; }
        public string DocumentNumber { get; set; }
        public string Notes { get; set; }
        public IEnumerable<StockMovementItem> Items { get; set; }
        public string ExternalDocument { get; set; }

    }
}
