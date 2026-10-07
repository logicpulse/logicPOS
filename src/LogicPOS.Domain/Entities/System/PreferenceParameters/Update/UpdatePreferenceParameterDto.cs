namespace LogicPOS.Domain.Entities.Dtos;

public record UpdatePreferenceParameterDto(
    Guid Id,
    uint Order,
    string Code,
    string? Value,
    string? Notes
);