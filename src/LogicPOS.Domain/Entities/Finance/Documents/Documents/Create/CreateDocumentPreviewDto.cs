using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.ValueObjects;

namespace LogicPOS.Domain.Entities.Finance.Documents.Documents.Create;

public record CreateDocumentPreviewDto(
    string? Type,
    Guid? CurrencyId,
    ShipAddress? ShipToAddress,
    ShipAddress? ShipFromAddress,
    decimal Discount,
    decimal? ExchangeRate,
    decimal? TotalDelivery,
    string? Notes,
    IEnumerable<CreateDocumentDetailDto> Details);