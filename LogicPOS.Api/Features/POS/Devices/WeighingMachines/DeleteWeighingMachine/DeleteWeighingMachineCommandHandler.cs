using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.WeighingMachines.DeleteWeighingMachine
{
    public class DeleteWeighingMachineCommandHandler :
        RequestHandler<DeleteWeighingMachineCommand, ErrorOr<bool>>
    {
        public DeleteWeighingMachineCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<bool>> Handle(DeleteWeighingMachineCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"weighingmachines/{command.Id}", cancellationToken);
        }
    }
}
