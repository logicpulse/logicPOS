namespace LogicPOS.Domain.Entities.Dtos;

public record UpdateWeighingMachineDto(
    uint Order,
    string Code,
    string Designation,
    string PortName,
    uint BaudRate,
    string Parity,
    string StopBits,
    uint DataBits,
    string? Notes,
    bool IsDeleted
);