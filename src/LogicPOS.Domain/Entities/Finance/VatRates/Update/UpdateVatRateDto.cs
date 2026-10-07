namespace LogicPOS.Domain.Entities.Dtos;

public record UpdateVatRateDto(
    Guid? UserId,
    Guid? TerminalId,
    uint Order,
    string Code,
    string Designation,
    decimal Value,
    string TaxType,
    string TaxCode,
    string CountryRegion,
    string Description,
    DateTime? ExpirationDate,
    string? ReasonCode,
    string? Notes,
    bool IsDeleted
);