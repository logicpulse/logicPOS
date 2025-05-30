using ErrorOr;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.SizeUnits.UpdateSizeUnit
{
    public class UpdateSizeUnitCommandHandler :
        RequestHandler<UpdateSizeUnitCommand, ErrorOr<Unit>>
    {
        public UpdateSizeUnitCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(UpdateSizeUnitCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"articles/sizeunits/{command.Id}", command, cancellationToken);
        }
    }
}
