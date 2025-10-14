using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Entities
{
    public class SystemNotificationViewModel : ApiEntity
    {
        public Guid TypeId { get; set; }

        public uint Ord { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime ReadingDate { get; set; }

        public Guid? TargetUserId { get; set; }

        public Guid? TargetTerminalId { get; set; }

        public Guid? LastReadUserId { get; set; }

        public Guid? LastReadTerminalId { get; set; }
    }
}
