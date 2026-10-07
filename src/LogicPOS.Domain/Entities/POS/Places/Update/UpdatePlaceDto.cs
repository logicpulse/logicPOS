namespace LogicPOS.Domain.Entities.Dtos;

public record UpdatePlaceDto (uint Order,
                              string Code,
                              string Designation,
                              string? ButtonImage,
                              string? TypeSubtotal,
                              string? AccountType,
                              int? OrderPrintMode,
                              Guid PriceTypeId,
                              Guid MovementTypeId,
                              string? Notes,
                              bool IsDeleted);