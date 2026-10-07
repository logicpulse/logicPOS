using System.Collections;

namespace LogicPOS.Application.Features.System.Licensing;

public interface ILicenseData
{
    public bool IsLicensed { get; }
    public string Version { get; }
    public string HardwareId { get; }
    public LicenseStatus Status { get; }
    public DateOnly? Date { get; }
    public string? Name { get; }
    public string? Company { get; }
    public string? Nif { get; }
    public string? Address { get; }
    public string? Email { get; }
    public string? Phone { get; }
    public string? Reseller { get; }
    public bool StocksModule { get; }
    public bool AgtFeModule { get; }
    public bool SmsModule { get; }
    public int NumberSms { get; }
    public DateOnly? AllUpdateExpirationDate { get; }
    public int AllNumberOfDevices { get; }
    public bool HasExpired { get; }
    public bool IsValid { get; }
}