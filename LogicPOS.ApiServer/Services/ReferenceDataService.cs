using LogicPOS.ApiServer.DTOs;

namespace LogicPOS.ApiServer.Services;

public sealed class ReferenceDataService
{
    private static readonly Guid SystemUserId = Guid.Empty;
    private static readonly DateTime CatalogTimestamp = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private static readonly IReadOnlyList<CurrencyResponse> Currencies =
    [
        new CurrencyResponse
        {
            Id = Guid.Parse("b9a1a510-7d3e-4c7a-902a-70c4c605aa01"),
            Order = 1,
            Code = "EUR",
            Designation = "Euro",
            Acronym = "EUR",
            Symbol = "€",
            Entity = "European Union",
            ExchangeRate = 1m,
            CreatedAt = CatalogTimestamp,
            UpdatedAt = CatalogTimestamp,
            UpdatedBy = SystemUserId
        },
        new CurrencyResponse
        {
            Id = Guid.Parse("b9a1a510-7d3e-4c7a-902a-70c4c605aa02"),
            Order = 2,
            Code = "AOA",
            Designation = "Kwanza",
            Acronym = "AOA",
            Symbol = "Kz",
            Entity = "Angola",
            ExchangeRate = 1m,
            CreatedAt = CatalogTimestamp,
            UpdatedAt = CatalogTimestamp,
            UpdatedBy = SystemUserId
        }
    ];

    private static readonly IReadOnlyList<CountryResponse> Countries =
    [
        new CountryResponse
        {
            Id = Guid.Parse("c4f6e75b-bb90-4a5b-bb91-fb1f899f0001"),
            Order = 1,
            Code = "PT",
            Designation = "Portugal",
            Code2 = "PT",
            Code3 = "PRT",
            Capital = "Lisbon",
            TLD = ".pt",
            Currency = "Euro",
            CurrencyCode = "EUR",
            FiscalNumberRegex = "^[0-9]{9,}$",
            ZipCodeRegex = @"^\d{4}-\d{3}$",
            CreatedAt = CatalogTimestamp,
            UpdatedAt = CatalogTimestamp,
            UpdatedBy = SystemUserId
        },
        new CountryResponse
        {
            Id = Guid.Parse("c4f6e75b-bb90-4a5b-bb91-fb1f899f0002"),
            Order = 2,
            Code = "AO",
            Designation = "Angola",
            Code2 = "AO",
            Code3 = "AGO",
            Capital = "Luanda",
            TLD = ".ao",
            Currency = "Kwanza",
            CurrencyCode = "AOA",
            FiscalNumberRegex = "^[0-9A-Z]{9,}$",
            ZipCodeRegex = string.Empty,
            CreatedAt = CatalogTimestamp,
            UpdatedAt = CatalogTimestamp,
            UpdatedBy = SystemUserId
        }
    ];

    public IReadOnlyList<CountryResponse> GetCountries()
    {
        return Countries;
    }

    public IReadOnlyList<CurrencyResponse> GetCurrencies()
    {
        return Currencies;
    }

    public CurrencyResponse? GetCurrencyByCode(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return null;
        }

        return Currencies.FirstOrDefault(currency =>
                   string.Equals(currency.Code, code, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(currency.Acronym, code, StringComparison.OrdinalIgnoreCase));
    }
}
