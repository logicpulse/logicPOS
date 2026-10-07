namespace LogicPOS.Domain.Entities.Dtos;

public record UpdateUserCommissionGroupDto(uint Order,
                                           string Code,
                                           string Designation,
                                           decimal Commission,
                                           string? Notes,
                                           bool IsDeleted);
