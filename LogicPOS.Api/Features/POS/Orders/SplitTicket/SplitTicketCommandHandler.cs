using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Orders.SplitTicket
{
    public class SplitTicketCommandHandler :
        RequestHandler<SplitTicketCommand, ErrorOr<Success>>
    {
        public SplitTicketCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Success>> Handle(SplitTicketCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"orders/{command.OrderId}/split-ticket", command, cancellationToken);
        }
    }
}
