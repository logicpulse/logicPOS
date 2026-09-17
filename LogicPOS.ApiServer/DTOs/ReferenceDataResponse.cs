namespace LogicPOS.ApiServer.DTOs;

public abstract class ApiEntityResponse
{
    public Guid Id { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
}

public sealed class CountryResponse : ApiEntityResponse
{
    public uint Order { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string Code2 { get; set; } = string.Empty;
    public string Code3 { get; set; } = string.Empty;
    public string Capital { get; set; } = string.Empty;
    public string TLD { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = string.Empty;
    public string FiscalNumberRegex { get; set; } = string.Empty;
    public string ZipCodeRegex { get; set; } = string.Empty;
}

public sealed class CurrencyResponse : ApiEntityResponse
{
    public uint Order { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string Acronym { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public string Entity { get; set; } = string.Empty;
    public decimal ExchangeRate { get; set; }
}
