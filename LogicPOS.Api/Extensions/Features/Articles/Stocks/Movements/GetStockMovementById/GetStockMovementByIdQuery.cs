using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Articles.Stocks.Movements.GetStockMovementById
{
    public class GetStockMovementByIdQuery : IRequest<ErrorOr<StockMovement>>
    {
        public Guid Id { get; set; }

        public GetStockMovementByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
