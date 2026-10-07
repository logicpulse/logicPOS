namespace LogicPOS.Domain.Entities.Dtos;

public record CreateDiscountGroupDto(string Designation,
                                     string? Notes,
                                     bool? IsDeleted);