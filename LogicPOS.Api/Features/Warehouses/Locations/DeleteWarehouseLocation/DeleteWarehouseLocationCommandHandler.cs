using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Warehouses.Locations.DeleteWarehouseLocation
{
    public class DeleteWarehouseLocationCommandHandler :
        RequestHandler<DeleteWarehouseLocationCommand, ErrorOr<Unit>>
    {
        public DeleteWarehouseLocationCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(DeleteWarehouseLocationCommand request, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"warehouses/locations/{request.Id}", cancellationToken);
                return await HandleDeleteEntityHttpResponseAsync(response);
            }
            catch (HttpRequestException)
            {
                return ApiErrors.CommunicationError;
            }
        }
    }
}
