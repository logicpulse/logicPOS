namespace LogicPOS.Domain.Entities.Dtos;

public record UpdateInputReaderDto(
    uint Order,
    string Code,
    string Designation,
    string ReaderSizes,
    string? Notes,
    bool IsDeleted
);