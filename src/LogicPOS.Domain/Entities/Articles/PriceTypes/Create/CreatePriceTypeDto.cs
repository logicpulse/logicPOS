namespace LogicPOS.Domain.Entities.Dtos;

public record CreatePriceTypeDto(string Designation,
                                 int EnumValue,
                                 string? Notes,
                                 bool? IsDeleted);