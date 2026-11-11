using LogicPOS.Application.Features.Articles.Stocks;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Domain.Results;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.BackOffice;

/// <summary>
/// Applies DocumentType.StockMode when a fiscal document is issued or cancelled.
/// Without this, serial sales stay Available in Gestão de Stocks.
/// </summary>
public sealed class LocalDocumentStockProcessor : IDocumentStockProcessor
{
    private readonly IServiceScopeFactory _scopes;

    public LocalDocumentStockProcessor(IServiceScopeFactory scopes)
    {
        _scopes = scopes;
    }

    public async Task<Result> ProcessIssuedDocumentAsync(
        Document document,
        ProcessArticleStockMode stockMode,
        ArticleSerialNumberStatus returnStatus = ArticleSerialNumberStatus.Available,
        CancellationToken ct = default)
    {
        if (stockMode == ProcessArticleStockMode.None || document.IsDraft)
        {
            return Result.Success();
        }

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var details = await LoadDetailsAsync(database, document, ct);
        await ApplyAsync(database, document, details, stockMode, reverse: false, returnStatus, ct);
        await database.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> ProcessCancelledDocumentAsync(
        Document document,
        ProcessArticleStockMode stockMode,
        ArticleSerialNumberStatus returnStatus = ArticleSerialNumberStatus.Available,
        CancellationToken ct = default)
    {
        if (stockMode == ProcessArticleStockMode.None || document.IsDraft)
        {
            return Result.Success();
        }

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var details = await LoadDetailsAsync(database, document, ct);
        await ApplyAsync(database, document, details, stockMode, reverse: true, returnStatus, ct);
        await database.SaveChangesAsync(ct);
        return Result.Success();
    }

    /// <summary>Same-context apply used by POS issue/cancel before a shared SaveChanges.</summary>
    public static Task ApplyOnContextAsync(
        LogicPOSDbContext database,
        Document document,
        IEnumerable<DocumentDetail> details,
        ProcessArticleStockMode stockMode,
        bool reverse,
        CancellationToken cancellationToken = default,
        ArticleSerialNumberStatus returnStatus = ArticleSerialNumberStatus.Available)
    {
        if (stockMode == ProcessArticleStockMode.None || document.IsDraft)
        {
            return Task.CompletedTask;
        }

        return ApplyAsync(database, document, details, stockMode, reverse, returnStatus, cancellationToken);
    }

    public static decimal SignedQuantity(ProcessArticleStockMode stockMode, decimal lineQuantity, bool reverse)
    {
        var magnitude = Math.Abs(lineQuantity);
        var outbound = stockMode == ProcessArticleStockMode.Out;
        if (reverse)
        {
            outbound = !outbound;
        }

        return outbound ? -magnitude : magnitude;
    }

    public static ArticleSerialNumberStatus SerialStatusFor(
        ProcessArticleStockMode stockMode,
        bool reverse,
        ArticleSerialNumberStatus returnStatus = ArticleSerialNumberStatus.Available)
    {
        var outbound = stockMode == ProcessArticleStockMode.Out;
        if (reverse)
        {
            outbound = !outbound;
        }

        return outbound ? ArticleSerialNumberStatus.Sold : returnStatus;
    }

    private static async Task<IReadOnlyList<DocumentDetail>> LoadDetailsAsync(
        LogicPOSDbContext database,
        Document document,
        CancellationToken cancellationToken)
    {
        if (document.Details is { Count: > 0 })
        {
            return document.Details.Where(item => item.IsDeleted == false).ToList();
        }

        return await database.DocumentDetails
            .Where(item => item.DocumentId == document.Id && item.IsDeleted == false)
            .ToListAsync(cancellationToken);
    }

    private static async Task ApplyAsync(
        LogicPOSDbContext database,
        Document document,
        IEnumerable<DocumentDetail> details,
        ProcessArticleStockMode stockMode,
        bool reverse,
        ArticleSerialNumberStatus returnStatus,
        CancellationToken cancellationToken)
    {
        var defaultLocation = await ResolveDefaultLocationAsync(database, cancellationToken);
        var lines = details.Where(item => item.IsDeleted == false && item.ArticleId != Guid.Empty).ToList();
        if (lines.Count == 0)
        {
            return;
        }

        var detailIds = lines.Select(item => item.Id).ToList();
        var existing = await database.StockMovements
            .Where(item => item.IsDeleted == false && item.DetailId != null && detailIds.Contains(item.DetailId.Value))
            .ToListAsync(cancellationToken);

        if (reverse)
        {
            foreach (var movement in existing)
            {
                await ReverseMovementAsync(database, movement, returnStatus, cancellationToken);
            }

            return;
        }

        if (existing.Count > 0)
        {
            // Already applied for these lines (idempotent re-entry).
            return;
        }

        foreach (var line in lines)
        {
            var quantity = SignedQuantity(stockMode, line.Quantity, reverse: false);
            if (quantity == 0)
            {
                continue;
            }

            var serial = string.IsNullOrWhiteSpace(line.SerialNumber) ? null : line.SerialNumber.Trim();
            Guid? locationId = defaultLocation;
            if (string.IsNullOrWhiteSpace(serial) == false)
            {
                var serialStock = await database.WarehouseArticles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        item => item.IsDeleted == false
                            && item.ArticleId == line.ArticleId
                            && item.SerialNumber == serial,
                        cancellationToken);
                if (serialStock is not null)
                {
                    locationId = serialStock.WarehouseLocationId;
                }
            }

            if (locationId is null)
            {
                continue;
            }

            database.StockMovements.Add(new StockMovement
            {
                CustomerId = document.CustomerId,
                ArticleId = line.ArticleId,
                SerialNumber = serial,
                WarehouseLocationId = locationId,
                DocumentNumber = document.Number,
                Quantity = quantity,
                Price = line.Price,
                Date = document.CreatedAt == default ? DateTime.Now : document.CreatedAt.Date,
                DetailId = line.Id
            });

            var stock = await FindOrCreateWarehouseArticleAsync(
                database,
                line.ArticleId,
                locationId.Value,
                serial,
                cancellationToken);
            stock.Quantity += quantity;
            if (string.IsNullOrWhiteSpace(serial) == false)
            {
                stock.Status = SerialStatusFor(stockMode, reverse: false, returnStatus);
                stock.Quantity = quantity < 0 ? 0 : 1;
            }

            stock.UpdatedAt = DateTime.Now;
        }
    }

    private static async Task ReverseMovementAsync(
        LogicPOSDbContext database,
        StockMovement movement,
        ArticleSerialNumberStatus returnStatus,
        CancellationToken cancellationToken)
    {
        if (movement.WarehouseLocationId is Guid locationId)
        {
            var stock = await FindOrCreateWarehouseArticleAsync(
                database,
                movement.ArticleId,
                locationId,
                movement.SerialNumber,
                cancellationToken);
            stock.Quantity -= movement.Quantity;
            if (string.IsNullOrWhiteSpace(movement.SerialNumber) == false && movement.Quantity < 0)
            {
                stock.Status = returnStatus;
                if (stock.Quantity < 1)
                {
                    stock.Quantity = 1;
                }
            }
            else if (string.IsNullOrWhiteSpace(movement.SerialNumber) == false && movement.Quantity > 0)
            {
                stock.Status = ArticleSerialNumberStatus.Sold;
                stock.Quantity = 0;
            }

            stock.UpdatedAt = DateTime.Now;
        }

        movement.IsDeleted = true;
        movement.UpdatedAt = DateTime.Now;
        await Task.CompletedTask;
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
                .FirstOrDefaultAsync(
                    item => item.IsDeleted == false
                        && item.ArticleId == articleId
                        && item.SerialNumber == serialNumber,
                    cancellationToken);
        }
        else
        {
            stock = await database.WarehouseArticles
                .FirstOrDefaultAsync(
                    item => item.IsDeleted == false
                        && item.ArticleId == articleId
                        && item.WarehouseLocationId == locationId
                        && (item.SerialNumber == null || item.SerialNumber == string.Empty),
                    cancellationToken);
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

    private static async Task<Guid?> ResolveDefaultLocationAsync(
        LogicPOSDbContext database,
        CancellationToken cancellationToken)
    {
        var location = await database.WarehouseLocations.AsNoTracking()
            .Where(item => item.IsDeleted == false)
            .OrderByDescending(item => item.IsDefault)
            .ThenBy(item => item.Designation)
            .FirstOrDefaultAsync(cancellationToken);
        return location?.Id;
    }
}
