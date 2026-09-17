using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.MeasurementUnits.DeleteMeasurementUnit
{
    public class DeleteMeasurementUnitCommandHandler :
        RequestHandler<DeleteMeasurementUnitCommand, ErrorOr<bool>>
    {
        public DeleteMeasurementUnitCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<bool>> Handle(DeleteMeasurementUnitCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"articles/measurementunits/{command.Id}", cancellationToken);
        }
    }
}
