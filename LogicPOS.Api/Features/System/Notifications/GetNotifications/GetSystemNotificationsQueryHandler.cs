using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.System.Notifications.GetAllSystemNotifications
{
    public class GetSystemNotificationsQueryHandler :
        RequestHandler<GetSystemNotificationsQuery, ErrorOr<IEnumerable<SystemNotificationViewModel>>>
    {
        public GetSystemNotificationsQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<SystemNotificationViewModel>>> Handle(GetSystemNotificationsQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetListQueryAsync<SystemNotificationViewModel>($"system/system-notifications", cancellationToken);
        }
    }
}
