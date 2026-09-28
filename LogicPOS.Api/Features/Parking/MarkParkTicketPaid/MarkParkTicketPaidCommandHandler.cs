using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Parking.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Parking.MarkParkTicketPaid
{
    public class MarkParkTicketPaidCommandHandler : RequestHandler<MarkParkTicketPaidCommand, ErrorOr<TrackParkTicketPaidResult>>
    {
        public MarkParkTicketPaidCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override Task<ErrorOr<TrackParkTicketPaidResult>> Handle(MarkParkTicketPaidCommand request, CancellationToken cancellationToken = default)
        {
            return HandlePostCommandAsync<TrackParkTicketPaidResult>("park-tickets/paid", request, cancellationToken);
        }
    }
}
