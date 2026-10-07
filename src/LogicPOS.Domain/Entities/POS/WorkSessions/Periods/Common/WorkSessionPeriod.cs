using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Enums;

namespace LogicPOS.Domain.Entities;

//[Table("pos_worksessionperiod")]
public class WorkSessionPeriod : Entity
{
    public WorkSessionPeriodType Type { get; set; }
    public WorkSessionPeriodStatus Status { get; set; }
    public string Designation { get; set; } = null!;
    public DateTime StartDate { get; set; } = DateTime.Now;
    public DateTime? EndDate { get; set; }
    public Guid? ParentId { get; set; }
}

