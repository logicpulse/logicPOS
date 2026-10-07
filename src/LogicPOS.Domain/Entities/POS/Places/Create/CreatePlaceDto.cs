namespace LogicPOS.Domain.Entities.Dtos;

public record CreatePlaceDto (string Designation,
                              string? ButtonImage,
                              string? TypeSubtotal,
                              string? AccountType,
                              int? OrderPrintMode,
                              Guid PriceTypeId,
                              Guid? MovementTypeId,
                              string? Notes,
                              bool? IsDeleted,
                              bool Imported = false);