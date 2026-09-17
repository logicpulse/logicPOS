using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Articles.StockManagement.ChangeArticleLocation
{
    public class ChangeArticleLocationCommand : IRequest<ErrorOr<Success>>
    {
        public Guid WarehouseArticleId { get;  }
        public Guid LocationId { get;  }
        public decimal Quantity { get; }

        public ChangeArticleLocationCommand(Guid warehouseArticleId, Guid targetLocationId, decimal quantity)
        {
            WarehouseArticleId = warehouseArticleId;
            LocationId = targetLocationId;
            Quantity = quantity;
        }
    }
}
