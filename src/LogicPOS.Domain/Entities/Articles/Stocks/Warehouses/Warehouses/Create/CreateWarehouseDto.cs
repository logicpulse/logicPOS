namespace LogicPOS.Domain.Entities.Dtos;

public record CreateWarehouseDto(string Designation,
                                 bool IsDefault,
                                 string? Notes,
                                 bool? IsDeleted,
                                 IEnumerable<string> Locations);
