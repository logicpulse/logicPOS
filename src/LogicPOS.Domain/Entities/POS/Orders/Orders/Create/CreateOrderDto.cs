namespace LogicPOS.Domain.Entities.Dtos;

public record CreateOrderDto(Guid TableId,
                             IEnumerable<CreateTicketDto> Tickets);