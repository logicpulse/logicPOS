namespace LogicPOS.Domain.Entities.Dtos;

public record UpdateHolidayDto(
    uint Order,
    string Code,
    string Designation,
    uint Day,
    uint Month,
    uint Year,
    bool Fixed,
    string? Description,
    string? Notes,
    bool IsDeleted
);