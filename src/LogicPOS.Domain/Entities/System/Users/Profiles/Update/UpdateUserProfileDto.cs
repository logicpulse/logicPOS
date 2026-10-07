namespace LogicPOS.Domain.Entities.Dtos;

public record UpdateUserProfileDto(
    uint Order,
    string Code,
    string Designation,
    string? Notes,
    bool IsDeleted
    );
