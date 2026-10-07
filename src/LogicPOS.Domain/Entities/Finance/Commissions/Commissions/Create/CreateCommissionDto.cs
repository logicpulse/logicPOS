namespace LogicPOS.Domain.Entities.Dtos;

public record CreateCommissionDto(
    Guid? CreatedBy,
    Guid? CreatedWhere,
    Guid CommissionGroupId,
    Guid DocumentDetailId,
    Guid UserId,
    decimal CommissionValue,
    decimal Total,
    string? Notes
);