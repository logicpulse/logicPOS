using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Warehouses.Locations.AddWarehouseLocations
{
    public class AddWarehouseLocationsCommandHandler :
        RequestHandler<AddWarehouseLocationsCommand, ErrorOr<Success>>
    {
        public AddWarehouseLocationsCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Success>> Handle(AddWarehouseLocationsCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleNoResponsePostCommandAsync($"articles/stocks/warehouses/{command.Id}/locations", command, cancellationToken);
        }
    }
}
