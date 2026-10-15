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

        var familiesTask = database.ArticleFamilies
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

        var subfamiliesTask = database.ArticleSubfamilies
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

        var articlesTask = database.Articles
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

        await Task.WhenAll(familiesTask, subfamiliesTask, articlesTask);
        var families = await familiesTask;
        var subfamilies = await subfamiliesTask;
        var articles = await articlesTask;

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

    private static string LabelOrDesignation(string? label, string designation)
    {
        return string.IsNullOrWhiteSpace(label) ? designation : label;
    }
}
