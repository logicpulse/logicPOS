using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Enums;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("fin_documentordermain")]
public class Order : Entity
{
    public Table? Table { get; set; }
    public Guid TableId { get; set; }
    public OrderStatus OrderStatus { get; set; }

    public IList<Ticket>? Tickets { get; set; }

    internal async Task<Result> AddTicketsAsync(IEnumerable<CreateTicketDto> tickets,
                                                ITicketRepository ticketRepository,
                                                IArticleRepository articleRepository,
                                                CancellationToken cancellationToken = default)
    {
        foreach (var ticket in tickets)
        {
            var addTicketResult = await AddTicketAsync(ticket,
                                                       ticketRepository,
                                                       articleRepository,
                                                       cancellationToken);

            if (addTicketResult.IsFailure)
            {
                return addTicketResult;
            }
        }

        return Result.Success();
    }

    public async Task<Result<Ticket>> AddTicketAsync(CreateTicketDto ticketDto,
                                                   ITicketRepository ticketRepository,
                                                   IArticleRepository articleRepository,
                                                   CancellationToken cancellationToken = default)
    {
        var ticketCreator = new TicketCreator(ticketDto,
                                             ticketRepository,
                                             articleRepository);

        var createTicketResult = await ticketCreator.CreateAsync(cancellationToken);

        if (createTicketResult.IsFailure)
        {
            return createTicketResult;
        }

        var ticket = createTicketResult.Value!;
        ticket.OrderId = Id;

        Tickets ??= new List<Ticket>();

        Tickets.Add(ticket);

        return Result.Success(ticket);
    }

    public static Task<Result<Order>> CreateAsync(CreateOrderDto dto,
                                                  OrderReferences references,
                                                  CancellationToken cancellationToken = default)
    {
        var orderCreator = new OrderCreator(dto, references);
        return orderCreator.CreateAsync(cancellationToken);
    }
}
