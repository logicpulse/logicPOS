namespace LogicPOS.ApiServer.DTOs;

public sealed class MovementTypeResponse
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
    public bool VatDirectSelling { get; set; }
}

public sealed class AddMovementTypeRequest
{
    public string Designation { get; set; } = string.Empty;
    public bool VatDirectSelling { get; set; }
    public string? Notes { get; set; }
}

public sealed class UpdateMovementTypeRequest
{
    public uint Order { get; set; }
    public string? Code { get; set; }
    public string Designation { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public bool VatDirectSelling { get; set; }
    public bool IsDeleted { get; set; }
}
