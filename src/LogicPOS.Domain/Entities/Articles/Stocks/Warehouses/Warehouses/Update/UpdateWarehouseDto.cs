namespace LogicPOS.Domain.Entities.Dtos;

public record UpdateWarehouseDto
(
    Guid? UserId,
    Guid? TerminalId,
    uint Order,
    string Code,
    string Designation,
    bool IsDefault,
    string? Notes,
    bool IsDeleted
);
