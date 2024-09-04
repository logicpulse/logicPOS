using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.MeasurementUnits.GetAllMeasurementUnits
{
    public class GetAllMeasurementUnitsQueryHandler :
        RequestHandler<GetAllMeasurementUnitsQuery, ErrorOr<IEnumerable<MeasurementUnit>>>
    {
        public GetAllMeasurementUnitsQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<MeasurementUnit>>> Handle(GetAllMeasurementUnitsQuery query,
                                                                     CancellationToken cancellationToken = default)
        {
            return await HandleGetAllQuery<MeasurementUnit>("articles/measurementunits", cancellationToken);
        }
    }
}
