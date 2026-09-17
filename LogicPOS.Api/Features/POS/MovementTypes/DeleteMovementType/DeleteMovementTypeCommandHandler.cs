using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.MovementTypes.DeleteMovementType
{
    public class DeleteMovementTypeCommandHandler :
        RequestHandler<DeleteMovementTypeCommand, ErrorOr<bool>>
    {
        public DeleteMovementTypeCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<bool>> Handle(DeleteMovementTypeCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"movementtypes/{command.Id}", cancellationToken);
        }
    }
}
