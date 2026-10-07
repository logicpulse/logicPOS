using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Enums;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class OrderCreator : EntityCreator<Order>
{
    private readonly CreateOrderDto _dto;
    private readonly OrderReferences _references;
    public OrderCreator(CreateOrderDto dto,
                        OrderReferences references) :
        base(new())
    {
        _dto = dto;
        _references = references;
    }

    public override async Task<Result<Order>> CreateAsync(CancellationToken ct = default)
    {
        var createResult = await base.CreateAsync(ct);
        if (createResult.IsFailure)
        {
            return createResult;
        }

        var order = createResult.Value!;


        var addTicketsResult = await order.AddTicketsAsync(_dto.Tickets,
                                                           _references.TicketRepository,
                                                           _references.ArticleRepository,
                                                           ct);
        if (addTicketsResult.IsFailure)
        {
            return addTicketsResult.ToGenericFailure<Order>();
        }

        return order;
    }

    protected override Task AssignDataAsync(CancellationToken ct = default)
    {
        _entity.OrderStatus = OrderStatus.Open;
        _entity.TableId = _dto.TableId;
        return Task.FromResult(Result.Success());
    }

    protected override Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success());
    }

    protected override async Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        if(!await _references.TableExistsAsync(_dto.TableId, ct))
        {
            return Result.NotFound<Table>(_dto.TableId.ToString());
        }

        return Result.Success();
    }
}