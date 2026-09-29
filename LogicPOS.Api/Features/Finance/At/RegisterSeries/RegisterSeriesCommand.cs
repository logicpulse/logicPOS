using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Finance.At.RegisterSeries
{
    public class RegisterSeriesCommand : IRequest<ErrorOr<AtSeriesInfo>>
    {
        public Guid SeriesId { get; set; }
        public RegisterSeriesCommand(Guid seriesId) => SeriesId = seriesId;
    }
}
