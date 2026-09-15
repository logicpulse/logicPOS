namespace LogicPOS.ApiServer.DTOs;

public sealed class LicenseResponse
{
    public LicenseDataResponse Data { get; set; } = new();
}

public sealed class LicenseDataResponse
{
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

public sealed class HardwareIdResponse
{
    public string HardwareId { get; set; } = string.Empty;
}

public sealed class ConnectResponse
{
    public bool Connected { get; set; }
}

public sealed class ActivateLicenseRequest
{
    public string Name { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string FiscalNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string HardwareId { get; set; } = string.Empty;
    public string AssemblyVersion { get; set; } = string.Empty;
    public int IdCountry { get; set; }
    public string SoftwareKey { get; set; } = string.Empty;
}

public sealed class ActivateLicenseResponseDto
{
    public bool Success { get; set; }
    public string LicenseData { get; set; } = string.Empty;
}
