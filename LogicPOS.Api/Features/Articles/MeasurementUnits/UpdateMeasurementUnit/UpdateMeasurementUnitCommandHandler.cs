using ErrorOr;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.MeasurementUnits.UpdateMeasurementUnit
{
    public class UpdateMeasurementUnitCommandHandler :
        RequestHandler<UpdateMeasurementUnitCommand, ErrorOr<Unit>>
    {
        public UpdateMeasurementUnitCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(UpdateMeasurementUnitCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommand($"/articles/measurementunits/{command.Id}", command, cancellationToken);
        }
    }
}
