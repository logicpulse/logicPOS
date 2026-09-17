namespace LogicPOS.ApiServer.DTOs;

public sealed class TerminalResponse
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
    public object? Printer { get; set; }
    public Guid? PrinterId { get; set; }
    public object? WeighingMachine { get; set; }
    public Guid? WeighingMachineId { get; set; }
    public object? Place { get; set; }
    public Guid? PlaceId { get; set; }
    public object? ThermalPrinter { get; set; }
    public Guid? ThermalPrinterId { get; set; }
    public object? BarcodeReader { get; set; }
    public Guid? BarcodeReaderId { get; set; }
    public object? CardReader { get; set; }
    public Guid? CardReaderId { get; set; }
    public object? PoleDisplay { get; set; }
    public Guid? PoleDisplayId { get; set; }
    public string HardwareId { get; set; } = string.Empty;
    public uint TimerInterval { get; set; }
    public bool IsDefault { get; set; }
}

public sealed class CreateTerminalRequest
{
    public string HardwareId { get; set; } = string.Empty;
}

public sealed class AddEntityIdResponse
{
    public Guid Id { get; set; }
}
