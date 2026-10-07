namespace LogicPOS.Domain.Entities.Dtos;

public record CreateUserCommissionGroupDto(string Designation,
                                           decimal Commission,
                                           string? Notes,
                                           bool? IsDeleted);
