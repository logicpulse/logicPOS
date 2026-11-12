using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.BackOffice;

/// <summary>
/// Applies the same values to many rows of a listing. Only the keys passed in <c>changes</c> are written.
/// </summary>
public interface IBulkEditService
{
    bool Supports(string title);

    IReadOnlyList<ListingField> Fields(string title);

    Task<ListingSaveResult> ApplyAsync(string title, IReadOnlyCollection<Guid> ids, IReadOnlyDictionary<string, string> changes, CancellationToken cancellationToken = default);
}

public sealed class BulkEditService : IBulkEditService
{
    /// <summary>Sets both <see cref="Article.VatOnTableId"/> and <see cref="Article.VatDirectSellingId"/>.</summary>
    public const string ArticleVatKey = "VatRateId";

    private static readonly IReadOnlyList<ListingField> ArticleFields =
    [
        new() { Key = ArticleVatKey, Label = "Taxa de IVA (mesa e venda direta)", Kind = "lookup", Lookup = nameof(VatRate), Required = true, Group = "Impostos" },
        new() { Key = "VatExemptionReasonId", Label = "Motivo de isenção de IVA", Kind = "lookup", Lookup = nameof(VatExemptionReason), Required = true, Group = "Impostos" },
        new() { Key = "SubfamilyId", Label = "Subfamília (define a família)", Kind = "lookup", Lookup = nameof(ArticleSubfamily), Required = true, Group = "Classificação" },
        new() { Key = "TypeId", Label = "Tipo de artigo", Kind = "lookup", Lookup = nameof(ArticleType), Required = true, Group = "Classificação" },
        new() { Key = "ClassId", Label = "Classe do artigo", Kind = "lookup", Lookup = nameof(ArticleClass), Required = true, Group = "Classificação" },
        new() { Key = "MeasurementUnitId", Label = "Unidade de medida", Kind = "lookup", Lookup = nameof(MeasurementUnit), Required = true, Group = "Classificação" },
        new() { Key = "PriceWithVat", Label = "Preço com imposto", Kind = "bool", Group = "Outros" },
        new() { Key = "Favorite", Label = "Favorito", Kind = "bool", Group = "Outros" },
        new() { Key = "Discount", Label = "Desconto (%)", Kind = "decimal", Group = "Outros" }
    ];

    private readonly IServiceScopeFactory _scopes;
    private readonly IBackOfficeListingService _listing;

    public BulkEditService(IServiceScopeFactory scopes, IBackOfficeListingService listing)
    {
        _scopes = scopes;
        _listing = listing;
    }

    public bool Supports(string title) => Fields(title).Count > 0;

    public IReadOnlyList<ListingField> Fields(string title) => title switch
    {
        "Artigos" => ArticleFields,
        _ => []
    };

    public async Task<ListingSaveResult> ApplyAsync(string title, IReadOnlyCollection<Guid> ids, IReadOnlyDictionary<string, string> changes, CancellationToken cancellationToken = default)
    {
        if (Supports(title) == false)
        {
            return ListingSaveResult.Fail("Esta listagem não permite alterar em massa.");
        }

        var allowed = await _listing.AllowedMenuTitlesAsync(cancellationToken);
        if (allowed is not null && allowed.Contains(title) == false)
        {
            return ListingSaveResult.Fail("Não tem permissão para alterar estes registos.");
        }

        var targets = ids.Where(id => id != Guid.Empty).Distinct().ToList();
        if (targets.Count == 0)
        {
            return ListingSaveResult.Fail("Selecione pelo menos um registo.");
        }

        if (changes.Count == 0)
        {
            return ListingSaveResult.Fail("Marque pelo menos um campo a alterar.");
        }

        var unknown = changes.Keys.FirstOrDefault(key => Fields(title).Any(field => field.Key == key) == false);
        if (unknown is not null)
        {
            return ListingSaveResult.Fail($"Campo não suportado: {unknown}.");
        }

        try
        {
            await using var scope = _scopes.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
            return title switch
            {
                "Artigos" => await ApplyArticlesAsync(database, targets, changes, cancellationToken),
                _ => ListingSaveResult.Fail("Esta listagem não permite alterar em massa.")
            };
        }
        catch (Exception exception)
        {
            return ListingSaveResult.Fail(SaveExceptionText.Describe(exception));
        }
    }

    private static async Task<ListingSaveResult> ApplyArticlesAsync(
        LogicPOSDbContext database,
        IReadOnlyList<Guid> ids,
        IReadOnlyDictionary<string, string> changes,
        CancellationToken cancellationToken)
    {
        VatRate? vat = null;
        if (changes.TryGetValue(ArticleVatKey, out var vatText))
        {
            vat = await FindAsync(database.VatRates, vatText, cancellationToken);
            if (vat is null)
            {
                return ListingSaveResult.Fail("Indique a taxa de IVA.");
            }
        }

        VatExemptionReason? reason = null;
        var reasonTicked = changes.TryGetValue("VatExemptionReasonId", out var reasonText);
        if (reasonTicked && string.IsNullOrWhiteSpace(reasonText) == false)
        {
            reason = await FindAsync(database.VatExemptionReasons, reasonText, cancellationToken);
            if (reason is null)
            {
                return ListingSaveResult.Fail("Motivo de isenção de IVA inválido.");
            }
        }

        // Same rule as the article editor: a 0% rate needs an exemption reason.
        // Non-zero rates clear the reason; an empty «Alterar» on the reason just clears it.
        if (vat is not null && vat.Value == 0 && reason is null)
        {
            return ListingSaveResult.Fail("A taxa de IVA é 0%. Indique o motivo de isenção de IVA.");
        }

        var subfamily = await ReferenceAsync(database.ArticleSubfamilies, changes, "SubfamilyId", cancellationToken);
        var type = await ReferenceAsync(database.ArticleTypes, changes, "TypeId", cancellationToken);
        var articleClass = await ReferenceAsync(database.ArticleClasses, changes, "ClassId", cancellationToken);
        var unit = await ReferenceAsync(database.MeasurementUnits, changes, "MeasurementUnitId", cancellationToken);
        var missing = subfamily == Guid.Empty ? "a subfamília"
            : type == Guid.Empty ? "o tipo de artigo"
            : articleClass == Guid.Empty ? "a classe do artigo"
            : unit == Guid.Empty ? "a unidade de medida"
            : null;
        if (missing is not null)
        {
            return ListingSaveResult.Fail($"Indique {missing}.");
        }

        decimal? discount = null;
        if (changes.TryGetValue("Discount", out var discountText))
        {
            if (FormDecimal.TryParse(discountText, out var parsed) == false || parsed < 0 || parsed > 100)
            {
                return ListingSaveResult.Fail("O desconto tem de estar entre 0 e 100.");
            }

            discount = parsed;
        }

        var rates = await database.VatRates.AsNoTracking()
            .ToDictionaryAsync(item => item.Id, item => item.Value, cancellationToken);
        var articles = await database.Articles
            .Where(item => item.IsDeleted == false && ids.Contains(item.Id))
            .ToListAsync(cancellationToken);
        if (articles.Count == 0)
        {
            return ListingSaveResult.Fail("Nenhum artigo encontrado.");
        }

        var now = DateTime.Now;
        foreach (var article in articles)
        {
            if (vat is not null)
            {
                article.VatOnTableId = vat.Id;
                article.VatDirectSellingId = vat.Id;
            }

            // The reason only belongs to articles whose resulting VAT is 0%; any other article loses it.
            if (vat is not null || reasonTicked)
            {
                var zero = (article.VatOnTableId is Guid table && rates.TryGetValue(table, out var tableRate) && tableRate == 0)
                    || (rates.TryGetValue(article.VatDirectSellingId, out var directRate) && directRate == 0);
                article.VatExemptionReasonId = zero ? reason?.Id ?? article.VatExemptionReasonId : null;
            }

            if (subfamily is Guid subfamilyId)
            {
                article.SubfamilyId = subfamilyId;
            }

            if (type is Guid typeId)
            {
                article.TypeId = typeId;
            }

            if (articleClass is Guid classId)
            {
                article.ClassId = classId;
            }

            if (unit is Guid unitId)
            {
                article.MeasurementUnitId = unitId;
            }

            if (changes.TryGetValue("PriceWithVat", out var priceWithVat))
            {
                article.PriceWithVat = IsOn(priceWithVat);
            }

            if (changes.TryGetValue("Favorite", out var favorite))
            {
                article.Favorite = IsOn(favorite);
            }

            if (discount is decimal value)
            {
                article.Discount = value;
            }

            article.UpdatedAt = now;
        }

        // Npgsql/SQL Server retry strategies forbid user transactions unless wrapped.
        var strategy = database.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await database.Database.BeginTransactionAsync(cancellationToken);
            await database.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        });

        return ListingSaveResult.OkMany(articles.Select(article => article.Id).ToList(), $"{articles.Count} artigo(s) atualizado(s).");
    }

    /// <summary>Null when the key is not ticked; <see cref="Guid.Empty"/> when ticked but not a live row.</summary>
    private static async Task<Guid?> ReferenceAsync<T>(DbSet<T> set, IReadOnlyDictionary<string, string> changes, string key, CancellationToken cancellationToken)
        where T : Entity
    {
        if (changes.TryGetValue(key, out var text) == false)
        {
            return null;
        }

        return await FindAsync(set, text, cancellationToken) is { } row ? row.Id : Guid.Empty;
    }

    private static async Task<T?> FindAsync<T>(DbSet<T> set, string? text, CancellationToken cancellationToken)
        where T : Entity
    {
        if (Guid.TryParse(text, out var id) == false || id == Guid.Empty)
        {
            return null;
        }

        return await set.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id && item.IsDeleted == false, cancellationToken);
    }

    private static bool IsOn(string value) => string.Equals(value.Trim(), "true", StringComparison.OrdinalIgnoreCase);
}
