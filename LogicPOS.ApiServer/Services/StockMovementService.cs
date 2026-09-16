using LogicPOS.ApiServer.Data;
using LogicPOS.ApiServer.Data.Entities;
using LogicPOS.ApiServer.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.ApiServer.Services;

public sealed class StockMovementService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly WarehouseArticleService _warehouseArticleService;
    private readonly WarehouseService _warehouseService;

    public StockMovementService(ApplicationDbContext dbContext, WarehouseArticleService warehouseArticleService, WarehouseService warehouseService)
    {
        _dbContext = dbContext;
        _warehouseArticleService = warehouseArticleService;
        _warehouseService = warehouseService;
    }

    public async Task<StockMovementsPaginatedResponse> GetPagedAsync(int page, int pageSize, Guid? articleId, CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 25 : pageSize;

        var itemsQuery = _dbContext.ApiStockMovementItems.AsNoTracking().AsQueryable();
        if (articleId.HasValue)
        {
            itemsQuery = itemsQuery.Where(item => item.ArticleId == articleId.Value);
        }

        var movementIds = await itemsQuery.Select(item => item.StockMovementId).Distinct().ToListAsync(cancellationToken);
        var movements = await _dbContext.ApiStockMovements.AsNoTracking()
            .Where(m => !m.IsDeleted && movementIds.Contains(m.Id))
            .OrderByDescending(m => m.Date)
            .ToListAsync(cancellationToken);

        var allItems = await _dbContext.ApiStockMovementItems.AsNoTracking()
            .Where(item => movementIds.Contains(item.StockMovementId) && (!articleId.HasValue || item.ArticleId == articleId.Value))
            .ToListAsync(cancellationToken);

        var articleIds = allItems.Select(item => item.ArticleId).Distinct().ToList();
        var articles = await _dbContext.ApiArticles.AsNoTracking().Where(item => articleIds.Contains(item.Id)).ToDictionaryAsync(item => item.Id, cancellationToken);

        var rows = movements
            .SelectMany(movement => allItems.Where(item => item.StockMovementId == movement.Id).Select(item => new StockMovementViewModelResponse
            {
                // The movement's id, not the line item's — GetById/Update operate on the movement header
                // (see GetStockMovementByIdQueryHandler / UpdateStockMovementCommandHandler client-side),
                // so a client reading this list and then calling GetById/Update on this "id" must land on
                // the same movement, even though this view flattens one row per line item.
                Id = movement.Id,
                Notes = movement.Notes,
                CreatedAt = movement.CreatedUtc,
                UpdatedAt = movement.UpdatedUtc,
                UpdatedBy = Guid.Empty,
                IsDeleted = movement.IsDeleted,
                Customer = null,
                CustomerId = null,
                Article = articles.GetValueOrDefault(item.ArticleId)?.Designation ?? string.Empty,
                ArticleId = item.ArticleId,
                SerialNumber = item.SerialNumber,
                DocumentNumber = movement.DocumentNumber,
                Quantity = item.Quantity,
                Price = item.Price,
                Date = movement.Date,
                HasExternalDocument = !string.IsNullOrWhiteSpace(movement.ExternalDocument)
            }))
            .ToList();

        var totalItems = rows.Count;
        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)pageSize);
        var paged = rows.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return new StockMovementsPaginatedResponse
        {
            Items = paged,
            ItemsCount = paged.Count,
            TotalItems = totalItems,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages
        };
    }

    public async Task<StockMovementResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var movement = await _dbContext.ApiStockMovements.AsNoTracking().SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (movement is null)
        {
            return null;
        }

        return new StockMovementResponse
        {
            Id = movement.Id,
            Notes = movement.Notes,
            CreatedAt = movement.CreatedUtc,
            UpdatedAt = movement.UpdatedUtc,
            UpdatedBy = Guid.Empty,
            IsDeleted = movement.IsDeleted,
            DocumentNumber = movement.DocumentNumber,
            ExternalDocument = movement.ExternalDocument
        };
    }

    public async Task<(bool Success, string? Field, string? Error)> CreateAsync(AddStockMovementRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Items.Count == 0)
        {
            return (false, "items", "Pelo menos um item é obrigatório.");
        }

        foreach (var item in request.Items)
        {
            if (!await _dbContext.ApiArticles.AnyAsync(a => a.Id == item.ArticleId, cancellationToken))
            {
                return (false, "items", $"ArticleId {item.ArticleId} não existe.");
            }

            if (item.WarehouseLocationId.HasValue && !await _warehouseService.LocationExistsAsync(item.WarehouseLocationId.Value, cancellationToken))
            {
                return (false, "items", $"WarehouseLocationId {item.WarehouseLocationId} não existe.");
            }
        }

        Guid? defaultLocationId = null;
        if (request.Items.Any(item => !item.WarehouseLocationId.HasValue))
        {
            defaultLocationId = await _dbContext.ApiWarehouseLocations.AsNoTracking()
                .Where(l => !l.IsDeleted)
                .OrderByDescending(l => l.IsDefault)
                .Select(l => (Guid?)l.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (defaultLocationId is null)
            {
                return (false, "items", "Nenhuma localização de armazém existe; crie um Warehouse com pelo menos uma localização primeiro, ou indique WarehouseLocationId em cada item.");
            }
        }

        var movement = new ApiStockMovement
        {
            Id = Guid.NewGuid(),
            SupplierId = request.SupplierId,
            Date = request.Date == default ? DateTime.UtcNow : request.Date,
            DocumentNumber = string.IsNullOrWhiteSpace(request.DocumentNumber) ? $"MOV-{DateTime.UtcNow:yyyyMMddHHmmss}" : request.DocumentNumber,
            ExternalDocument = request.ExternalDocument,
            Notes = request.Notes ?? string.Empty,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
        };

        _dbContext.ApiStockMovements.Add(movement);

        foreach (var item in request.Items)
        {
            var locationId = item.WarehouseLocationId ?? defaultLocationId!.Value;

            _dbContext.ApiStockMovementItems.Add(new ApiStockMovementItem
            {
                Id = Guid.NewGuid(),
                StockMovementId = movement.Id,
                ArticleId = item.ArticleId,
                Quantity = item.Quantity,
                SerialNumber = item.SerialNumber,
                WarehouseLocationId = locationId,
                Price = item.Price
            });

            await _warehouseArticleService.ReceiveStockAsync(item.ArticleId, locationId, item.Quantity, item.SerialNumber, cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return (true, null, null);
    }

    public async Task<(bool Success, string? Error)> UpdateAsync(Guid id, UpdateStockMovementRequest request, CancellationToken cancellationToken = default)
    {
        var movement = await _dbContext.ApiStockMovements.Include(m => m.Items).SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (movement is null)
        {
            return (false, "StockMovement não encontrado.");
        }

        if (request.SupplierId.HasValue) movement.SupplierId = request.SupplierId.Value;
        if (request.Date.HasValue) movement.Date = request.Date.Value;
        if (!string.IsNullOrWhiteSpace(request.DocumentNumber)) movement.DocumentNumber = request.DocumentNumber;
        movement.ExternalDocument = request.ExternalDocument ?? movement.ExternalDocument;
        movement.UpdatedUtc = DateTime.UtcNow;

        // Quantity/Price updates only make sense when the client isn't telling us which line to change;
        // this mirrors the client contract (UpdateStockMovementCommand has no line identifier), so it
        // only applies when the movement has exactly one item.
        if (movement.Items.Count == 1)
        {
            var singleItem = movement.Items[0];
            if (request.Quantity.HasValue && request.Quantity.Value != singleItem.Quantity && singleItem.WarehouseLocationId.HasValue)
            {
                var delta = request.Quantity.Value - singleItem.Quantity;
                await _warehouseArticleService.AdjustStockAsync(singleItem.ArticleId, singleItem.WarehouseLocationId.Value, delta, singleItem.SerialNumber, cancellationToken);
            }

            if (request.Quantity.HasValue) singleItem.Quantity = request.Quantity.Value;
            if (request.Price.HasValue) singleItem.Price = request.Price.Value;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var movement = await _dbContext.ApiStockMovements.Include(m => m.Items).SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (movement is null)
        {
            return false;
        }

        // Reverse the stock this movement originally added, so deleting a purchase doesn't leave the
        // warehouse permanently overstated.
        foreach (var item in movement.Items.Where(item => item.WarehouseLocationId.HasValue))
        {
            await _warehouseArticleService.AdjustStockAsync(item.ArticleId, item.WarehouseLocationId!.Value, -item.Quantity, item.SerialNumber, cancellationToken);
        }

        _dbContext.ApiStockMovements.Remove(movement);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<ArticleHistoriesPaginatedResponse> GetHistoriesAsync(int page, int pageSize, Guid? articleId, CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 25 : pageSize;

        var itemsQuery = _dbContext.ApiStockMovementItems.AsNoTracking().AsQueryable();
        if (articleId.HasValue)
        {
            itemsQuery = itemsQuery.Where(item => item.ArticleId == articleId.Value);
        }

        var items = await itemsQuery.ToListAsync(cancellationToken);
        var movementIds = items.Select(item => item.StockMovementId).Distinct().ToList();
        var movements = await _dbContext.ApiStockMovements.AsNoTracking().Where(m => movementIds.Contains(m.Id)).ToDictionaryAsync(m => m.Id, cancellationToken);

        var articleIds = items.Select(item => item.ArticleId).Distinct().ToList();
        var articles = await _dbContext.ApiArticles.AsNoTracking().Where(item => articleIds.Contains(item.Id)).ToDictionaryAsync(item => item.Id, cancellationToken);

        var locationIds = items.Where(item => item.WarehouseLocationId.HasValue).Select(item => item.WarehouseLocationId!.Value).Distinct().ToList();
        var locations = await _dbContext.ApiWarehouseLocations.AsNoTracking().Where(item => locationIds.Contains(item.Id)).ToDictionaryAsync(item => item.Id, cancellationToken);
        var warehouseIds = locations.Values.Select(item => item.WarehouseId).Distinct().ToList();
        var warehouses = await _dbContext.ApiWarehouses.AsNoTracking().Where(item => warehouseIds.Contains(item.Id)).ToDictionaryAsync(item => item.Id, cancellationToken);

        var rows = items.Select(item =>
        {
            var movement = movements.GetValueOrDefault(item.StockMovementId);
            var article = articles.GetValueOrDefault(item.ArticleId);
            var location = item.WarehouseLocationId.HasValue ? locations.GetValueOrDefault(item.WarehouseLocationId.Value) : null;
            var warehouse = location is not null ? warehouses.GetValueOrDefault(location.WarehouseId) : null;

            return new ArticleHistoryResponse
            {
                Id = item.Id,
                Notes = movement?.Notes ?? string.Empty,
                CreatedAt = movement?.CreatedUtc ?? default,
                UpdatedAt = movement?.UpdatedUtc ?? default,
                UpdatedBy = Guid.Empty,
                IsDeleted = movement?.IsDeleted ?? false,
                Article = article?.Designation ?? string.Empty,
                ArticlePrice = item.Price,
                ArticleId = item.ArticleId,
                SerialNumber = item.SerialNumber,
                Warehouse = warehouse?.Designation ?? string.Empty,
                WarehouseLocation = location?.Designation ?? string.Empty,
                WarehouseLocationId = item.WarehouseLocationId ?? Guid.Empty,
                Status = (int)Data.Entities.ApiArticleSerialNumberStatus.Available,
                IsComposed = article?.IsComposed ?? false,
                PurchaseDate = movement?.Date,
                SaleDate = null,
                OriginDocument = movement?.DocumentNumber,
                SaleDocument = null,
                InMovementId = item.StockMovementId,
                OutMovementId = null,
                Supplier = null,
                PurchasePrice = item.Price,
                HasExternalDocument = !string.IsNullOrWhiteSpace(movement?.ExternalDocument)
            };
        })
        .OrderByDescending(row => row.CreatedAt)
        .ToList();

        var totalItems = rows.Count;
        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)pageSize);
        var paged = rows.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return new ArticleHistoriesPaginatedResponse
        {
            Items = paged,
            ItemsCount = paged.Count,
            TotalItems = totalItems,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages
        };
    }
}
