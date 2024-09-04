using ErrorOr;
using LogicPOS.Api.Features.Common;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.MeasurementUnits.AddMeasurementUnit
{
    public class AddMeasurementUnitCommandHandler : RequestHandler<AddMeasurementUnitCommand, ErrorOr<Guid>>
    {
        public AddMeasurementUnitCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddMeasurementUnitCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleAddCommand("articles/measurementunits", command, cancellationToken);
        }
    }
}
