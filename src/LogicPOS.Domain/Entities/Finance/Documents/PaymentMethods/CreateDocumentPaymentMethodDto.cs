namespace LogicPOS.Domain.Entities.Dtos;

public record CreateDocumentPaymentMethodDto(Guid PaymentMethodId,
                                             decimal Amount);