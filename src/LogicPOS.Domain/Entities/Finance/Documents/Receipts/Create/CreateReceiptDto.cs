namespace LogicPOS.Domain.Entities.Dtos;

public record CreateReceiptDto(Guid PaymentMethodId,
                               Guid CurrencyId,
                               decimal Amount,
                               decimal TaxPayable,
                               decimal CurrencyAmount,
                               decimal ExchangeRate,
                               string? Notes);