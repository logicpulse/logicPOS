namespace LogicPOS.Domain.Entities.Dtos;

public record UpdatePaymentMethodDto(
    Guid? UserId,
    Guid? TerminalId,
    uint Order,
    string Code,
    string Designation,
    string Token,
    string? ResourceString,
    string? ButtonIcon,
    string Acronym,
    string? AllowPayback,
    string? Symbol,
    string? Notes,
    bool IsDeleted
);