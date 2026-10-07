namespace LogicPOS.Domain.Entities.Dtos;

public record CreateUserProfileDto(
    string Designation,
    string? AccessPassword,
    string? Notes,
    bool? IsDeleted
    );