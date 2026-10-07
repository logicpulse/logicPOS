namespace LogicPOS.Domain.Entities.Dtos;

public record UpdateMovementTypeDto(
    uint Order,
    string Code,
    string Designation,
    bool VatDirectSelling,
    string? Notes,
    bool IsDeleted
);