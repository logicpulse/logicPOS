using ErrorOr;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.PreferenceParameters.UpdatePreferenceParameter
{
    public class UpdatePreferenceParameterCommandHandler :
        RequestHandler<UpdatePreferenceParameterCommand, ErrorOr<Unit>>
    {
        public UpdatePreferenceParameterCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(UpdatePreferenceParameterCommand command,
                                                   CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommand($"preferenceparameters/{command.Id}", command, cancellationToken);
        }
    }
}
