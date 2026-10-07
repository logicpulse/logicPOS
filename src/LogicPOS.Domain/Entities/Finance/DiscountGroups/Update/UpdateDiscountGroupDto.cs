namespace LogicPOS.Domain.Entities.Dtos;

public record UpdateDiscountGroupDto(
    uint Order,
    string Code,
    string Designation,
    string? Notes,
    bool IsDeleted
);