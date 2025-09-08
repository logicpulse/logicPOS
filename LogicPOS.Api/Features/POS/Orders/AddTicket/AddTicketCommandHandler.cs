using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Orders.AddTicket
{
    public class AddTicketCommandHandler :
        RequestHandler<AddTicketCommand, ErrorOr<Guid>>
    {
        public AddTicketCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddTicketCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync($"orders/{command.OrderId}/tickets", command, cancellationToken);
        }
    }
}
