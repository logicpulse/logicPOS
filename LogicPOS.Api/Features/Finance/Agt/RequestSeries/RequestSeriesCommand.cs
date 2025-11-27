using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Finance.Agt.RequestSeries
{
    public class RequestSeriesCommand : IRequest<ErrorOr<AgtSeriesInfo>>
    {
        public int Year { get; set; }
        public string DocumentType { get; set; } 
        public string EstablishmentNumber { get; set; } 
        public string SeriesContingencyIndicator { get; set; }
    }
}
