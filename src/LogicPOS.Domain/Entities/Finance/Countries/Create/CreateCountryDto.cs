namespace LogicPOS.Domain.Entities.Dtos;

public record CreateCountryDto(string Designation,
                               string Code2,
                               string Code3,
                               string Capital,
                               string TLD,
                               string Currency,
                               string CurrencyCode,
                               string FiscalNumberRegex,
                               string ZipCodeRegex,
                               string? Notes,
                               bool? IsDeleted);
