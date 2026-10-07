using System.Globalization;
using LogicPOS.Core.FrontOffice;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace LogicPOS.Core.BackOffice;

public sealed class LocalStockService : IStockManagementService
{
    private readonly IServiceScopeFactory _scopes;
    private readonly IPosDocumentService? _documents;

    public LocalStockService(IServiceScopeFactory scopes, IPosDocumentService? documents = null)
    {
        _scopes = scopes;
        _documents = documents;
    }

    public async Task<StockPageResult<StockArticleRow>> GetArticlesAsync(StockPageRequest request, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var query = database.Articles.AsNoTracking()
            .Include(item => item.MeasurementUnit)
            .Where(item => item.IsDeleted == false);
        if (string.IsNullOrWhiteSpace(request.Search) == false)
        {
            var search = request.Search.Trim();
            query = query.Where(item =>
                (item.Code != null && item.Code.Contains(search)) ||
                (item.Designation != null && item.Designation.Contains(search)));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var pageSize = Math.Max(1, request.PageSize);
        var page = Math.Max(1, request.Page);
        var articles = await query
            .OrderBy(item => item.Code)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var ids = articles.Select(item => item.Id).ToList();
        var totals = await database.StockMovements.AsNoTracking()
            .Where(item => item.IsDeleted == false && ids.Contains(item.ArticleId))
            .GroupBy(item => item.ArticleId)
            .Select(group => new { ArticleId = group.Key, Quantity = group.Sum(item => item.Quantity) })
            .ToListAsync(cancellationToken);
        var byId = totals.ToDictionary(item => item.ArticleId, item => item.Quantity);

        return new StockPageResult<StockArticleRow>
        {
            Page = page,
            TotalCount = totalCount,
            TotalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize)),
            Items = articles.Select(item => new StockArticleRow
            {
                Id = item.Id,
                Code = item.Code ?? string.Empty,
                Designation = item.Designation ?? string.Empty,
                MinimumStock = item.MinimumStock,
                TotalStock = byId.TryGetValue(item.Id, out var quantity) ? quantity : 0m,
                Unit = item.MeasurementUnit?.Designation ?? string.Empty,
                UpdatedAt = item.UpdatedAt
            }).ToList()
        };
    }

    public async Task<ListingSaveResult> SaveMinimumStockAsync(Guid articleId, decimal minimumStock, CancellationToken cancellationToken = default)
    {
        if (minimumStock < 0)
        {
            return ListingSaveResult.Fail("O stock mínimo não pode ser negativo.");
        }

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var article = await database.Articles.FirstOrDefaultAsync(item => item.Id == articleId && item.IsDeleted == false, cancellationToken);
        if (article is null)
        {
            return ListingSaveResult.Fail("Artigo não encontrado.");
        }

        article.MinimumStock = minimumStock;
        article.UpdatedAt = DateTime.Now;
        await database.SaveChangesAsync(cancellationToken);
        return ListingSaveResult.Ok(articleId, "Stock mínimo atualizado.");
    }

    public async Task<ListingSaveResult> AdjustArticleStockAsync(Guid articleId, decimal newTotal, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var current = await database.StockMovements.AsNoTracking()
            .Where(item => item.IsDeleted == false && item.ArticleId == articleId)
            .SumAsync(item => item.Quantity, cancellationToken);
        var delta = newTotal - current;
        if (delta == 0)
        {
            return ListingSaveResult.Ok(articleId, "Stock sem alterações.");
        }

        var locationId = await ResolveDefaultLocationAsync(database, cancellationToken);
        await ApplyMovementAsync(database, new StockMovementCreateRequest
        {
            SupplierId = await ResolveSupplierAsync(database, cancellationToken),
            Date = DateTime.Today,
            DocumentNumber = $"ST{DateTime.Now:yyyyMMddHHmmss}",
            Notes = "Ajuste de stock",
            Items =
            [
                new StockMovementLineInput
                {
                    ArticleId = articleId,
                    Quantity = delta,
                    Price = 0,
                    WarehouseLocationId = locationId
                }
            ]
        }, cancellationToken);

        return ListingSaveResult.Ok(articleId, "Stock ajustado.");
    }

    public async Task<StockPageResult<StockMovementRow>> GetMovementsAsync(StockPageRequest request, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var query = database.StockMovements.AsNoTracking()
            .Include(item => item.Customer)
            .Include(item => item.Article)
            .Include(item => item.Detail)!.ThenInclude(detail => detail!.Document)
            .Where(item => item.IsDeleted == false);

        if (request.StartDate is DateTime start)
        {
            query = query.Where(item => item.Date >= start.Date);
        }

        if (request.EndDate is DateTime end)
        {
            var until = end.Date.AddDays(1);
            query = query.Where(item => item.Date < until);
        }

        if (request.ArticleId is Guid articleId)
        {
            query = query.Where(item => item.ArticleId == articleId);
        }

        if (request.CustomerId is Guid customerId)
        {
            query = query.Where(item => item.CustomerId == customerId);
        }

        if (string.IsNullOrWhiteSpace(request.Search) == false)
        {
            var search = request.Search.Trim();
            query = query.Where(item =>
                (item.DocumentNumber != null && item.DocumentNumber.Contains(search)) ||
                (item.Article != null && item.Article.Designation != null && item.Article.Designation.Contains(search)) ||
                (item.SerialNumber != null && item.SerialNumber.Contains(search)) ||
                (item.Customer != null && item.Customer.Name != null && item.Customer.Name.Contains(search)));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var pageSize = Math.Max(1, request.PageSize);
        var page = Math.Max(1, request.Page);
        var items = await query
            .OrderByDescending(item => item.Date)
            .ThenByDescending(item => item.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new StockPageResult<StockMovementRow>
        {
            Page = page,
            TotalCount = totalCount,
            TotalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize)),
            Items = items.Select(item =>
            {
                var saleNumber = item.Detail?.Document?.Number;
                if (string.IsNullOrWhiteSpace(saleNumber) && item.Quantity < 0)
                {
                    saleNumber = item.DocumentNumber;
                }

                return new StockMovementRow
                {
                    Id = item.Id,
                    Date = item.Date,
                    Customer = item.Customer?.Name ?? string.Empty,
                    CustomerId = item.CustomerId,
                    DocumentNumber = item.DocumentNumber ?? string.Empty,
                    Article = item.Article?.Designation ?? string.Empty,
                    ArticleId = item.ArticleId,
                    SerialNumber = item.SerialNumber ?? string.Empty,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    HasExternalDocument = item.ExternalDocument is { Length: > 0 },
                    HasSaleDocument = string.IsNullOrWhiteSpace(saleNumber) == false,
                    SaleDocumentNumber = saleNumber,
                    CanEdit = item.DetailId is null
                };
            }).ToList()
        };
    }

    public async Task<ListingSaveResult> CreateMovementAsync(StockMovementCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Items.Count == 0)
        {
            return ListingSaveResult.Fail("Indique pelo menos um artigo.");
        }

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        try
        {
            await ApplyMovementAsync(database, request, cancellationToken);
            return ListingSaveResult.Ok(Guid.Empty, "Movimento de stock registado.");
        }
        catch (Exception exception)
        {
            return ListingSaveResult.Fail(exception.Message);
        }
    }

    public async Task<ListingSaveResult> UpdateMovementAsync(StockMovementUpdateRequest request, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var movement = await database.StockMovements.FirstOrDefaultAsync(item => item.Id == request.Id && item.IsDeleted == false, cancellationToken);
        if (movement is null)
        {
            return ListingSaveResult.Fail("Movimento não encontrado.");
        }

        if (movement.DetailId is not null)
        {
            return ListingSaveResult.Fail("Este movimento está ligado a um documento e não pode ser editado.");
        }

        var oldQuantity = movement.Quantity;
        if (request.SupplierId is Guid supplier)
        {
            movement.CustomerId = supplier;
        }

        if (request.Date is DateTime date)
        {
            movement.Date = date;
        }

        if (request.DocumentNumber is not null)
        {
            movement.DocumentNumber = request.DocumentNumber;
        }

        if (request.Quantity is decimal quantity)
        {
            movement.Quantity = quantity;
        }

        if (request.Price is decimal price)
        {
            movement.Price = price;
        }

        if (request.ExternalDocument is { Length: > 0 })
        {
            movement.ExternalDocument = request.ExternalDocument;
        }

        movement.UpdatedAt = DateTime.Now;

        if (request.Quantity is decimal updatedQuantity && updatedQuantity != oldQuantity && movement.WarehouseLocationId is Guid locationId)
        {
            var delta = updatedQuantity - oldQuantity;
            var stock = await FindOrCreateWarehouseArticleAsync(
                database,
                movement.ArticleId,
                locationId,
                movement.SerialNumber,
                cancellationToken);
            stock.Quantity += delta;
            stock.UpdatedAt = DateTime.Now;
        }

        await database.SaveChangesAsync(cancellationToken);
        return ListingSaveResult.Ok(movement.Id, "Movimento atualizado.");
    }

    public async Task<ListingSaveResult> DeleteMovementAsync(Guid movementId, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var movement = await database.StockMovements.FirstOrDefaultAsync(item => item.Id == movementId && item.IsDeleted == false, cancellationToken);
        if (movement is null)
        {
            return ListingSaveResult.Fail("Movimento não encontrado.");
        }

        if (movement.DetailId is not null)
        {
            return ListingSaveResult.Fail("Este movimento está ligado a um documento e não pode ser apagado.");
        }

        if (movement.WarehouseLocationId is Guid locationId)
        {
            var stock = await FindOrCreateWarehouseArticleAsync(
                database,
                movement.ArticleId,
                locationId,
                movement.SerialNumber,
                cancellationToken);
            stock.Quantity -= movement.Quantity;
            stock.UpdatedAt = DateTime.Now;
        }

        movement.IsDeleted = true;
        movement.UpdatedAt = DateTime.Now;
        await database.SaveChangesAsync(cancellationToken);
        return ListingSaveResult.Ok(movementId, "Movimento apagado.");
    }

    public async Task<string?> SaveExternalDocumentAsync(Guid movementId, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var movement = await database.StockMovements.AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == movementId && item.IsDeleted == false, cancellationToken);
        if (movement?.ExternalDocument is not { Length: > 0 })
        {
            return null;
        }

        var path = Path.Combine(Path.GetTempPath(), "logicpos-stock-" + movementId.ToString("N") + ".pdf");
        await File.WriteAllBytesAsync(path, movement.ExternalDocument, cancellationToken);
        return path;
    }

    public async Task<string?> SaveSaleDocumentPdfAsync(string documentNumber, CancellationToken cancellationToken = default)
    {
        if (_documents is null || string.IsNullOrWhiteSpace(documentNumber))
        {
            return null;
        }

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var document = await database.Documents.AsNoTracking()
            .FirstOrDefaultAsync(item => item.IsDeleted == false && item.Number == documentNumber, cancellationToken);
        return document is null ? null : await _documents.CreateA4FileAsync(document.Id, cancellationToken);
    }

    public async Task<StockPageResult<StockHistoryRow>> GetHistoryAsync(StockPageRequest request, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var query = database.WarehouseArticles.AsNoTracking()
            .Include(item => item.Article)
            .Include(item => item.WarehouseLocation)!.ThenInclude(location => location!.Warehouse)
            .Where(item => item.IsDeleted == false && string.IsNullOrEmpty(item.SerialNumber) == false);

        if (request.ArticleId is Guid articleId)
        {
            query = query.Where(item => item.ArticleId == articleId);
        }

        if (string.IsNullOrWhiteSpace(request.Search) == false)
        {
            var search = request.Search.Trim();
            query = query.Where(item =>
                (item.SerialNumber != null && item.SerialNumber.Contains(search)) ||
                (item.Article != null && item.Article.Designation != null && item.Article.Designation.Contains(search)));
        }

        if (string.IsNullOrWhiteSpace(request.Status) == false &&
            Enum.TryParse<ArticleSerialNumberStatus>(request.Status, true, out var status))
        {
            query = query.Where(item => item.Status == status);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var pageSize = Math.Max(1, request.PageSize);
        var page = Math.Max(1, request.Page);
        var items = await query
            .OrderByDescending(item => item.UpdatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var serials = items.Select(item => item.SerialNumber!).ToList();
        var movements = await database.StockMovements.AsNoTracking()
            .Include(item => item.Customer)
            .Include(item => item.Detail)!.ThenInclude(detail => detail!.Document)
            .Where(item => item.IsDeleted == false && item.SerialNumber != null && serials.Contains(item.SerialNumber))
            .ToListAsync(cancellationToken);

        return new StockPageResult<StockHistoryRow>
        {
            Page = page,
            TotalCount = totalCount,
            TotalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize)),
            Items = items.Select(item =>
            {
                var related = movements.Where(move => move.SerialNumber == item.SerialNumber && move.ArticleId == item.ArticleId).ToList();
                var inbound = related.Where(move => move.Quantity > 0).OrderByDescending(move => move.Date).FirstOrDefault();
                var outbound = related.Where(move => move.Quantity < 0).OrderByDescending(move => move.Date).FirstOrDefault();
                return new StockHistoryRow
                {
                    Id = item.Id,
                    ArticleId = item.ArticleId,
                    Article = item.Article?.Designation ?? string.Empty,
                    SerialNumber = item.SerialNumber ?? string.Empty,
                    Status = StatusLabel(item.Status),
                    IsComposed = item.Article?.IsComposed == true,
                    IsSold = item.Status == ArticleSerialNumberStatus.Sold || outbound is not null,
                    Warehouse = item.WarehouseLocation?.Warehouse?.Designation ?? string.Empty,
                    Location = item.WarehouseLocation?.Designation ?? string.Empty,
                    WarehouseLocationId = item.WarehouseLocationId,
                    WarehouseArticleId = item.Id,
                    Supplier = inbound?.Customer?.Name ?? string.Empty,
                    PurchasePrice = inbound?.Price ?? 0m,
                    ArticlePrice = item.Article?.Price1?.Value ?? 0m,
                    PurchaseDate = inbound?.Date,
                    SaleDate = outbound?.Date,
                    OriginDocument = inbound?.DocumentNumber,
                    SaleDocument = outbound?.Detail?.Document?.Number ?? outbound?.DocumentNumber,
                    InMovementId = inbound?.Id ?? Guid.Empty,
                    OutMovementId = outbound?.Id,
                    HasExternalDocument = inbound?.ExternalDocument is { Length: > 0 }
                };
            }).ToList()
        };
    }

    public async Task<string?> GenerateBarcodeLabelsAsync(IReadOnlyList<Guid> ids, CancellationToken cancellationToken = default)
    {
        if (ids.Count == 0)
        {
            return null;
        }

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var serials = await database.WarehouseArticles.AsNoTracking()
            .Where(item => ids.Contains(item.Id) && item.SerialNumber != null)
            .Select(item => item.SerialNumber!)
            .ToListAsync(cancellationToken);
        return serials.Count == 0 ? null : WriteBarcodePdf(serials);
    }

    public Task<string?> GenerateBarcodeLabelsBySerialAsync(IReadOnlyList<string> serialNumbers, CancellationToken cancellationToken = default)
    {
        var serials = serialNumbers.Where(item => string.IsNullOrWhiteSpace(item) == false).Distinct().ToList();
        return Task.FromResult(serials.Count == 0 ? null : WriteBarcodePdf(serials));
    }

    public async Task<StockDashboardSnapshot> LoadDashboardAsync(int year, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var from = new DateTime(year, 1, 1);
        var to = from.AddYears(1);
        var totals = await database.StockMovements.AsNoTracking()
            .Where(item => item.IsDeleted == false)
            .GroupBy(item => item.ArticleId)
            .Select(group => new { ArticleId = group.Key, Quantity = group.Sum(item => item.Quantity) })
            .ToListAsync(cancellationToken);
        var byArticle = totals.ToDictionary(item => item.ArticleId, item => item.Quantity);
        var minimums = await database.Articles.AsNoTracking()
            .Where(item => item.IsDeleted == false && item.MinimumStock > 0)
            .Select(item => new { item.Id, item.MinimumStock })
            .ToListAsync(cancellationToken);
        var serials = await database.WarehouseArticles.AsNoTracking()
            .Where(item => item.IsDeleted == false && item.SerialNumber != null && item.SerialNumber != "")
            .Select(item => new
            {
                item.SerialNumber,
                item.Status,
                Composed = item.Article != null && item.Article.IsComposed
            })
            .ToListAsync(cancellationToken);
        var soldSerials = (await database.StockMovements.AsNoTracking()
            .Where(item => item.IsDeleted == false && item.Quantity < 0 && item.SerialNumber != null && item.SerialNumber != "")
            .Select(item => item.SerialNumber!)
            .Distinct()
            .ToListAsync(cancellationToken)).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var inStock = serials
            .Where(item => item.Status != ArticleSerialNumberStatus.Sold && soldSerials.Contains(item.SerialNumber!) == false)
            .ToList();
        var soldInYear = await database.StockMovements.AsNoTracking()
            .Where(item => item.IsDeleted == false
                && item.Quantity < 0
                && item.SerialNumber != null
                && item.SerialNumber != ""
                && item.Date >= from
                && item.Date < to)
            .Select(item => item.SerialNumber!)
            .Distinct()
            .CountAsync(cancellationToken);
        var keys = inStock.Select(item => item.SerialNumber!).Distinct().ToList();
        var stockValue = 0m;
        if (keys.Count > 0)
        {
            var inbound = await database.StockMovements.AsNoTracking()
                .Where(item => item.IsDeleted == false && item.Quantity > 0 && item.SerialNumber != null && keys.Contains(item.SerialNumber))
                .Select(item => new { item.SerialNumber, item.Price, item.Date })
                .ToListAsync(cancellationToken);
            stockValue = inbound
                .GroupBy(item => item.SerialNumber, StringComparer.OrdinalIgnoreCase)
                .Sum(group => group.OrderByDescending(item => item.Date).First().Price);
        }

        var positions = await database.WarehouseArticles.AsNoTracking()
            .Where(item => item.IsDeleted == false && item.Quantity != 0)
            .Select(item => new
            {
                Warehouse = item.WarehouseLocation != null && item.WarehouseLocation.Warehouse != null
                    ? item.WarehouseLocation.Warehouse.Designation
                    : string.Empty,
                item.Quantity
            })
            .ToListAsync(cancellationToken);
        var sales = await database.StockMovements.AsNoTracking()
            .Where(item => item.IsDeleted == false && item.Quantity < 0 && item.Date >= from && item.Date < to)
            .Select(item => new
            {
                Article = item.Article != null ? item.Article.Designation : string.Empty,
                Quantity = -item.Quantity
            })
            .ToListAsync(cancellationToken);

        return new StockDashboardSnapshot
        {
            Year = year,
            BelowMinimum = minimums.Count(item => byArticle.GetValueOrDefault(item.Id) < item.MinimumStock),
            UniqueInStock = inStock.Count,
            UniqueSoldInYear = soldInYear,
            UniqueComposedInStock = inStock.Count(item => item.Composed),
            StockValue = stockValue,
            Warehouses = StockDashboard.Rank(positions.Select(item => (Label: item.Warehouse, Value: item.Quantity)), int.MaxValue),
            TopSold = StockDashboard.Rank(sales.Select(item => (Label: item.Article, Value: item.Quantity)), 8)
        };
    }

    public Task<StockModuleAccess> GetModuleAccessAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(new StockModuleAccess { HasModule = true });

    public async Task<IReadOnlyList<StockCompositionSlot>> GetCompositionSlotsAsync(Guid warehouseArticleId, Guid articleId, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var links = await database.UniqueArticleCompositions.AsNoTracking()
            .Include(item => item.Child)!.ThenInclude(child => child!.Article)
            .Where(item => item.ParentId == warehouseArticleId && item.ChildId != null)
            .ToListAsync(cancellationToken);
        if (links.Count > 0)
        {
            return links
                .Where(item => item.Child is not null)
                .Select(item => new StockCompositionSlot
                {
                    ArticleId = item.Child!.ArticleId,
                    Article = item.Child.Article?.Designation ?? item.Child.SerialNumber ?? string.Empty,
                    ChildId = item.ChildId,
                    SerialNumber = item.Child.SerialNumber ?? string.Empty
                })
                .ToList();
        }

        var recipe = await database.ArticleCompositions.AsNoTracking()
            .Include(item => item.Child)
            .Where(item => item.ParentId == articleId)
            .ToListAsync(cancellationToken);
        return recipe
            .Select(item => new StockCompositionSlot
            {
                ArticleId = item.ChildId,
                Article = item.Child?.Designation ?? string.Empty
            })
            .ToList();
    }

    public async Task<IReadOnlyList<StockSaleDocumentOption>> LookupSaleDocumentsAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var documents = await database.Documents.AsNoTracking()
            .Where(item => item.IsDeleted == false && item.Number != string.Empty)
            .OrderByDescending(item => item.CreatedAt)
            .Take(100)
            .ToListAsync(cancellationToken);
        return documents
            .Select(item => new StockSaleDocumentOption
            {
                CustomerId = item.CustomerId,
                Number = item.Number,
                Label = string.IsNullOrWhiteSpace(item.Customer?.Name) ? item.Number : $"{item.Number} · {item.Customer.Name}"
            })
            .ToList();
    }

    public async Task<ListingSaveResult> ChangeLocationAsync(Guid warehouseArticleId, Guid locationId, decimal quantity, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var source = await database.WarehouseArticles.FirstOrDefaultAsync(item => item.Id == warehouseArticleId && item.IsDeleted == false, cancellationToken);
        if (source is null)
        {
            return ListingSaveResult.Fail("Posição de stock não encontrada.");
        }

        if (quantity <= 0 || quantity > source.Quantity)
        {
            return ListingSaveResult.Fail("A quantidade a transferir é inválida.");
        }

        if (source.WarehouseLocationId == locationId)
        {
            return ListingSaveResult.Ok(warehouseArticleId, "Localização sem alterações.");
        }

        source.Quantity -= quantity;
        source.UpdatedAt = DateTime.Now;
        if (source.Quantity <= 0 && string.IsNullOrWhiteSpace(source.SerialNumber) == false)
        {
            source.IsDeleted = true;
        }

        var target = await FindOrCreateWarehouseArticleAsync(database, source.ArticleId, locationId, source.SerialNumber, cancellationToken);
        target.Quantity += quantity;
        target.Status = source.Status;
        target.UpdatedAt = DateTime.Now;
        await database.SaveChangesAsync(cancellationToken);
        return ListingSaveResult.Ok(target.Id, "Localização atualizada.");
    }

    public async Task<ListingSaveResult> UpdateUniqueArticleAsync(Guid warehouseArticleId, string? serialNumber, IReadOnlyList<Guid>? children, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var item = await database.WarehouseArticles.FirstOrDefaultAsync(row => row.Id == warehouseArticleId && row.IsDeleted == false, cancellationToken);
        if (item is null)
        {
            return ListingSaveResult.Fail("Número de série não encontrado.");
        }

        if (string.IsNullOrWhiteSpace(serialNumber) == false)
        {
            item.SerialNumber = serialNumber.Trim();
        }

        if (children is not null)
        {
            var existing = await database.UniqueArticleCompositions
                .Where(row => row.ParentId == warehouseArticleId)
                .ToListAsync(cancellationToken);
            database.UniqueArticleCompositions.RemoveRange(existing);
            foreach (var childId in children.Distinct())
            {
                database.UniqueArticleCompositions.Add(new UniqueArticleComposition
                {
                    ParentId = warehouseArticleId,
                    ChildId = childId
                });
            }
        }

        item.UpdatedAt = DateTime.Now;
        await database.SaveChangesAsync(cancellationToken);
        return ListingSaveResult.Ok(warehouseArticleId, "Artigo único atualizado.");
    }

    public async Task<ListingSaveResult> ExchangeUniqueArticleAsync(Guid returnedId, Guid exchangeId, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var returned = await database.WarehouseArticles.FirstOrDefaultAsync(item => item.Id == returnedId && item.IsDeleted == false, cancellationToken);
        var exchange = await database.WarehouseArticles.FirstOrDefaultAsync(item => item.Id == exchangeId && item.IsDeleted == false, cancellationToken);
        if (returned is null || exchange is null)
        {
            return ListingSaveResult.Fail("Artigos da troca não encontrados.");
        }

        if (exchange.Status != ArticleSerialNumberStatus.Available)
        {
            return ListingSaveResult.Fail("O artigo de troca tem de estar disponível.");
        }

        returned.Status = ArticleSerialNumberStatus.Returned;
        returned.UpdatedAt = DateTime.Now;
        exchange.Status = ArticleSerialNumberStatus.Sold;
        exchange.UpdatedAt = DateTime.Now;
        await database.SaveChangesAsync(cancellationToken);
        return ListingSaveResult.Ok(exchangeId, "Troca registada.");
    }

    public async Task<ListingSaveResult> DeleteSerialAsync(Guid warehouseArticleId, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var item = await database.WarehouseArticles.FirstOrDefaultAsync(row => row.Id == warehouseArticleId && row.IsDeleted == false, cancellationToken);
        if (item is null)
        {
            return ListingSaveResult.Fail("Número de série não encontrado.");
        }

        if (item.Status == ArticleSerialNumberStatus.Sold)
        {
            return ListingSaveResult.Fail("Não é possível apagar um número de série vendido.");
        }

        if (item.Quantity != 0)
        {
            database.StockMovements.Add(new StockMovement
            {
                ArticleId = item.ArticleId,
                WarehouseLocationId = item.WarehouseLocationId,
                SerialNumber = item.SerialNumber,
                Quantity = -item.Quantity,
                Date = DateTime.Now,
                DocumentNumber = $"ST{DateTime.Now:yyyyMMddHHmmss}",
                Notes = "Anulação de número de série"
            });
            item.Quantity = 0;
        }

        item.IsDeleted = true;
        item.UpdatedAt = DateTime.Now;
        await database.SaveChangesAsync(cancellationToken);
        return ListingSaveResult.Ok(warehouseArticleId, "Número de série removido.");
    }

    public async Task<IReadOnlyList<StockSerialChoice>> SearchAvailableSerialsAsync(string? search, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var term = search?.Trim();
        var query = database.WarehouseArticles.AsNoTracking()
            .Where(item => item.IsDeleted == false
                && item.Status == ArticleSerialNumberStatus.Available
                && item.SerialNumber != null
                && item.SerialNumber != "");
        if (string.IsNullOrWhiteSpace(term) == false)
        {
            query = query.Where(item =>
                item.SerialNumber!.Contains(term)
                || (item.Article != null && item.Article.Code.Contains(term))
                || (item.Article != null && item.Article.Designation != null && item.Article.Designation.Contains(term)));
        }

        return await query
            .OrderBy(item => item.SerialNumber)
            .Take(80)
            .Select(item => new StockSerialChoice
            {
                ArticleId = item.ArticleId,
                Code = item.Article != null ? item.Article.Code : string.Empty,
                Designation = item.Article != null ? item.Article.Designation ?? string.Empty : string.Empty,
                SerialNumber = item.SerialNumber ?? string.Empty
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LookupOption>> GetAvailableSerialsAsync(Guid articleId, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        return await database.WarehouseArticles.AsNoTracking()
            .Where(item => item.IsDeleted == false &&
                           item.ArticleId == articleId &&
                           item.Status == ArticleSerialNumberStatus.Available &&
                           string.IsNullOrEmpty(item.SerialNumber) == false)
            .OrderBy(item => item.SerialNumber)
            .Select(item => new LookupOption
            {
                Id = item.Id,
                Label = item.SerialNumber!,
                Meta = item.WarehouseLocationId.ToString()
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<StockPageResult<StockWarehouseRow>> GetWarehouseArticlesAsync(StockPageRequest request, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var query = database.WarehouseArticles.AsNoTracking()
            .Include(item => item.Article)
            .Include(item => item.WarehouseLocation)!.ThenInclude(location => location!.Warehouse)
            .Where(item => item.IsDeleted == false && item.Quantity != 0);

        if (request.ArticleId is Guid articleId)
        {
            query = query.Where(item => item.ArticleId == articleId);
        }

        if (string.IsNullOrWhiteSpace(request.Search) == false)
        {
            var search = request.Search.Trim();
            query = query.Where(item =>
                (item.Article != null && item.Article.Designation != null && item.Article.Designation.Contains(search)) ||
                (item.SerialNumber != null && item.SerialNumber.Contains(search)) ||
                (item.WarehouseLocation != null && item.WarehouseLocation.Designation.Contains(search)));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var pageSize = Math.Max(1, request.PageSize);
        var page = Math.Max(1, request.Page);
        var items = await query
            .OrderBy(item => item.Article!.Designation)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new StockPageResult<StockWarehouseRow>
        {
            Page = page,
            TotalCount = totalCount,
            TotalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize)),
            Items = items.Select(item => new StockWarehouseRow
            {
                Id = item.Id,
                ArticleId = item.ArticleId,
                Article = item.Article?.Designation ?? string.Empty,
                SerialNumber = item.SerialNumber ?? string.Empty,
                WarehouseId = item.WarehouseLocation?.WarehouseId ?? Guid.Empty,
                Warehouse = item.WarehouseLocation?.Warehouse?.Designation ?? string.Empty,
                LocationId = item.WarehouseLocationId,
                Location = item.WarehouseLocation?.Designation ?? string.Empty,
                Quantity = item.Quantity,
                Status = StatusLabel(item.Status)
            }).ToList()
        };
    }

    public async Task<IReadOnlyList<StockLocationOption>> GetLocationsAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        return await database.WarehouseLocations.AsNoTracking()
            .Include(item => item.Warehouse)
            .Where(item => item.IsDeleted == false)
            .OrderBy(item => item.Warehouse!.Designation)
            .ThenBy(item => item.Designation)
            .Select(item => new StockLocationOption
            {
                Id = item.Id,
                WarehouseId = item.WarehouseId,
                Warehouse = item.Warehouse != null ? item.Warehouse.Designation : string.Empty,
                Designation = item.Designation,
                IsDefault = item.IsDefault
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LookupOption>> LookupArticlesAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var rows = await database.Articles.AsNoTracking()
            .Where(item => item.IsDeleted == false)
            .OrderBy(item => item.Code)
            .Select(item => new { item.Id, item.Code, item.Designation, item.Barcode })
            .ToListAsync(cancellationToken);
        return rows
            .Select(item => LookupOption.FromArticle(item.Id, item.Code, item.Designation, item.Barcode))
            .ToList();
    }

    public async Task<IReadOnlyList<LookupOption>> LookupSuppliersAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        return await database.Customers.AsNoTracking()
            .Where(item => item.IsDeleted == false && item.Supplier)
            .OrderBy(item => item.Name)
            .Select(item => new LookupOption
            {
                Id = item.Id,
                Label = item.Name ?? item.FiscalNumber ?? item.Id.ToString()
            })
            .ToListAsync(cancellationToken);
    }

    private static async Task ApplyMovementAsync(LogicPOSDbContext database, StockMovementCreateRequest request, CancellationToken cancellationToken)
    {
        var defaultLocation = await ResolveDefaultLocationAsync(database, cancellationToken);
        foreach (var line in request.Items)
        {
            var locationId = line.WarehouseLocationId ?? defaultLocation;
            if (locationId is null)
            {
                throw new InvalidOperationException("Não há localização de armazém disponível.");
            }

            var serial = string.IsNullOrWhiteSpace(line.SerialNumber) ? null : line.SerialNumber.Trim();
            if (string.IsNullOrWhiteSpace(serial) == false && line.Quantity > 0)
            {
                var exists = await database.WarehouseArticles.AsNoTracking()
                    .AnyAsync(item => item.IsDeleted == false && item.SerialNumber == serial, cancellationToken);
                if (exists)
                {
                    throw new InvalidOperationException($"O número de série {serial} já existe.");
                }
            }

            database.StockMovements.Add(new StockMovement
            {
                CustomerId = request.SupplierId == Guid.Empty ? null : request.SupplierId,
                ArticleId = line.ArticleId,
                SerialNumber = serial,
                WarehouseLocationId = locationId,
                DocumentNumber = request.DocumentNumber,
                Quantity = line.Quantity,
                Price = line.Price,
                Date = request.Date,
                Notes = request.Notes,
                ExternalDocument = request.ExternalDocument
            });

            var stock = await FindOrCreateWarehouseArticleAsync(database, line.ArticleId, locationId.Value, serial, cancellationToken);
            stock.Quantity += line.Quantity;
            if (string.IsNullOrWhiteSpace(serial) == false)
            {
                stock.Status = line.Quantity < 0 ? ArticleSerialNumberStatus.Sold : ArticleSerialNumberStatus.Available;
                stock.Quantity = line.Quantity < 0 ? 0 : 1;
            }

            stock.UpdatedAt = DateTime.Now;
        }

        await database.SaveChangesAsync(cancellationToken);
    }

    private static async Task<WarehouseArticle> FindOrCreateWarehouseArticleAsync(
        LogicPOSDbContext database,
        Guid articleId,
        Guid locationId,
        string? serialNumber,
        CancellationToken cancellationToken)
    {
        WarehouseArticle? stock;
        if (string.IsNullOrWhiteSpace(serialNumber) == false)
        {
            stock = await database.WarehouseArticles
                .FirstOrDefaultAsync(item =>
                    item.IsDeleted == false &&
                    item.ArticleId == articleId &&
                    item.SerialNumber == serialNumber, cancellationToken);
        }
        else
        {
            stock = await database.WarehouseArticles
                .FirstOrDefaultAsync(item =>
                    item.IsDeleted == false &&
                    item.ArticleId == articleId &&
                    item.WarehouseLocationId == locationId &&
                    (item.SerialNumber == null || item.SerialNumber == string.Empty), cancellationToken);
        }

        if (stock is not null)
        {
            return stock;
        }

        stock = new WarehouseArticle
        {
            ArticleId = articleId,
            WarehouseLocationId = locationId,
            SerialNumber = serialNumber,
            Quantity = 0,
            Status = string.IsNullOrWhiteSpace(serialNumber)
                ? ArticleSerialNumberStatus.None
                : ArticleSerialNumberStatus.Available
        };
        database.WarehouseArticles.Add(stock);
        return stock;
    }

    private static async Task<Guid?> ResolveDefaultLocationAsync(LogicPOSDbContext database, CancellationToken cancellationToken)
    {
        var location = await database.WarehouseLocations.AsNoTracking()
            .Where(item => item.IsDeleted == false)
            .OrderByDescending(item => item.IsDefault)
            .ThenBy(item => item.Designation)
            .FirstOrDefaultAsync(cancellationToken);
        return location?.Id;
    }

    private static async Task<Guid> ResolveSupplierAsync(LogicPOSDbContext database, CancellationToken cancellationToken)
    {
        var supplier = await database.Customers.AsNoTracking()
            .Where(item => item.IsDeleted == false && item.Supplier)
            .OrderBy(item => item.Name)
            .FirstOrDefaultAsync(cancellationToken);
        return supplier?.Id ?? Guid.Empty;
    }

    private static string StatusLabel(ArticleSerialNumberStatus status) => status switch
    {
        ArticleSerialNumberStatus.Available => "Disponível",
        ArticleSerialNumberStatus.Sold => "Vendido",
        ArticleSerialNumberStatus.Exchanged => "Trocado",
        ArticleSerialNumberStatus.Returned => "Devolvido",
        _ => "Nenhum"
    };

    private static string WriteBarcodePdf(IReadOnlyList<string> serials)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        var writer = new ZXing.BarcodeWriter<SKBitmap>
        {
            Format = ZXing.BarcodeFormat.CODE_128,
            Options = new ZXing.Common.EncodingOptions
            {
                Width = 320,
                Height = 90,
                Margin = 2,
                PureBarcode = true
            }
        };

        var images = serials.Select(serial =>
        {
            using var bitmap = writer.Write(serial);
            using var data = bitmap.Encode(SKEncodedImageFormat.Png, 100);
            return (serial, bytes: data.ToArray());
        }).ToList();

        var path = Path.Combine(Path.GetTempPath(), "logicpos-labels-" + Guid.NewGuid().ToString("N") + ".pdf");
        QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(24);
                page.Content().Column(column =>
                {
                    column.Spacing(16);
                    foreach (var (serial, bytes) in images)
                    {
                        column.Item().Column(label =>
                        {
                            label.Item().AlignCenter().Width(220).Image(bytes);
                            label.Item().AlignCenter().Text(serial);
                        });
                    }
                });
            });
        }).GeneratePdf(path);
        return path;
    }
}
