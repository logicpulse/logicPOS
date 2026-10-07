using LogicPOS.Domain.Entities.POS.WorkSessions.Movements.Common;

namespace LogicPOS.Domain.Entities.Dtos;

public record CreateWorkSessionMovementDto(Guid PeriodId,
                                           WorkSessionMovementType Type,
                                           decimal Amount,
                                           Guid? DocumentId,
                                           Guid? PaymentId,
                                           string? Notes);