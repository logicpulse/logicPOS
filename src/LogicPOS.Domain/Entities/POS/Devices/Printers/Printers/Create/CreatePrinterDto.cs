namespace LogicPOS.Domain.Entities.Dtos;

public record CreatePrinterDto(
    Guid PrinterTypeId,
    string Designation,
    string? NetworkName,
    string? ThermalEncoding,
    bool? ThermalPrintLogo,
    string? ThermalImageCompanyLogo,
    int? ThermalMaxCharsPerLineNormal,
    int? ThermalMaxCharsPerLineNormalBold,
    int? ThermalMaxCharsPerLineSmall,
    string? ThermalCutCommand,
    int? ThermalOpenDrawerValueM,
    int? ThermalOpenDrawerValueT1,
    int? ThermalOpenDrawerValueT2,
    bool? ShowInDialog,
    string? Notes,
    bool? IsDeleted
);