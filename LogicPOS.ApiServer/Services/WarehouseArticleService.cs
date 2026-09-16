using LogicPOS.ApiServer.Data;
using LogicPOS.ApiServer.Data.Entities;
using LogicPOS.ApiServer.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.ApiServer.Services;

public sealed class WarehouseArticleService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ArticleService _articleService;
    private readonly WarehouseService _warehouseService;

    public WarehouseArticleService(ApplicationDbContext dbContext, ArticleService articleService, WarehouseService warehouseService)
    {
        _dbContext = dbContext;
        _articleService = articleService;
        _warehouseService = warehouseService;
    }

    public async Task<WarehouseArticlesPaginatedResponse> GetPagedAsync(int page, int pageSize, Guid? articleId, CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 25 : pageSize;

        var query = _dbContext.ApiWarehouseArticles.AsNoTracking().Where(item => !item.IsDeleted);
        if (articleId.HasValue)
        {
            query = query.Where(item => item.ArticleId == articleId.Value);
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)pageSize);

        var items = await query.OrderBy(item => item.CreatedUtc).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        var viewModels = await MapToViewModelsAsync(items, cancellationToken);

        return new WarehouseArticlesPaginatedResponse
        {
            Items = viewModels,
            ItemsCount = viewModels.Count,
            TotalItems = totalItems,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages
        };
    }

    public async Task<WarehouseArticleResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.ApiWarehouseArticles.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null)
        {
            return null;
        }

        var article = await _articleService.GetByIdAsync(item.ArticleId, cancellationToken);
        var location = await _warehouseService.GetLocationByIdInternalAsync(item.WarehouseLocationId, cancellationToken);

        return new WarehouseArticleResponse
        {
            Id = item.Id,
            Notes = item.Notes,
            CreatedAt = item.CreatedUtc,
            UpdatedAt = item.UpdatedUtc,
            UpdatedBy = Guid.Empty,
            IsDeleted = item.IsDeleted,
            Article = article,
            ArticleId = item.ArticleId,
            SerialNumber = item.SerialNumber,
            Status = (int)item.Status,
            WarehouseLocation = location,
            WarehouseLocationId = item.WarehouseLocationId,
            Quantity = item.Quantity
        };
    }

    public async Task<IReadOnlyList<TotalStockResponse>> GetTotalsAsync(IReadOnlyList<Guid> articleIds, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.ApiWarehouseArticles.AsNoTracking().Where(item => !item.IsDeleted);
        if (articleIds.Count > 0)
        {
            query = query.Where(item => articleIds.Contains(item.ArticleId));
        }

        return await query
            .GroupBy(item => item.ArticleId)
            .Select(group => new TotalStockResponse { ArticleId = group.Key, Quantity = group.Sum(item => item.Quantity) })
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ApiWarehouseArticles.AnyAsync(item => item.Id == id, cancellationToken);
    }

    public async Task<(bool Success, string? Error)> ChangeLocationAsync(Guid warehouseArticleId, ChangeArticleLocationRequest request, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.ApiWarehouseArticles.SingleOrDefaultAsync(x => x.Id == warehouseArticleId, cancellationToken);
        if (item is null)
        {
            return (false, "WarehouseArticle não encontrado.");
        }

        if (!await _warehouseService.LocationExistsAsync(request.LocationId, cancellationToken))
        {
            return (false, "LocationId não existe.");
        }

        item.WarehouseLocationId = request.LocationId;
        item.Quantity = request.Quantity;
        item.UpdatedUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.ApiWarehouseArticles.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null)
        {
            return false;
        }

        _dbContext.ApiWarehouseArticles.Remove(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <summary>
    /// Adds incoming stock (merging into an existing row at the same location/serial number when one exists).
    /// Used by StockMovementService when a purchase-in movement is posted.
    /// </summary>
    public Task ReceiveStockAsync(Guid articleId, Guid warehouseLocationId, decimal quantity, string? serialNumber, CancellationToken cancellationToken = default)
    {
        return AdjustStockAsync(articleId, warehouseLocationId, quantity, serialNumber, cancellationToken);
    }

    /// <summary>
    /// Applies a signed quantity delta to the matching stock row (creating one for a positive delta with no
    /// existing match, or removing the row once it reaches zero for a negative one). Used both to receive
    /// stock (positive delta) and to reverse/correct it when a stock movement is deleted or edited (negative
    /// or arbitrary delta) — without this, deleting or re-quantifying a posted movement leaves the warehouse's
    /// on-hand quantity permanently out of sync with the movements that produced it.
    /// </summary>
    public async Task AdjustStockAsync(Guid articleId, Guid warehouseLocationId, decimal quantityDelta, string? serialNumber, CancellationToken cancellationToken = default)
    {
        var normalizedSerialNumber = string.IsNullOrWhiteSpace(serialNumber) ? null : serialNumber;

        var existing = await _dbContext.ApiWarehouseArticles.SingleOrDefaultAsync(
            item => item.ArticleId == articleId && item.WarehouseLocationId == warehouseLocationId && item.SerialNumber == normalizedSerialNumber && !item.IsDeleted,
            cancellationToken);

        if (existing is not null)
        {
            existing.Quantity += quantityDelta;
            existing.UpdatedUtc = DateTime.UtcNow;
            if (existing.Quantity <= 0)
            {
                _dbContext.ApiWarehouseArticles.Remove(existing);
            }

            return;
        }

        if (quantityDelta <= 0)
        {
            // Nothing to reverse against (already adjusted or removed elsewhere) — avoid creating a phantom
            // negative-quantity row.
            return;
        }

        _dbContext.ApiWarehouseArticles.Add(new ApiWarehouseArticle
        {
            Id = Guid.NewGuid(),
            ArticleId = articleId,
            SerialNumber = normalizedSerialNumber,
            Status = ApiArticleSerialNumberStatus.Available,
            WarehouseLocationId = warehouseLocationId,
            Quantity = quantityDelta,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
        });
    }

    private async Task<List<WarehouseArticleViewModelResponse>> MapToViewModelsAsync(List<ApiWarehouseArticle> items, CancellationToken cancellationToken)
    {
        if (items.Count == 0)
        {
            return [];
        }

        var articleIds = items.Select(item => item.ArticleId).Distinct().ToList();
        var articles = await _dbContext.ApiArticles.AsNoTracking().Where(item => articleIds.Contains(item.Id)).ToDictionaryAsync(item => item.Id, cancellationToken);

        var locationIds = items.Select(item => item.WarehouseLocationId).Distinct().ToList();
        var locations = await _dbContext.ApiWarehouseLocations.AsNoTracking().Where(item => locationIds.Contains(item.Id)).ToDictionaryAsync(item => item.Id, cancellationToken);

        var warehouseIds = locations.Values.Select(item => item.WarehouseId).Distinct().ToList();
        var warehouses = await _dbContext.ApiWarehouses.AsNoTracking().Where(item => warehouseIds.Contains(item.Id)).ToDictionaryAsync(item => item.Id, cancellationToken);

        return items.Select(item =>
        {
            var article = articles.GetValueOrDefault(item.ArticleId);
            var location = locations.GetValueOrDefault(item.WarehouseLocationId);
            var warehouse = location is not null ? warehouses.GetValueOrDefault(location.WarehouseId) : null;

            return new WarehouseArticleViewModelResponse
            {
                Id = item.Id,
                Notes = item.Notes,
                CreatedAt = item.CreatedUtc,
                UpdatedAt = item.UpdatedUtc,
                UpdatedBy = Guid.Empty,
                IsDeleted = item.IsDeleted,
                WarehouseId = warehouse?.Id ?? Guid.Empty,
                Warehouse = warehouse?.Designation ?? string.Empty,
                LocationId = item.WarehouseLocationId,
                Location = location?.Designation ?? string.Empty,
                Article = article?.Designation ?? string.Empty,
                SerialNumber = item.SerialNumber,
                Quantity = item.Quantity,
                ArticleId = item.ArticleId
            };
        }).ToList();
    }
}
