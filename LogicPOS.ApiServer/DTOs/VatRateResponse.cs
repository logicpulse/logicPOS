namespace LogicPOS.ApiServer.DTOs;

public sealed class VatRateResponse
{
    public Guid Id { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public uint Order { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string ReasonCode { get; set; } = string.Empty;
    public string TaxType { get; set; } = string.Empty;
    public string TaxCode { get; set; } = string.Empty;
    public string CountryRegion { get; set; } = string.Empty;
    public DateTime? ExpirationDate { get; set; }
    public string Description { get; set; } = string.Empty;
}

public sealed class AddVatRateRequest
{
    public string Designation { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string? ReasonCode { get; set; }
    public string? TaxType { get; set; }
    public string? TaxCode { get; set; }
    public string? CountryRegionCode { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
}

public sealed class UpdateVatRateRequest
{
    public uint Order { get; set; }
    public string? Code { get; set; }
    public string Designation { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string? ReasonCode { get; set; }
    public string? TaxType { get; set; }
    public string? TaxCode { get; set; }
    public string? CountryRegionCode { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public bool IsDeleted { get; set; }
}
