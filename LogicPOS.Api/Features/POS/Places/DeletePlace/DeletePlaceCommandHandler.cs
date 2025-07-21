using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Places.DeletePlace
{
    public class DeletePlaceCommandHandler :
        RequestHandler<DeletePlaceCommand, ErrorOr<bool>>
    {
        public DeletePlaceCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<bool>> Handle(DeletePlaceCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"places/{command.Id}", cancellationToken);
        }
    }
}
