using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Entities
{
    public class PermissionProfile : ApiEntity
    {
        public Guid UserProfileId { get; set; }
        public Guid PermissionItemId { get; set; }
        public bool Granted { get; set; }
    }
}
