using ErrorOr;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Places.UpdatePlace
{
    public class UpdatePlaceCommandHandler
         : RequestHandler<UpdatePlaceCommand, ErrorOr<Unit>>
    {
        public UpdatePlaceCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(UpdatePlaceCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommand($"places/{command.Id}", command, cancellationToken);
        }
    }
}
