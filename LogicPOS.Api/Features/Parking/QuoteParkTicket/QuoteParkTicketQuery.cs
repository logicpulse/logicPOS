using ErrorOr;
using LogicPOS.Api.Features.Parking.Common;
using MediatR;

namespace LogicPOS.Api.Features.Parking.QuoteParkTicket
{
    public class QuoteParkTicketQuery : IRequest<ErrorOr<TrackParkTicketQuoteResult>>
    {
        public string Payload { get; set; }
    }
}
