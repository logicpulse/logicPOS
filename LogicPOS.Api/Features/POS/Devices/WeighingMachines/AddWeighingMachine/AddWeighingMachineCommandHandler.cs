using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.WeighingMachines.AddWeighingMachine
{
    public class AddWeighingMachineCommandHandler
        : RequestHandler<AddWeighingMachineCommand, ErrorOr<Guid>>
    {
        public AddWeighingMachineCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddWeighingMachineCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("weighingmachines", command, cancellationToken);
        }
    }
}
