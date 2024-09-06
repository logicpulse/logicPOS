using ErrorOr;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.WeighingMachines.UpdateWeighingMachine
{
    public class UpdateWeighingMachineCommandHandler
         : RequestHandler<UpdateWeighingMachineCommand, ErrorOr<Unit>>
    {
        public UpdateWeighingMachineCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(UpdateWeighingMachineCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"weighingmachines/{command.Id}", command, cancellationToken);
        }
    }
}
