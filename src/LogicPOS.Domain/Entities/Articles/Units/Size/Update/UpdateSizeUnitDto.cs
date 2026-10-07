namespace LogicPOS.Domain.Entities.Dtos;

public record UpdateSizeUnitDto (uint Order,
                                 string Code,
                                 string Designation,
                                 string? Notes,
                                 bool IsDeleted);