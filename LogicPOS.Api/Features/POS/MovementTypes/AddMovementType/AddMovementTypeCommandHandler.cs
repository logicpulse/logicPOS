using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.MovementTypes.AddMovementType
{
    public class AddMovementTypeCommandHandler : RequestHandler<AddMovementTypeCommand, ErrorOr<Guid>>
    {
        public AddMovementTypeCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(
            AddMovementTypeCommand command,
            CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("/movementtypes", command, cancellationToken);
        }
    }
}
