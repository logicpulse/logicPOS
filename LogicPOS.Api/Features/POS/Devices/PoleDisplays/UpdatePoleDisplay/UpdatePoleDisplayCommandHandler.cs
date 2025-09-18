using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.PoleDisplays.UpdatePoleDisplay
{
    public class UpdatePoleDisplayCommandHandler :
        RequestHandler<UpdatePoleDisplayCommand, ErrorOr<Success>>
    {
        public UpdatePoleDisplayCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Success>> Handle(UpdatePoleDisplayCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"poledisplays/{command.Id}", command, cancellationToken);
        }
    }
}
