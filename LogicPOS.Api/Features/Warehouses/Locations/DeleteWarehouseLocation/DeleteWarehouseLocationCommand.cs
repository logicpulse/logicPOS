using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Warehouses.Locations.DeleteWarehouseLocation
{
    public class DeleteWarehouseLocationCommand : IRequest<ErrorOr<Unit>>
    {
        public Guid Id { get; }

        public DeleteWarehouseLocationCommand(Guid id)
        {
            Id = id;
        }
    }
}
