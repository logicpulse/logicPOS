namespace LogicPOS.ApiServer.DTOs;

public sealed class PaymentMethodResponse
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
    public string? Token { get; set; }
    public string? ResourceString { get; set; }
    public string? ButtonIcon { get; set; }
    public string? Acronym { get; set; }
    public string? AllowPayback { get; set; }
    public string? Symbol { get; set; }
}

public sealed class AddPaymentMethodRequest
{
    public string Designation { get; set; } = string.Empty;
    public string? Token { get; set; }
    public string? Acronym { get; set; }
    public string? Notes { get; set; }
}

public sealed class UpdatePaymentMethodRequest
{
    public uint Order { get; set; }
    public string? Code { get; set; }
    public string Designation { get; set; } = string.Empty;
    public string? Token { get; set; }
    public string? ResourceString { get; set; }
    public string? ButtonIcon { get; set; }
    public string? Acronym { get; set; }
    public string? AllowPayback { get; set; }
    public string? Symbol { get; set; }
    public string? Notes { get; set; }
    public bool IsDeleted { get; set; }
}
