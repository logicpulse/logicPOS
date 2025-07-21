using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Warehouses.Locations.UpdateWarehouseLocation
{
    public class UpdateWarehouseLocationCommandHandler :
        RequestHandler<UpdateWarehouseLocationCommand, ErrorOr<Unit>>
    {
        public UpdateWarehouseLocationCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(UpdateWarehouseLocationCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"articles/stocks/warehouses/locations/{command.Id}", command, cancellationToken);
        }
    }
}
