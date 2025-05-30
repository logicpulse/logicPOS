using ErrorOr;
using MediatR;
using System;
using System.Text.Json.Serialization;

namespace LogicPOS.Api.Features.FiscalYears.CreateDefaultSeries
{
    public class CreateDefaultSeriesCommand : IRequest<ErrorOr<Unit>>
    {
        public CreateDefaultSeriesCommand(Guid id) => Id = id;

        [JsonIgnore]
        public Guid Id { get; set; }
        public string Options { get; set; }
    }
}
