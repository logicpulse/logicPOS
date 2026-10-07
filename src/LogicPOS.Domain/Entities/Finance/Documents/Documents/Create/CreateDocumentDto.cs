using LogicPOS.Domain.ValueObjects;

namespace LogicPOS.Domain.Entities.Dtos;

public record CreateDocumentDto(string Type,
                                Guid? CustomerId,
                                Guid? PaymentConditionId,
                                Guid? CurrencyId,
                                Guid? ParentId,
                                ShipAddress? ShipToAddress,
                                ShipAddress? ShipFromAddress,
                                decimal Discount,
                                DocumentCustomer? Customer,
                                decimal? ExchangeRate,
                                decimal? TotalDelivery,
                                string? Notes,
                                IEnumerable<CreateDocumentDetailDto> Details,
                                IEnumerable<CreateDocumentPaymentMethodDto>? PaymentMethods,
                                bool IsDraft);