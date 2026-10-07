using System.ComponentModel.DataAnnotations.Schema;
using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.POS.WorkSessions.Movements.Common;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("pos_worksessionmovement")]
public class WorkSessionMovement : Entity
{
    public WorkSessionPeriod? Period { get; set; }
    public Guid PeriodId { get; set; }
    [Column(TypeName = "decimal(18,6)")] public decimal Amount { get; set; }
    public Document? Document { get; set; }
    public Guid? DocumentId { get; set; }
    public Payment? Payment { get; set; }
    public Guid? PaymentId { get; set; }
    public WorkSessionMovementType Type { get; set; }

    public static async Task<Result<WorkSessionMovement>> CreateAsync(CreateWorkSessionMovementDto dto,
        WorkSessionMovementReferences references,
        CancellationToken cancellationToken = default)
    {
        var creator = new WorkSessionMovementCreator(dto, references);
        return await creator.CreateAsync(cancellationToken);
    }
}