using LogicPOS.ApiServer.Data;
using LogicPOS.ApiServer.Data.Entities;
using LogicPOS.ApiServer.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.ApiServer.Services;

public sealed class OrderService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ArticleService _articleService;
    private readonly TableService _tableService;

    public OrderService(ApplicationDbContext dbContext, ArticleService articleService, TableService tableService)
    {
        _dbContext = dbContext;
        _articleService = articleService;
        _tableService = tableService;
    }

    public async Task<IReadOnlyList<OrderResponse>> GetOpenOrdersAsync(Guid? tableId, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.ApiOrders.AsNoTracking()
            .Include(o => o.Tickets).ThenInclude(t => t.Details)
            .Where(o => o.Status == ApiOrderStatus.Open);

        if (tableId.HasValue)
        {
            query = query.Where(o => o.TableId == tableId.Value);
        }

        var orders = await query.ToListAsync(cancellationToken);
        return await MapManyAsync(orders, cancellationToken);
    }

    public async Task<OrderResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _dbContext.ApiOrders.AsNoTracking()
            .Include(o => o.Tickets).ThenInclude(t => t.Details)
            .SingleOrDefaultAsync(o => o.Id == id, cancellationToken);

        if (order is null)
        {
            return null;
        }

        return (await MapManyAsync([order], cancellationToken)).Single();
    }

    public async Task<(bool Success, string? Field, string? Error, AddEntityIdResponse? Response)> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var table = await _dbContext.ApiTables.SingleOrDefaultAsync(t => t.Id == request.TableId, cancellationToken);
        if (table is null)
        {
            return (false, "tableId", "TableId não existe.", null);
        }

        if (request.Tickets.Count == 0 || request.Tickets.All(t => t.Details.Count == 0))
        {
            return (false, "tickets", "Pelo menos um item é obrigatório.", null);
        }

        var order = new ApiOrder
        {
            Id = Guid.NewGuid(),
            TableId = request.TableId,
            Status = ApiOrderStatus.Open,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
        };

        var ticketNumber = 1;
        foreach (var ticketRequest in request.Tickets)
        {
            var ticket = new ApiOrderTicket
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                TicketNumber = ticketNumber++,
                CreatedUtc = DateTime.UtcNow
            };

            foreach (var detailRequest in ticketRequest.Details)
            {
                var (detail, error) = await BuildDetailAsync(ticket.Id, detailRequest.ArticleId, detailRequest.Quantity, detailRequest.UnitPrice, cancellationToken);
                if (error is not null)
                {
                    return (false, "articleId", error, null);
                }

                ticket.Details.Add(detail!);
            }

            order.Tickets.Add(ticket);
        }

        _dbContext.ApiOrders.Add(order);

        table.Status = ApiTableStatus.Open;
        table.OpennedAtUtc = DateTime.UtcNow;
        table.UpdatedUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return (true, null, null, new AddEntityIdResponse { Id = order.Id });
    }

    public async Task<(bool Success, string? Field, string? Error, AddEntityIdResponse? Response)> AddTicketAsync(Guid orderId, AddTicketRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _dbContext.ApiOrders.Include(o => o.Tickets).SingleOrDefaultAsync(o => o.Id == orderId, cancellationToken);
        if (order is null)
        {
            return (false, null, "Order não encontrada.", null);
        }

        if (order.Status != ApiOrderStatus.Open)
        {
            return (false, null, "Order não está aberta.", null);
        }

        if (request.Details.Count == 0)
        {
            return (false, "details", "Pelo menos um item é obrigatório.", null);
        }

        var ticket = new ApiOrderTicket
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            TicketNumber = (order.Tickets.Count == 0 ? 0 : order.Tickets.Max(t => t.TicketNumber)) + 1,
            CreatedUtc = DateTime.UtcNow
        };

        foreach (var detailRequest in request.Details)
        {
            var (detail, error) = await BuildDetailAsync(ticket.Id, detailRequest.ArticleId, detailRequest.Quantity, detailRequest.UnitPrice, cancellationToken);
            if (error is not null)
            {
                return (false, "articleId", error, null);
            }

            ticket.Details.Add(detail!);
        }

        _dbContext.ApiOrderTickets.Add(ticket);
        order.UpdatedUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return (true, null, null, new AddEntityIdResponse { Id = ticket.Id });
    }

    public async Task<(bool Success, string? Error)> ChangeTableAsync(Guid orderId, ChangeOrderTableRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _dbContext.ApiOrders.SingleOrDefaultAsync(o => o.Id == orderId, cancellationToken);
        if (order is null)
        {
            return (false, "Order não encontrada.");
        }

        if (!await _dbContext.ApiTables.AnyAsync(t => t.Id == request.NewTableId, cancellationToken))
        {
            return (false, "NewTableId não existe.");
        }

        order.TableId = request.NewTableId;
        order.UpdatedUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<bool> CloseAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _dbContext.ApiOrders.SingleOrDefaultAsync(o => o.Id == orderId, cancellationToken);
        if (order is null || order.Status != ApiOrderStatus.Open)
        {
            return false;
        }

        order.Status = ApiOrderStatus.Closed;
        order.UpdatedUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid orderId, string? reason, CancellationToken cancellationToken = default)
    {
        var order = await _dbContext.ApiOrders.SingleOrDefaultAsync(o => o.Id == orderId, cancellationToken);
        if (order is null)
        {
            return false;
        }

        order.Status = ApiOrderStatus.Deleted;
        order.DeleteReason = reason;
        order.UpdatedUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<(bool Success, string? Field, string? Error)> ReduceItemsAsync(Guid orderId, ReduceOrderItemsRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _dbContext.ApiOrders.Include(o => o.Tickets).ThenInclude(t => t.Details).SingleOrDefaultAsync(o => o.Id == orderId, cancellationToken);
        if (order is null)
        {
            return (false, null, "Order não encontrada.");
        }

        if (order.Status != ApiOrderStatus.Open)
        {
            return (false, null, "Order não está aberta.");
        }

        var allDetails = order.Tickets.SelectMany(t => t.Details).ToList();

        foreach (var item in request.Items)
        {
            var consumed = ConsumeQuantity(allDetails, item.ArticleId, item.UnitPrice, item.Quantity);
            if (consumed is null)
            {
                return (false, "articleId", $"Quantidade insuficiente para o artigo {item.ArticleId} ao preço {item.UnitPrice}.");
            }
        }

        RemoveEmptyDetails(order);
        order.UpdatedUtc = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return (true, null, null);
    }

    public async Task<(bool Success, string? Field, string? Error)> MoveTicketItemAsync(Guid orderId, MoveTicketItemRequest request, CancellationToken cancellationToken = default)
    {
        var sourceOrder = await _dbContext.ApiOrders.Include(o => o.Tickets).ThenInclude(t => t.Details).SingleOrDefaultAsync(o => o.Id == orderId, cancellationToken);
        if (sourceOrder is null)
        {
            return (false, null, "Order não encontrada.");
        }

        if (sourceOrder.Status != ApiOrderStatus.Open)
        {
            return (false, null, "Order de origem não está aberta.");
        }

        var targetTable = await _dbContext.ApiTables.SingleOrDefaultAsync(t => t.Id == request.TableId, cancellationToken);
        if (targetTable is null)
        {
            return (false, "tableId", "TableId de destino não existe.");
        }

        var sourceDetails = sourceOrder.Tickets.SelectMany(t => t.Details).ToList();
        var consumed = ConsumeQuantity(sourceDetails, request.Item.ArticleId, request.Item.UnitPrice, request.Item.Quantity);
        if (consumed is null)
        {
            return (false, "articleId", "Quantidade insuficiente no item de origem.");
        }

        RemoveEmptyDetails(sourceOrder);
        sourceOrder.UpdatedUtc = DateTime.UtcNow;

        var targetOrder = await _dbContext.ApiOrders.Include(o => o.Tickets).ThenInclude(t => t.Details)
            .Where(o => o.TableId == request.TableId && o.Status == ApiOrderStatus.Open)
            .FirstOrDefaultAsync(cancellationToken);

        if (targetOrder is null)
        {
            targetOrder = new ApiOrder
            {
                Id = Guid.NewGuid(),
                TableId = request.TableId,
                Status = ApiOrderStatus.Open,
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow
            };
            _dbContext.ApiOrders.Add(targetOrder);

            targetTable.Status = ApiTableStatus.Open;
            targetTable.OpennedAtUtc = DateTime.UtcNow;
            targetTable.UpdatedUtc = DateTime.UtcNow;
        }

        var targetTicket = targetOrder.Tickets.FirstOrDefault();
        if (targetTicket is null)
        {
            targetTicket = new ApiOrderTicket
            {
                Id = Guid.NewGuid(),
                OrderId = targetOrder.Id,
                TicketNumber = 1,
                CreatedUtc = DateTime.UtcNow
            };
            _dbContext.ApiOrderTickets.Add(targetTicket);
        }

        _dbContext.ApiOrderDetails.Add(new ApiOrderDetail
        {
            Id = Guid.NewGuid(),
            TicketId = targetTicket.Id,
            ArticleId = consumed.Value.Detail.ArticleId,
            Designation = consumed.Value.Detail.Designation,
            Quantity = consumed.Value.Quantity,
            Price = consumed.Value.Detail.Price,
            Discount = consumed.Value.Detail.Discount,
            Vat = consumed.Value.Detail.Vat,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
        return (true, null, null);
    }

    public async Task<(bool Success, string? Field, string? Error)> SplitTicketAsync(Guid orderId, SplitTicketRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _dbContext.ApiOrders.Include(o => o.Tickets).ThenInclude(t => t.Details).SingleOrDefaultAsync(o => o.Id == orderId, cancellationToken);
        if (order is null)
        {
            return (false, null, "Order não encontrada.");
        }

        if (order.Status != ApiOrderStatus.Open)
        {
            return (false, null, "Order não está aberta.");
        }

        if (request.Items.Count == 0)
        {
            return (false, "items", "Pelo menos um item é obrigatório.");
        }

        var allDetails = order.Tickets.SelectMany(t => t.Details).ToList();
        var consumedItems = new List<(ApiOrderDetail Detail, decimal Quantity)>();

        foreach (var item in request.Items)
        {
            var consumed = ConsumeQuantity(allDetails, item.ArticleId, item.UnitPrice, item.Quantity);
            if (consumed is null)
            {
                return (false, "articleId", $"Quantidade insuficiente para o artigo {item.ArticleId} ao preço {item.UnitPrice}.");
            }

            consumedItems.Add(consumed.Value);
        }

        RemoveEmptyDetails(order);

        var newTicket = new ApiOrderTicket
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            TicketNumber = (order.Tickets.Count == 0 ? 0 : order.Tickets.Max(t => t.TicketNumber)) + 1,
            SplittersNumber = request.SplittersNumber,
            CreatedUtc = DateTime.UtcNow
        };

        foreach (var (originalDetail, quantity) in consumedItems)
        {
            newTicket.Details.Add(new ApiOrderDetail
            {
                Id = Guid.NewGuid(),
                TicketId = newTicket.Id,
                ArticleId = originalDetail.ArticleId,
                Designation = originalDetail.Designation,
                Quantity = quantity,
                Price = originalDetail.Price,
                Discount = originalDetail.Discount,
                Vat = originalDetail.Vat,
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow
            });
        }

        _dbContext.ApiOrderTickets.Add(newTicket);
        order.UpdatedUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return (true, null, null);
    }

    private async Task<(ApiOrderDetail? Detail, string? Error)> BuildDetailAsync(Guid ticketId, Guid articleId, decimal quantity, decimal unitPrice, CancellationToken cancellationToken)
    {
        var article = await _dbContext.ApiArticles.AsNoTracking().SingleOrDefaultAsync(a => a.Id == articleId, cancellationToken);
        if (article is null)
        {
            return (null, $"ArticleId {articleId} não existe.");
        }

        var vatRate = await _dbContext.ApiVatRates.AsNoTracking().SingleOrDefaultAsync(v => v.Id == article.VatDirectSellingId, cancellationToken);

        return (new ApiOrderDetail
        {
            Id = Guid.NewGuid(),
            TicketId = ticketId,
            ArticleId = articleId,
            Designation = article.Designation,
            Quantity = quantity,
            Price = unitPrice,
            Discount = 0,
            Vat = vatRate?.Value ?? 0,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
        }, null);
    }

    /// <summary>
    /// Greedily reduces <paramref name="quantity"/> worth of (articleId, unitPrice) across the given detail
    /// lines (which may span multiple tickets), consuming from the first matching lines found. Returns the
    /// last consumed line (for its Designation/Discount/Vat snapshot) and the total quantity consumed, or
    /// null if there wasn't enough matching quantity across all lines.
    /// </summary>
    private static (ApiOrderDetail Detail, decimal Quantity)? ConsumeQuantity(List<ApiOrderDetail> details, Guid articleId, decimal unitPrice, decimal quantity)
    {
        if (quantity <= 0)
        {
            return null;
        }

        var matches = details.Where(d => d.ArticleId == articleId && d.Price == unitPrice).ToList();
        var available = matches.Sum(d => d.Quantity);
        if (available < quantity)
        {
            return null;
        }

        ApiOrderDetail? lastMatch = null;
        var remaining = quantity;
        foreach (var match in matches)
        {
            if (remaining <= 0)
            {
                break;
            }

            var take = Math.Min(match.Quantity, remaining);
            match.Quantity -= take;
            remaining -= take;
            lastMatch = match;
        }

        return (lastMatch!, quantity);
    }

    private void RemoveEmptyDetails(ApiOrder order)
    {
        foreach (var ticket in order.Tickets)
        {
            var empty = ticket.Details.Where(d => d.Quantity <= 0).ToList();
            foreach (var detail in empty)
            {
                ticket.Details.Remove(detail);
                _dbContext.ApiOrderDetails.Remove(detail);
            }
        }
    }

    private async Task<List<OrderResponse>> MapManyAsync(List<ApiOrder> orders, CancellationToken cancellationToken)
    {
        if (orders.Count == 0)
        {
            return [];
        }

        var tableIds = orders.Select(o => o.TableId).Distinct().ToList();
        var tables = new Dictionary<Guid, TableViewModelResponse>();
        foreach (var tableId in tableIds)
        {
            var table = await _tableService.GetViewModelAsync(tableId, cancellationToken);
            if (table is not null)
            {
                tables[tableId] = table;
            }
        }

        var articleIds = orders.SelectMany(o => o.Tickets).SelectMany(t => t.Details).Select(d => d.ArticleId).Distinct().ToList();
        var articles = new Dictionary<Guid, ArticleViewModelResponse>();
        foreach (var articleId in articleIds)
        {
            var article = await _articleService.GetViewModelAsync(articleId, cancellationToken);
            if (article is not null)
            {
                articles[articleId] = article;
            }
        }

        return orders.Select(order => new OrderResponse
        {
            Id = order.Id,
            Notes = order.Notes,
            CreatedAt = order.CreatedUtc,
            UpdatedAt = order.UpdatedUtc,
            UpdatedBy = Guid.Empty,
            IsDeleted = order.Status == ApiOrderStatus.Deleted,
            Table = tables.GetValueOrDefault(order.TableId),
            Tickets = order.Tickets.OrderBy(t => t.TicketNumber).Select(ticket => new OrderTicketResponse
            {
                TicketId = ticket.TicketNumber,
                Details = ticket.Details.Select(detail => new OrderDetailResponse
                {
                    Id = detail.Id,
                    Notes = detail.Notes,
                    CreatedAt = detail.CreatedUtc,
                    UpdatedAt = detail.UpdatedUtc,
                    UpdatedBy = Guid.Empty,
                    IsDeleted = false,
                    Designation = detail.Designation,
                    Article = articles.GetValueOrDefault(detail.ArticleId),
                    Quantity = detail.Quantity,
                    Price = detail.Price,
                    Discount = detail.Discount,
                    Vat = detail.Vat,
                    TotalFinal = ComputeTotalFinal(detail.Quantity, detail.Price, detail.Discount, detail.Vat)
                }).ToList()
            }).ToList()
        }).ToList();
    }

    private static decimal ComputeTotalFinal(decimal quantity, decimal price, decimal discountPercent, decimal vatPercent)
    {
        var afterDiscount = quantity * price * (1 - discountPercent / 100m);
        return afterDiscount * (1 + vatPercent / 100m);
    }
}
