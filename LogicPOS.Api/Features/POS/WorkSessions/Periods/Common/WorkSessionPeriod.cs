using LogicPOS.Api.Entities.Enums;
using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Entities
{
    public class WorkSessionPeriod : ApiEntity, IWithDesignation
    {
        public WorkSessionPeriodType Type { get; set; }
        public WorkSessionPeriodStatus Status { get; set; }
        public string Designation { get; set; } 
        public DateTime StartDate { get; set; } 
        public DateTime? EndDate { get; set; }
        public WorkSessionPeriod Parent { get; set; }
        public Guid? ParentId { get; set; }
    }
}
