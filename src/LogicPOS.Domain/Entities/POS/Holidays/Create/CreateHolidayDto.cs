namespace LogicPOS.Domain.Entities.Dtos;

public record CreateHolidayDto(
    string Designation,
    uint Day,
    uint Month,
    uint Year,
    bool Fixed,
    string? Description,
    string? Notes,
    bool? IsDeleted
);