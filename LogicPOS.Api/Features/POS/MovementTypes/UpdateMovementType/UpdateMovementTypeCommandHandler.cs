using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.MovementTypes.UpdateMovementType
{
    public class UpdateMovementTypeCommandHandler :
        RequestHandler<UpdateMovementTypeCommand, ErrorOr<Success>>
    {
        public UpdateMovementTypeCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Success>> Handle(UpdateMovementTypeCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"/movementtypes/{command.Id}", command, cancellationToken);
        }
    }
}
