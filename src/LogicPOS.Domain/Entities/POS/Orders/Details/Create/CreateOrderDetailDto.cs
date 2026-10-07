namespace LogicPOS.Domain.Entities.Dtos;

public record CreateOrderDetailDto(Guid ArticleId,
                                   decimal Quantity,
                                   decimal UnitPrice);