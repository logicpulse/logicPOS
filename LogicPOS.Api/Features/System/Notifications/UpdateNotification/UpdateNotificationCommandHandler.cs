using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.System.Notifications.UpdateNotification
{
    public class UpdateNotificationCommandHandler :
        RequestHandler<UpdateNotificationCommand, ErrorOr<Success>>
    {
        public UpdateNotificationCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Success>> Handle(UpdateNotificationCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"/system/system-notification/{command.Id}", command, cancellationToken);
        }
    }
}
