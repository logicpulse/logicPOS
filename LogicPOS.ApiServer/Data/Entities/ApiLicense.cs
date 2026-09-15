namespace LogicPOS.ApiServer.Data.Entities;

public sealed class ApiLicense
{
    public int Id { get; set; }
    public bool IsLicensed { get; set; }
    public string Version { get; set; } = string.Empty;
    public string HardwareId { get; set; } = string.Empty;
    public int Status { get; set; }
    public DateTime? Date { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Nif { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Reseller { get; set; } = string.Empty;
    public bool StocksModule { get; set; }
    public bool AgtFeModule { get; set; }
    public DateTime? AllUpdateExpirationDate { get; set; }
    public int? AllNumberOfDevices { get; set; }
    public bool HasExpired { get; set; }
    public bool IsValid { get; set; }
}
