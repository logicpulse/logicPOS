using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.PreferenceParameters.UpdatePreferenceParameter
{
    public class UpdatePreferenceParameterCommandHandler :
        RequestHandler<UpdatePreferenceParameterCommand, ErrorOr<Success>>
    {
        public UpdatePreferenceParameterCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Success>> Handle(UpdatePreferenceParameterCommand command,
                                                   CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"preference-parameters/{command.Id}", command, cancellationToken);
        }
    }
}
