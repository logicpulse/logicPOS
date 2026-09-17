using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.System.Notifications.AddSystemNotifications
{
    public class AddSystemNotificationCommandHandler : RequestHandler<AddSystemNotificationCommand, ErrorOr<Guid>>
    {
        public AddSystemNotificationCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(
            AddSystemNotificationCommand command,
            CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("system/system-notifications", command, cancellationToken);
        }
    }
}
