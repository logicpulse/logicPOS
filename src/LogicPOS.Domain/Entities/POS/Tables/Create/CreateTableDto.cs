namespace LogicPOS.Domain.Entities.Dtos;

public record CreateTableDto (Guid PlaceId,
                              string Designation,
                              string? ButtonImage,
                              decimal? Discount,
                              string? Notes,
                              bool? IsDeleted,
                              bool Imported = false);