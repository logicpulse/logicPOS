using System.Collections.Concurrent;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.FrontOffice;

public sealed class PosCatalogService : IPosCatalogService
{
    private const int MaxCacheEntries = 500;

    private readonly IServiceScopeFactory _scopes;
    private readonly ConcurrentDictionary<(ButtonImageKind Kind, Guid Id), string?> _imageCache = new();
    private readonly SemaphoreSlim _loadSemaphore = new(1);
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
        if (_imageCache.TryGetValue((kind, id), out var cached))
        {
            return cached;
        }

        var image = await LoadImageFromDbAsync(kind, id, cancellationToken);
        _imageCache[(kind, id)] = image;
        EvictIfNeeded();
        return image;
    }

    public async Task<PosButtonImages> GetButtonImagesAsync(
        IReadOnlyCollection<Guid> familyIds,
        IReadOnlyCollection<Guid> subfamilyIds,
        IReadOnlyCollection<Guid> articleIds,
        CancellationToken cancellationToken = default)
    {
        var families = Missing(ButtonImageKind.Family, familyIds);
        var subfamilies = Missing(ButtonImageKind.Subfamily, subfamilyIds);
        var articles = Missing(ButtonImageKind.Article, articleIds);
        if (families.Count > 0 || subfamilies.Count > 0 || articles.Count > 0)
        {
            await _loadSemaphore.WaitAsync(cancellationToken);
            try
            {
                families = Missing(ButtonImageKind.Family, familyIds);
                subfamilies = Missing(ButtonImageKind.Subfamily, subfamilyIds);
                articles = Missing(ButtonImageKind.Article, articleIds);
                await using var scope = _scopes.CreateAsyncScope();
                var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
                if (families.Count > 0)
                {
                    var rows = await database.ArticleFamilies.AsNoTracking()
                        .Where(item => families.Contains(item.Id))
                        .Select(item => new { item.Id, Image = item.Button == null ? null : item.Button.Image })
                        .ToListAsync(cancellationToken);
                    Remember(ButtonImageKind.Family, rows.Select(item => (item.Id, item.Image)), families);
                }

                if (subfamilies.Count > 0)
                {
                    var rows = await database.ArticleSubfamilies.AsNoTracking()
                        .Where(item => subfamilies.Contains(item.Id))
                        .Select(item => new { item.Id, Image = item.Button == null ? null : item.Button.Image })
                        .ToListAsync(cancellationToken);
                    Remember(ButtonImageKind.Subfamily, rows.Select(item => (item.Id, item.Image)), subfamilies);
                }

                if (articles.Count > 0)
                {
                    var rows = await database.Articles.AsNoTracking()
                        .Where(item => articles.Contains(item.Id))
                        .Select(item => new { item.Id, item.Button.Image })
                        .ToListAsync(cancellationToken);
                    Remember(ButtonImageKind.Article, rows.Select(item => (item.Id, item.Image)), articles);
                }
            }
            finally
            {
                _loadSemaphore.Release();
            }
        }

        return new PosButtonImages(
            ReadCached(ButtonImageKind.Family, familyIds),
            ReadCached(ButtonImageKind.Subfamily, subfamilyIds),
            ReadCached(ButtonImageKind.Article, articleIds));
    }

    private List<Guid> Missing(ButtonImageKind kind, IReadOnlyCollection<Guid> ids)
    {
        var missing = new List<Guid>();
        foreach (var id in ids)
        {
            if (_imageCache.ContainsKey((kind, id)) == false)
            {
                missing.Add(id);
            }
        }

        return missing;
    }

    private void Remember(ButtonImageKind kind, IEnumerable<(Guid Id, string? Image)> rows, IReadOnlyCollection<Guid> requested)
    {
        var found = new HashSet<Guid>();
        foreach (var row in rows)
        {
            found.Add(row.Id);
            _imageCache[(kind, row.Id)] = row.Image;
        }

        foreach (var id in requested)
        {
            if (found.Contains(id) == false)
            {
                _imageCache[(kind, id)] = null;
            }
        }

        EvictIfNeeded();
    }

    private Dictionary<Guid, string?> ReadCached(ButtonImageKind kind, IReadOnlyCollection<Guid> ids)
    {
        var images = new Dictionary<Guid, string?>();
        foreach (var id in ids)
        {
            if (_imageCache.TryGetValue((kind, id), out var image))
            {
                images[id] = image;
            }
        }

        return images;
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
                    .Where(f => f.Id == id)
                    .Select(f => f.Button == null ? null : f.Button.Image)
                    .FirstOrDefaultAsync(cancellationToken),

                ButtonImageKind.Subfamily => await database.ArticleSubfamilies
                    .AsNoTracking()
                    .Where(s => s.Id == id)
                    .Select(s => s.Button == null ? null : s.Button.Image)
                    .FirstOrDefaultAsync(cancellationToken),

                ButtonImageKind.Article => await database.Articles
                    .AsNoTracking()
                    .Where(a => a.Id == id)
                    .Select(a => a.Button.Image)
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
