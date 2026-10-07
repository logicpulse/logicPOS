namespace LogicPOS.Domain.Entities.Dtos;

public record UpdatePriceTypeDto(uint Order,
                                 string Code,
                                 string Designation,
                                 int EnumValue,
                                 string? Notes,
                                 bool IsDeleted);