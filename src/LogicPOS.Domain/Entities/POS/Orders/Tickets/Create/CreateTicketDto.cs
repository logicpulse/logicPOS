namespace LogicPOS.Domain.Entities.Dtos;

public record CreateTicketDto(Guid? OrderId, IEnumerable<CreateOrderDetailDto> Details);