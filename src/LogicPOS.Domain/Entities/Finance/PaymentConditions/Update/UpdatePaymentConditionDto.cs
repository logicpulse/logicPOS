namespace LogicPOS.Domain.Entities.Dtos;

public record UpdatePaymentConditionDto(
    uint Order,
    string Code,
    string Designation,
    string Acronym,
    string? Notes,
    bool IsDeleted
);