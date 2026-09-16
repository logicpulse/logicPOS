using LogicPOS.ApiServer.Data;
using LogicPOS.ApiServer.Data.Entities;
using LogicPOS.ApiServer.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.ApiServer.Services;

public sealed class TableService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly PlaceService _placeService;

    public TableService(ApplicationDbContext dbContext, PlaceService placeService)
    {
        _dbContext = dbContext;
        _placeService = placeService;
    }

    public async Task<IReadOnlyList<TableViewModelResponse>> GetAllAsync(ApiTableStatus? status, Guid? placeId, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.ApiTables.AsNoTracking().Where(item => !item.IsDeleted);
        if (status.HasValue)
        {
            query = query.Where(item => item.Status == status.Value);
        }

        if (placeId.HasValue)
        {
            query = query.Where(item => item.PlaceId == placeId.Value);
        }

        var tables = await query.OrderBy(item => item.Order).ToListAsync(cancellationToken);
        if (tables.Count == 0)
        {
            return [];
        }

        var places = await _dbContext.ApiPlaces
            .AsNoTracking()
            .Where(item => tables.Select(table => table.PlaceId).Contains(item.Id))
            .ToDictionaryAsync(item => item.Id, cancellationToken);

        return tables.Select(table => MapViewModel(table, places.GetValueOrDefault(table.PlaceId))).ToList();
    }

    public async Task<TableResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var table = await _dbContext.ApiTables.AsNoTracking().SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (table is null)
        {
            return null;
        }

        var place = await _placeService.GetByIdInternalAsync(table.PlaceId, cancellationToken);
        return Map(table, place);
    }

    public async Task<TableViewModelResponse?> GetViewModelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var table = await _dbContext.ApiTables.AsNoTracking().SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (table is null)
        {
            return null;
        }

        var place = await _dbContext.ApiPlaces.AsNoTracking().SingleOrDefaultAsync(item => item.Id == table.PlaceId, cancellationToken);
        return MapViewModel(table, place);
    }

    public async Task<TableViewModelResponse?> GetDefaultAsync(CancellationToken cancellationToken = default)
    {
        var table = await _dbContext.ApiTables
            .AsNoTracking()
            .Where(item => !item.IsDeleted)
            .OrderBy(item => item.Order)
            .FirstOrDefaultAsync(cancellationToken);

        if (table is null)
        {
            return null;
        }

        var place = await _dbContext.ApiPlaces.AsNoTracking().SingleOrDefaultAsync(item => item.Id == table.PlaceId, cancellationToken);
        return MapViewModel(table, place);
    }

    public async Task<decimal?> GetTotalAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var exists = await _dbContext.ApiTables.AnyAsync(item => item.Id == id, cancellationToken);
        if (!exists)
        {
            return null;
        }

        // Sum of Quantity * Price * (1 - Discount/100) * (1 + Vat/100) across every open order's line items
        // for this table (queried directly, not via OrderService, to avoid a TableService <-> OrderService cycle).
        var lines = await _dbContext.ApiOrders
            .Where(order => order.TableId == id && order.Status == ApiOrderStatus.Open)
            .SelectMany(order => order.Tickets)
            .SelectMany(ticket => ticket.Details)
            .Select(detail => new { detail.Quantity, detail.Price, detail.Discount, detail.Vat })
            .ToListAsync(cancellationToken);

        return lines.Sum(line => line.Quantity * line.Price * (1 - line.Discount / 100m) * (1 + line.Vat / 100m));
    }

    public async Task<(bool Success, string? Error, AddEntityIdResponse? Response)> CreateAsync(AddTableRequest request, CancellationToken cancellationToken = default)
    {
        if (!await _placeService.ExistsAsync(request.PlaceId, cancellationToken))
        {
            return (false, "PlaceId não existe.", null);
        }

        var count = await _dbContext.ApiTables.CountAsync(cancellationToken);
        var table = new ApiTable
        {
            Id = Guid.NewGuid(),
            Order = (uint)(count + 1),
            Code = $"T{count + 1}",
            Designation = request.Designation.Trim(),
            PlaceId = request.PlaceId,
            Status = ApiTableStatus.Free,
            Notes = request.Notes ?? string.Empty,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
        };

        _dbContext.ApiTables.Add(table);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return (true, null, new AddEntityIdResponse { Id = table.Id });
    }

    public async Task<(bool Success, string? Error)> UpdateAsync(Guid id, UpdateTableRequest request, CancellationToken cancellationToken = default)
    {
        var table = await _dbContext.ApiTables.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (table is null)
        {
            return (false, "Table não encontrada.");
        }

        if (!await _placeService.ExistsAsync(request.PlaceId, cancellationToken))
        {
            return (false, "PlaceId não existe.");
        }

        table.Order = request.Order;
        table.Code = string.IsNullOrWhiteSpace(request.Code) ? table.Code : request.Code;
        table.Designation = request.Designation.Trim();
        table.PlaceId = request.PlaceId;
        table.Notes = request.Notes ?? string.Empty;
        table.IsDeleted = request.IsDeleted;
        table.UpdatedUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<bool> FreeAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var table = await _dbContext.ApiTables.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (table is null)
        {
            return false;
        }

        table.Status = ApiTableStatus.Free;
        table.OpennedAtUtc = null;
        table.UpdatedUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ReserveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var table = await _dbContext.ApiTables.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (table is null)
        {
            return false;
        }

        table.Status = ApiTableStatus.Reserved;
        table.UpdatedUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var table = await _dbContext.ApiTables.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (table is null)
        {
            return false;
        }

        _dbContext.ApiTables.Remove(table);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static TableResponse Map(ApiTable table, PlaceResponse? place)
    {
        return new TableResponse
        {
            Id = table.Id,
            Notes = table.Notes,
            CreatedAt = table.CreatedUtc,
            UpdatedAt = table.UpdatedUtc,
            UpdatedBy = Guid.Empty,
            IsDeleted = table.IsDeleted,
            Order = table.Order,
            Code = table.Code,
            Designation = table.Designation,
            Place = place,
            PlaceId = table.PlaceId,
            ButtonImage = table.ButtonImage,
            Status = (int)table.Status,
            OpennedAt = table.OpennedAtUtc
        };
    }

    private static TableViewModelResponse MapViewModel(ApiTable table, ApiPlace? place)
    {
        return new TableViewModelResponse
        {
            Id = table.Id,
            Notes = table.Notes,
            CreatedAt = table.CreatedUtc,
            UpdatedAt = table.UpdatedUtc,
            UpdatedBy = Guid.Empty,
            IsDeleted = table.IsDeleted,
            Code = table.Code,
            Place = place?.Designation ?? string.Empty,
            PriceTypeEnum = 0,
            Designation = table.Designation,
            Status = (int)table.Status,
            OpennedAt = table.OpennedAtUtc,
            PlaceId = table.PlaceId
        };
    }
}
