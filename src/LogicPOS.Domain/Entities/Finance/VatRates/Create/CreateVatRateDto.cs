namespace LogicPOS.Domain.Entities.Dtos;

public record CreateVatRateDto(string Designation,
                               decimal Value,
                               string TaxType,
                               string TaxCode,
                               string CountryRegion,
                               string Description,
                               DateTime? ExpirationDate,
                               string? ReasonCode,
                               string? Notes,
                               bool? IsDeleted);