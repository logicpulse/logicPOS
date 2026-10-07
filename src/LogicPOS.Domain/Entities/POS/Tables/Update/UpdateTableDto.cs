namespace LogicPOS.Domain.Entities.Dtos;

public record UpdateTableDto(
    Guid PlaceId,
    uint Order,
    string Code,
    string Designation,
    string? Notes,
    bool IsDeleted);
