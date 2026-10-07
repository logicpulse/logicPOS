using LogicPOS.Core;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.POS.WorkSessions.Movements.Common;
using LogicPOS.Domain.Enums;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.FrontOffice;

public sealed class PosStoredLine
{
    public PosStoredLine(Guid articleId, decimal quantity, decimal netUnitPrice, string? ticketKey = null)
    {
        ArticleId = articleId;
        Quantity = quantity;
        NetUnitPrice = netUnitPrice;
        TicketKey = ticketKey;
    }

    public string? TicketKey { get; }

    public Guid ArticleId { get; }

    public decimal Quantity { get; }

    public decimal NetUnitPrice { get; }
}

public sealed class PosOpenOrder
{
    public PosOpenOrder(Guid id, string label)
    {
        Id = id;
        Label = label;
    }

    public Guid Id { get; }

    public string Label { get; }
}

public enum PosTableState
{
    Free,
    Open,
    Reserved
}

public sealed class PosTableSlot
{
    public PosTableSlot(Guid id, string name, PosTableState state, Guid? orderId, Guid placeId, string placeName, DateTime? openedAt = null, decimal total = 0)
    {
        Id = id;
        Name = name;
        State = state;
        OrderId = orderId;
        PlaceId = placeId;
        PlaceName = placeName;
        OpenedAt = openedAt;
        Total = total;
    }

    public DateTime? OpenedAt { get; }

    public decimal Total { get; }

    public Guid Id { get; }

    public string Name { get; }

    public PosTableState State { get; }

    public Guid? OrderId { get; }

    public Guid PlaceId { get; }

    public string PlaceName { get; }
}

public sealed class PosOrderService : IPosOrderService
{
    private readonly IServiceScopeFactory _scopes;

    public PosOrderService(IServiceScopeFactory scopes)
    {
        _scopes = scopes;
    }

    public async Task<PosWorkSessionState> GetWorkSessionAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var dayOpen = await database.WorkSessionPeriods.AnyAsync(
            item => item.Type == WorkSessionPeriodType.Day && item.Status == WorkSessionPeriodStatus.Open,
            cancellationToken);
        var hardwareId = MachineIdentity.HardwareId;
        var terminalId = await database.Terminals
            .AsNoTracking()
            .Where(item => item.IsDeleted == false && item.HardwareId == hardwareId)
            .Select(item => item.Id)
            .FirstOrDefaultAsync(cancellationToken);
        var terminalOpen = terminalId != Guid.Empty && await database.WorkSessionPeriods.AnyAsync(
            item => item.Type == WorkSessionPeriodType.Terminal
                && item.Status == WorkSessionPeriodStatus.Open
                && item.CreatedWhere == terminalId,
            cancellationToken);
        return new PosWorkSessionState { DayOpen = dayOpen, TerminalOpen = terminalOpen };
    }

    public async Task<PosDocumentResult> SaveAsync(
        Guid? orderId,
        IReadOnlyList<PosStoredLine> lines,
        Guid? tableId = null,
        CancellationToken cancellationToken = default)
    {
        if (lines.Count == 0)
        {
            return PosDocumentResult.Fail("O ticket não tem artigos.");
        }

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var details = lines.Select(line => new CreateOrderDetailDto(line.ArticleId, line.Quantity, line.NetUnitPrice)).ToList();

        if (orderId is Guid existingId)
        {
            var existing = await database.Orders
                .Include(order => order.Tickets)
                .FirstOrDefaultAsync(order => order.Id == existingId && order.IsDeleted == false, cancellationToken);
            if (existing is null)
            {
                return PosDocumentResult.Fail("A ordem já não existe.");
            }

            var added = await existing.AddTicketAsync(
                new CreateTicketDto(existing.Id, details),
                scope.ServiceProvider.GetRequiredService<ITicketRepository>(),
                scope.ServiceProvider.GetRequiredService<IArticleRepository>(),
                cancellationToken);
            if (added.IsFailure)
            {
                return PosDocumentResult.Fail(added.Error.Message);
            }

            await database.Tickets.AddAsync(added.Value!, cancellationToken);
            if (added.Value!.Details is not null)
            {
                await database.OrderDetails.AddRangeAsync(added.Value.Details, cancellationToken);
            }

            await database.SaveChangesAsync(cancellationToken);
            return PosDocumentResult.Ok(existing.CreatedAt.ToString("HH:mm"));
        }

        var tables = database.Tables.AsNoTracking().Where(item => item.IsDeleted == false);
        var table = tableId is Guid chosen
            ? await tables.FirstOrDefaultAsync(item => item.Id == chosen, cancellationToken)
            : await tables.OrderBy(item => item.Designation).FirstOrDefaultAsync(cancellationToken);
        if (table is null)
        {
            return PosDocumentResult.Fail("Não existe uma mesa para gravar o pedido.");
        }

        var references = new OrderReferences(
            scope.ServiceProvider.GetRequiredService<IArticleRepository>(),
            scope.ServiceProvider.GetRequiredService<IVatExemptionReasonRepository>(),
            scope.ServiceProvider.GetRequiredService<IOrderRepository>(),
            scope.ServiceProvider.GetRequiredService<ITableRepository>(),
            scope.ServiceProvider.GetRequiredService<ITicketRepository>());

        var created = await Order.CreateAsync(
            new CreateOrderDto(table.Id, new[] { new CreateTicketDto(null, details) }),
            references,
            cancellationToken);
        if (created.IsFailure)
        {
            return PosDocumentResult.Fail(created.Error.Message);
        }

        var order = created.Value!;
        await database.Orders.AddAsync(order, cancellationToken);
        if (order.Tickets is not null)
        {
            await database.Tickets.AddRangeAsync(order.Tickets, cancellationToken);
            foreach (var ticket in order.Tickets)
            {
                if (ticket.Details is not null)
                {
                    await database.OrderDetails.AddRangeAsync(ticket.Details, cancellationToken);
                }
            }
        }

        await database.SaveChangesAsync(cancellationToken);
        return PosDocumentResult.Ok(order.CreatedAt.ToString("HH:mm"));
    }

    public async Task<IReadOnlyList<PosOpenOrder>> ListOpenAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var orders = await database.Orders
            .AsNoTracking()
            .Include(order => order.Table)
            .Include(order => order.Tickets!)
            .ThenInclude(ticket => ticket.Details)
            .Where(order => order.IsDeleted == false && order.OrderStatus == OrderStatus.Open)
            .OrderByDescending(order => order.CreatedAt)
            .ToListAsync(cancellationToken);

        return orders.Select(order =>
        {
            var total = order.Tickets?.SelectMany(ticket => ticket.Details ?? Array.Empty<OrderDetail>()).Sum(detail => detail.TotalFinal) ?? 0m;
            var table = string.IsNullOrWhiteSpace(order.Table?.Designation) ? "Mesa" : order.Table!.Designation;
            return new PosOpenOrder(order.Id, $"{table}   {order.CreatedAt:dd/MM HH:mm}   {total:C}");
        }).ToList();
    }

    public async Task<IReadOnlyList<PosTableSlot>> ListTablesAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var tables = await database.Tables
            .AsNoTracking()
            .Include(item => item.Place)
            .Where(item => item.IsDeleted == false)
            .OrderBy(item => item.Designation)
            .ToListAsync(cancellationToken);
        var openOrders = await database.Orders
            .AsNoTracking()
            .Include(order => order.Tickets!)
            .ThenInclude(ticket => ticket.Details)
            .Where(order => order.IsDeleted == false && order.OrderStatus == OrderStatus.Open)
            .ToListAsync(cancellationToken);
        var orderByTable = openOrders
            .GroupBy(order => order.TableId)
            .ToDictionary(group => group.Key, group => group.OrderByDescending(order => order.CreatedAt).First());

        return tables.Select(table =>
        {
            orderByTable.TryGetValue(table.Id, out var order);
            var orderId = order?.Id ?? Guid.Empty;
            var total = order?.Tickets?.SelectMany(ticket => ticket.Details ?? Array.Empty<OrderDetail>())
                .Where(detail => detail.IsDeleted == false)
                .Sum(detail => detail.TotalFinal) ?? 0m;
            var state = table.Status switch
            {
                TableStatus.Open => PosTableState.Open,
                TableStatus.Reserved => PosTableState.Reserved,
                _ => PosTableState.Free
            };
            if (orderId != Guid.Empty && state == PosTableState.Free)
            {
                state = PosTableState.Open;
            }

            var placeName = string.IsNullOrWhiteSpace(table.Place?.Designation) ? "Zona" : table.Place!.Designation;
            return new PosTableSlot(table.Id, table.Designation ?? table.Code ?? "Mesa", state, orderId == Guid.Empty ? null : orderId, table.PlaceId, placeName, order?.CreatedAt, total);
        }).ToList();
    }

    public async Task<Guid?> GetDefaultTableIdAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var tableId = await database.Tables
            .AsNoTracking()
            .Where(item => item.IsDeleted == false)
            .OrderBy(item => item.Designation)
            .Select(item => item.Id)
            .FirstOrDefaultAsync(cancellationToken);
        return tableId == Guid.Empty ? null : tableId;
    }

    public async Task<PosDocumentResult> ReserveTableAsync(Guid tableId, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var table = await database.Tables.FirstOrDefaultAsync(item => item.Id == tableId && item.IsDeleted == false, cancellationToken);
        if (table is null)
        {
            return PosDocumentResult.Fail("A mesa já não existe.");
        }

        var reserved = table.Reserve();
        if (reserved.IsFailure)
        {
            return PosDocumentResult.Fail("A mesa não está livre.");
        }

        await database.SaveChangesAsync(cancellationToken);
        return PosDocumentResult.Ok(string.Empty);
    }

    public async Task<PosDocumentResult> FreeTableAsync(Guid tableId, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var table = await database.Tables.FirstOrDefaultAsync(item => item.Id == tableId && item.IsDeleted == false, cancellationToken);
        if (table is null)
        {
            return PosDocumentResult.Fail("A mesa já não existe.");
        }

        if (table.Status == TableStatus.Reserved)
        {
            table.Free();
        }
        else if (table.Status == TableStatus.Open)
        {
            // Open tables can only be released when no open order is left (orphan lock)
            var hasOrder = await database.Orders.AnyAsync(
                order => order.TableId == tableId && order.IsDeleted == false && order.OrderStatus == OrderStatus.Open,
                cancellationToken);
            if (hasOrder)
            {
                return PosDocumentResult.Fail("A mesa tem um pedido aberto.");
            }

            table.Close();
        }
        else
        {
            return PosDocumentResult.Fail("A mesa já está livre.");
        }

        await database.SaveChangesAsync(cancellationToken);
        return PosDocumentResult.Ok(string.Empty);
    }

    public async Task<IReadOnlyList<PosStoredLine>> ReadLinesAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var details = await database.OrderDetails
            .AsNoTracking()
            .Where(detail => detail.IsDeleted == false && detail.Ticket != null && detail.Ticket.OrderId == orderId)
            .ToListAsync(cancellationToken);

        return details.Select(detail => new PosStoredLine(detail.ArticleId, detail.Quantity, detail.Price, detail.TicketId.ToString())).ToList();
    }

    public async Task<PosDocumentResult> ReduceLineAsync(Guid orderId, Guid articleId, decimal unitPrice, decimal quantity, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var order = await database.Orders
            .Include(item => item.Tickets!)
            .ThenInclude(ticket => ticket.Details)
            .FirstOrDefaultAsync(item => item.Id == orderId && item.IsDeleted == false, cancellationToken);
        if (order is null)
        {
            return PosDocumentResult.Fail("Ordem não encontrada.");
        }

        var remaining = quantity;
        foreach (var ticket in order.Tickets!.ToList())
        {
            foreach (var detail in ticket.Details!.Where(item => item.IsDeleted == false && item.ArticleId == articleId && item.Price == unitPrice).ToList())
            {
                if (remaining <= 0)
                {
                    break;
                }

                var taken = Math.Min(remaining, detail.Quantity);
                detail.Quantity -= taken;
                remaining -= taken;
                if (detail.Quantity <= 0)
                {
                    ticket.Details!.Remove(detail);
                }
            }

            if (ticket.Details!.Count == 0)
            {
                order.Tickets!.Remove(ticket);
            }
        }

        await database.SaveChangesAsync(cancellationToken);
        return PosDocumentResult.Ok(string.Empty);
    }

    public async Task<PosDocumentResult> ChangeTableAsync(Guid orderId, Guid newTableId, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var order = await database.Orders
            .Include(item => item.Table)
            .FirstOrDefaultAsync(item => item.Id == orderId && item.IsDeleted == false, cancellationToken);
        var table = await database.Tables.FirstOrDefaultAsync(item => item.Id == newTableId && item.IsDeleted == false, cancellationToken);
        if (order is null)
        {
            return PosDocumentResult.Fail("Ordem não encontrada.");
        }

        if (table is null)
        {
            return PosDocumentResult.Fail("Mesa não encontrada.");
        }

        if (table.Status != TableStatus.Free)
        {
            return PosDocumentResult.Fail("A mesa de destino não está livre.");
        }

        table.Status = TableStatus.Open;
        table.OpennedAt = DateTime.Now;
        order.Table?.Close();
        order.TableId = newTableId;
        await database.SaveChangesAsync(cancellationToken);
        return PosDocumentResult.Ok(string.Empty);
    }

    public async Task<PosDocumentResult> MoveLineAsync(Guid orderId, Guid destinationTableId, Guid articleId, decimal unitPrice, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var order = await database.Orders
            .Include(item => item.Tickets!)
            .ThenInclude(ticket => ticket.Details)
            .FirstOrDefaultAsync(item => item.Id == orderId && item.IsDeleted == false, cancellationToken);
        if (order is null)
        {
            return PosDocumentResult.Fail("Ordem não encontrada.");
        }

        var destination = await database.Orders
            .Include(item => item.Tickets!)
            .ThenInclude(ticket => ticket.Details)
            .Include(item => item.Table)
            .FirstOrDefaultAsync(item => item.TableId == destinationTableId && item.OrderStatus != OrderStatus.Closed && item.IsDeleted == false, cancellationToken);
        var created = destination is null;
        if (destination is null)
        {
            var table = await database.Tables.FirstOrDefaultAsync(item => item.Id == destinationTableId && item.IsDeleted == false, cancellationToken);
            if (table is null)
            {
                return PosDocumentResult.Fail("Mesa não encontrada.");
            }

            if (table.Status == TableStatus.Reserved)
            {
                return PosDocumentResult.Fail("Mesa reservada.");
            }

            table.Status = TableStatus.Open;
            table.OpennedAt ??= DateTime.Now;
            destination = new Order
            {
                TableId = destinationTableId,
                Tickets = []
            };
        }

        OrderDetail? detail = null;
        Ticket? sourceTicket = null;
        foreach (var ticket in order.Tickets ?? [])
        {
            detail = ticket.Details?.FirstOrDefault(item => item.ArticleId == articleId
                && Math.Round(item.Price, 2, MidpointRounding.AwayFromZero) == Math.Round(unitPrice, 2, MidpointRounding.AwayFromZero));
            if (detail is not null)
            {
                sourceTicket = ticket;
                break;
            }
        }

        if (detail is null || sourceTicket is null)
        {
            return PosDocumentResult.Fail("Linha não encontrada na ordem.");
        }

        sourceTicket.Details!.Remove(detail);
        destination.Tickets ??= new List<Ticket>();
        var moved = new Ticket
        {
            Details = new List<OrderDetail> { detail },
            TicketId = created
                ? 1
                : destination.Tickets.Where(item => item.IsDeleted == false).Select(item => item.TicketId).DefaultIfEmpty(0).Max() + 1
        };
        detail.Ticket = moved;
        destination.Tickets.Add(moved);
        if (created)
        {
            database.Orders.Add(destination);
        }
        else
        {
            database.Tickets.Add(moved);
        }

        await database.SaveChangesAsync(cancellationToken);
        return PosDocumentResult.Ok(string.Empty);
    }

    public async Task<PosDocumentResult> SplitAsync(Guid orderId, int splitters, IReadOnlyList<PosStoredLine> lines, CancellationToken cancellationToken = default)
    {
        if (splitters < 2)
        {
            return PosDocumentResult.Fail("A conta tem de ser dividida por pelo menos duas pessoas.");
        }

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var order = await database.Orders
            .Include(item => item.Tickets!)
            .ThenInclude(ticket => ticket.Details)
            .FirstOrDefaultAsync(item => item.Id == orderId && item.IsDeleted == false, cancellationToken);
        if (order is null)
        {
            return PosDocumentResult.Fail("Ordem não encontrada.");
        }

        var articles = lines.Select(line => line.ArticleId).ToHashSet();
        foreach (var detail in order.Tickets!.SelectMany(ticket => ticket.Details ?? []).Where(detail => articles.Contains(detail.ArticleId)))
        {
            detail.Quantity /= splitters;
            detail.TotalTax /= splitters;
            detail.TotalFinal /= splitters;
            detail.TotalDiscount /= splitters;
        }

        await database.SaveChangesAsync(cancellationToken);
        return PosDocumentResult.Ok(string.Empty);
    }

    public async Task<PosCashReport?> GetTerminalCashReportAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var terminalId = await CurrentTerminalIdAsync(database, cancellationToken);
        if (terminalId == Guid.Empty)
        {
            return null;
        }

        var period = await database.WorkSessionPeriods
            .AsNoTracking()
            .Where(item => item.IsDeleted == false
                && item.Type == WorkSessionPeriodType.Terminal
                && item.CreatedWhere == terminalId)
            .OrderByDescending(item => item.StartDate)
            .FirstOrDefaultAsync(cancellationToken);
        return period is null ? null : await BuildCashReportAsync(database, period, day: false, cancellationToken);
    }

    public async Task<PosCashReport?> GetDayCashReportAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var period = await database.WorkSessionPeriods
            .AsNoTracking()
            .Where(item => item.IsDeleted == false && item.Type == WorkSessionPeriodType.Day)
            .OrderByDescending(item => item.StartDate)
            .FirstOrDefaultAsync(cancellationToken);
        return period is null ? null : await BuildCashReportAsync(database, period, day: true, cancellationToken);
    }

    public async Task CloseAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var order = await database.Orders.FirstOrDefaultAsync(item => item.Id == orderId && item.IsDeleted == false, cancellationToken);
        if (order is null)
        {
            return;
        }

        order.OrderStatus = OrderStatus.Closed;
        await database.SaveChangesAsync(cancellationToken);
    }

    private static async Task<Guid> CurrentTerminalIdAsync(LogicPOSDbContext database, CancellationToken cancellationToken)
    {
        var hardwareId = MachineIdentity.HardwareId;
        return await database.Terminals
            .AsNoTracking()
            .Where(item => item.IsDeleted == false && item.HardwareId == hardwareId)
            .Select(item => item.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private static async Task<PosCashReport> BuildCashReportAsync(
        LogicPOSDbContext database,
        WorkSessionPeriod period,
        bool day,
        CancellationToken cancellationToken)
    {
        var periodIds = day
            ? await database.WorkSessionPeriods
                .AsNoTracking()
                .Where(item => item.IsDeleted == false && (item.Id == period.Id || item.ParentId == period.Id))
                .Select(item => item.Id)
                .ToListAsync(cancellationToken)
            : new List<Guid> { period.Id };

        var movements = await database.WorkSessionMovements
            .AsNoTracking()
            .Where(item => periodIds.Contains(item.PeriodId))
            .Select(item => new { item.Type, item.Amount, item.DocumentId })
            .ToListAsync(cancellationToken);

        var opening = movements.Where(item => item.Type == WorkSessionMovementType.CashDrawerOpen).Sum(item => item.Amount);
        var cashIn = movements.Where(item => item.Type == WorkSessionMovementType.CashDrawerIn).Sum(item => item.Amount);
        var cashOut = movements.Where(item => item.Type == WorkSessionMovementType.CashDrawerOut).Sum(item => item.Amount);
        var closed = movements.Where(item => item.Type == WorkSessionMovementType.CashDrawerClose).Sum(item => item.Amount);
        var documentIds = movements
            .Where(item => item.DocumentId != null)
            .Select(item => item.DocumentId!.Value)
            .Distinct()
            .ToList();

        IReadOnlyList<PosCashReportLine> families = [];
        if (documentIds.Count > 0)
        {
            families = await database.DocumentDetails
                .AsNoTracking()
                .Where(detail => documentIds.Contains(detail.DocumentId))
                .GroupBy(detail => detail.Article!.Subfamily!.Family!.Designation)
                .Select(group => new PosCashReportLine
                {
                    Name = group.Key,
                    Quantity = group.Sum(item => item.Quantity),
                    Total = group.Sum(item => item.TotalFinal)
                })
                .ToListAsync(cancellationToken);
        }

        var open = period.Status == WorkSessionPeriodStatus.Open;
        return new PosCashReport
        {
            Title = CashReportTitle(day, open),
            Subtitle = day ? null : period.Designation,
            OpenedAt = period.StartDate,
            ClosedAt = period.EndDate,
            OpeningCash = opening,
            CashIn = cashIn,
            CashOut = cashOut,
            ClosingCash = closed != 0 ? closed : opening + cashIn - cashOut,
            Families = families
        };
    }

    private static string CashReportTitle(bool day, bool open)
    {
        if (day)
        {
            return open ? "Resumo do dia" : "Fecho do dia";
        }

        return open ? "Resumo do terminal" : "Fecho do terminal";
    }
}
