using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.System.Notifications.UpdateNotification
{
    public class UpdateNotificationCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public bool IsRead { get; set; }
        public DateTime ReadingDate { get; set; }
        public Guid? LastReadUserId { get; set; }
        public Guid? LastReadTerminalId { get; set; }

    }
}
