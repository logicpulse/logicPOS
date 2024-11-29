using ErrorOr;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Warehouses.Locations.DeleteWarehouseLocation
{
    public class DeleteWarehouseLocationCommandHandler :
        RequestHandler<DeleteWarehouseLocationCommand, ErrorOr<bool>>
    {
        public DeleteWarehouseLocationCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<bool>> Handle(DeleteWarehouseLocationCommand request, CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"warehouses/locations/{request.Id}", cancellationToken);
        }
    }
}
