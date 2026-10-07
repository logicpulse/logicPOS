namespace LogicPOS.Domain.Entities.Dtos;

public record UpdatePermissionGroupDto(
    uint Order,
    string Code,
    string Designation,
    string? Notes,
    bool IsDeleted
    );