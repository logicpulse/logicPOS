namespace LogicPOS.Domain.Entities.Dtos;

public record UpdatePoleDisplayDto(
    uint Order,
    string Code,
    string Designation,
    string VendorId,
    string ProductId,
    string COMPort,
    string EndPoint,
    string CodeTable,
    uint CharactersPerLine,
    uint GoToStandByInSeconds,
    string? StandByLine1,
    string? StandByLine2,
    string? Notes,
    bool IsDeleted
);