namespace LogicPOS.Domain.Entities.Dtos;

public record CreatePermissionGroupDto(string Designation,
                                       string? Notes,
                                       bool? IsDeleted);