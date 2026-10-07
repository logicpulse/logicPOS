namespace LogicPOS.Domain.Entities.Dtos;

public record UpdateCountryDto(uint Order,
                               string Code,
                               string Designation,
                               string Code2,
                               string Code3,
                               string Capital,
                               string TLD,
                               string Currency,
                               string CurrencyCode,
                               string FiscalNumberRegex,
                               string ZipCodeRegex,
                               string? Notes,
                               bool IsDeleted);

