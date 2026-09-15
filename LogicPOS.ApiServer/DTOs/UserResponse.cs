namespace LogicPOS.ApiServer.DTOs;

public sealed class UserResponse
{
    public Guid Id { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public object? Profile { get; set; }
    public Guid ProfileId { get; set; }
    public object? CommissionGroup { get; set; }
    public Guid? CommissionGroupId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Residence { get; set; } = string.Empty;
    public string Locality { get; set; } = string.Empty;
    public string ZipCpde { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string DateOfContract { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string MobilePhone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FiscalNumber { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string AssignedSeating { get; set; } = string.Empty;
    public string AccessPin { get; set; } = string.Empty;
    public string AccessCardNumber { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool PasswordReset { get; set; }
    public DateTime? PasswordResetDate { get; set; }
    public string BaseConsumption { get; set; } = string.Empty;
    public string BaseOffers { get; set; } = string.Empty;
    public string PVPOffers { get; set; } = string.Empty;
    public string Remarks { get; set; } = string.Empty;
    public string ButtonImage { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public uint Order { get; set; }
}
