using ErrorOr;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.MovementTypes.UpdateMovementType
{
    public class UpdateMovementTypeCommandHandler :
        RequestHandler<UpdateMovementTypeCommand, ErrorOr<Unit>>
    {
        public UpdateMovementTypeCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(UpdateMovementTypeCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommand($"/movementtypes/{command.Id}", command, cancellationToken);
        }
    }
}
