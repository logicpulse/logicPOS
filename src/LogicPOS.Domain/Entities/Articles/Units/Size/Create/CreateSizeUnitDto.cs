namespace LogicPOS.Domain.Entities.Dtos;

public record CreateSizeUnitDto (
    string Designation,
    string? Notes,
    bool? IsDeleted
);