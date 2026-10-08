using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.FrontOffice;

public sealed class PosCatalogService : IPosCatalogService
{
    private readonly IServiceScopeFactory _scopes;

    public PosCatalogService(IServiceScopeFactory scopes)
    {
        _scopes = scopes;
    }

    public async Task<PosCatalog> LoadAsync(CancellationToken cancellationToken = default)
    {
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
                Label = family.Button != null ? family.Button.Label : null,
                Image = family.Button != null ? family.Button.Image : null
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
                Label = subfamily.Button != null ? subfamily.Button.Label : null,
                Image = subfamily.Button != null ? subfamily.Button.Image : null
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
                Vat = article.VatDirectSelling != null ? article.VatDirectSelling.Value : 0m,
                Label = article.Button != null ? article.Button.Label : null,
                Image = article.Button != null ? article.Button.Image : null
            })
            .ToListAsync(cancellationToken);

        return new PosCatalog(
            families.Select(item => new PosMenuItem(
                item.Id,
                Guid.Empty,
                LabelOrDesignation(item.Label, item.Designation),
                false,
                item.Image)).ToList(),
            subfamilies.Select(item => new PosMenuItem(
                item.Id,
                item.FamilyId,
                LabelOrDesignation(item.Label, item.Designation),
                false,
                item.Image)).ToList(),
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
                item.Image,
                item.PVPVariable)).ToList());
    }

    private static string LabelOrDesignation(string? label, string designation)
    {
        return string.IsNullOrWhiteSpace(label) ? designation : label;
    }
}
