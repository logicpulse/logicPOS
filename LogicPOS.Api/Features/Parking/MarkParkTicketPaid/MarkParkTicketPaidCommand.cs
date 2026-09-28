using ErrorOr;
using LogicPOS.Api.Features.Parking.Common;
using MediatR;

namespace LogicPOS.Api.Features.Parking.MarkParkTicketPaid
{
    public class MarkParkTicketPaidCommand : IRequest<ErrorOr<TrackParkTicketPaidResult>>
    {
        public string Payload { get; set; }
        public decimal AmountPaid { get; set; }
        public string DocumentId { get; set; }
    }
}
