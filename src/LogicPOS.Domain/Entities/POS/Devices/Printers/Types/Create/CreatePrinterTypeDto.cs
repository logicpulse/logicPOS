namespace LogicPOS.Domain.Entities.Dtos;

public record CreatePrinterTypeDto(
    string Designation,
    string? Token,
    bool ThermalPrinter,
    string? Notes,
    bool? IsDeleted
);