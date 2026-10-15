using System.Collections.Concurrent;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.FrontOffice;

public sealed class PosCatalogService : IPosCatalogService
{
    private const int MaxCacheEntries = 500;

    private readonly IServiceScopeFactory _scopes;
    private readonly ConcurrentDictionary<(ButtonImageKind Kind, Guid Id), Lazy<Task<string?>>> _imageCache = new();
    private readonly SemaphoreSlim _loadSemaphore = new(4);
    private readonly object _evictionLock = new();

    public PosCatalogService(IServiceScopeFactory scopes)
    {
        _scopes = scopes;
    }

    public async Task<PosCatalog> LoadAsync(CancellationToken cancellationToken = default)
    {
        ClearImageCache();

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();

        var families = await database.ArticleFamilies
            .AsNoTracking()
            .Where(family => family.IsDeleted == false)
            .OrderBy(family => family.Order)
            .ThenBy(family => family.Designation)
            .Select(family => new
            {
                family.Id,
                family.Designation,
                Label = family.Button != null ? family.Button.Label : null
            })
            .ToListAsync(cancellationToken);

        var subfamilies = await database.ArticleSubfamilies
            .AsNoTracking()
            .Where(subfamily => subfamily.IsDeleted == false)
            .OrderBy(subfamily => subfamily.Order)
            .ThenBy(subfamily => subfamily.Designation)
            .Select(subfamily => new
            {
                subfamily.Id,
                subfamily.FamilyId,
                subfamily.Designation,
                Label = subfamily.Button != null ? subfamily.Button.Label : null
            })
            .ToListAsync(cancellationToken);

        var articles = await database.Articles
            .AsNoTracking()
            .Where(article => article.IsDeleted == false)
            .OrderBy(article => article.Order)
            .ThenBy(article => article.Designation)
            .Select(article => new
            {
                article.Id,
                article.SubfamilyId,
                article.Designation,
                article.Code,
                article.Barcode,
                article.Favorite,
                article.PriceWithVat,
                article.PVPVariable,
                article.DefaultQuantity,
                article.Discount,
                Price = article.Price1.Value,
                article.VatDirectSellingId,
                article.VatExemptionReasonId,
                Vat = article.VatDirectSelling != null ? article.VatDirectSelling.Value : 0m,
                Label = article.Button != null ? article.Button.Label : null
            })
            .ToListAsync(cancellationToken);

        return new PosCatalog(
            families.Select(item => new PosMenuItem(
                item.Id,
                Guid.Empty,
                LabelOrDesignation(item.Label, item.Designation),
                false)).ToList(),
            subfamilies.Select(item => new PosMenuItem(
                item.Id,
                item.FamilyId,
                LabelOrDesignation(item.Label, item.Designation),
                false)).ToList(),
            articles.Select(item => new PosArticle(
                item.Id,
                item.SubfamilyId,
                LabelOrDesignation(item.Label, item.Designation),
                item.Designation,
                item.Code,
                item.Barcode,
                item.Favorite,
                item.Price,
                item.PriceWithVat,
                item.Vat,
                item.DefaultQuantity,
                item.Discount,
                item.VatDirectSellingId,
                imageBase64: null,
                item.PVPVariable,
                item.VatExemptionReasonId)).ToList());
    }

    public async Task<string?> GetButtonImageAsync(ButtonImageKind kind, Guid id, CancellationToken cancellationToken = default)
    {
        var key = (kind, id);

        var lazyTask = _imageCache.GetOrAdd(
            key,
            k => new Lazy<Task<string?>>(() => LoadImageFromDbAsync(k.Kind, k.Id, CancellationToken.None)));

        EvictIfNeeded();

        try
        {
            return await lazyTask.Value.WaitAsync(cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            _imageCache.TryRemove(KeyValuePair.Create(key, lazyTask));
            throw;
        }
    }

    public void ClearImageCache()
    {
        _imageCache.Clear();
    }

    private async Task<string?> LoadImageFromDbAsync(ButtonImageKind kind, Guid id, CancellationToken cancellationToken)
    {
        await _loadSemaphore.WaitAsync(cancellationToken);
        try
        {
            await using var scope = _scopes.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();

            return kind switch
            {
                ButtonImageKind.Family => await database.ArticleFamilies
                    .AsNoTracking()
                    .Where(f => f.Id == id && f.Button != null)
                    .Select(f => f.Button!.Image)
                    .FirstOrDefaultAsync(cancellationToken),

                ButtonImageKind.Subfamily => await database.ArticleSubfamilies
                    .AsNoTracking()
                    .Where(s => s.Id == id && s.Button != null)
                    .Select(s => s.Button!.Image)
                    .FirstOrDefaultAsync(cancellationToken),

                ButtonImageKind.Article => await database.Articles
                    .AsNoTracking()
                    .Where(a => a.Id == id && a.Button != null)
                    .Select(a => a.Button!.Image)
                    .FirstOrDefaultAsync(cancellationToken),

                _ => null
            };
        }
        finally
        {
            _loadSemaphore.Release();
        }
    }

    private void EvictIfNeeded()
    {
        if (_imageCache.Count <= MaxCacheEntries)
        {
            return;
        }

        lock (_evictionLock)
        {
            if (_imageCache.Count <= MaxCacheEntries)
            {
                return;
            }

            var toRemove = _imageCache.Count - MaxCacheEntries + 50;
            foreach (var key in _imageCache.Keys.Take(toRemove))
            {
                _imageCache.TryRemove(key, out _);
            }
        }
    }

    private static string LabelOrDesignation(string? label, string designation)
    {
        return string.IsNullOrWhiteSpace(label) ? designation : label;
    }
}

public enum ButtonImageKind : byte
{
    Family = 1,
    Subfamily = 2,
    Article = 3
}
