namespace LogicPOS.Domain.Entities.Dtos;

public record UpdatePrinterTypeDto(uint Order,
                                   string Code,
                                   string Designation,
                                   string? Token,
                                   bool ThermalPrinter,
                                   string? Notes,
                                   bool IsDeleted);