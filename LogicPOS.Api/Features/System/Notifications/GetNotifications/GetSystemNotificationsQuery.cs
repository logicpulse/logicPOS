using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogicPOS.Api.Features.System.Notifications.GetAllSystemNotifications
{
    public class GetSystemNotificationsQuery : IRequest<ErrorOr<IEnumerable<SystemNotificationViewModel>>>
    {

    }
}
