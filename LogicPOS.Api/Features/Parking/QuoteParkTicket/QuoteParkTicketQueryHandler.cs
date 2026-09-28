using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Parking.Common;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Parking.QuoteParkTicket
{
    public class QuoteParkTicketQueryHandler : RequestHandler<QuoteParkTicketQuery, ErrorOr<TrackParkTicketQuoteResult>>
    {
        public QuoteParkTicketQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override Task<ErrorOr<TrackParkTicketQuoteResult>> Handle(QuoteParkTicketQuery request, CancellationToken cancellationToken = default)
        {
            var encoded = Uri.EscapeDataString(request.Payload ?? string.Empty);
            return HandleGetQueryAsync<TrackParkTicketQuoteResult>($"park-tickets/quote?payload={encoded}", cancellationToken);
        }
    }
}
