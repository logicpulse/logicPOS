using System.Globalization;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.BackOffice;

public interface IArticleSetupWizard
{
    Task<ArticleSetupDraft> LoadAsync(CancellationToken cancellationToken = default);

    Task<ArticleSetupResult> CreateAsync(ArticleSetupRequest request, CancellationToken cancellationToken = default);
}

public sealed class ArticleSetupDraft
{
    public string FamilyDesignation { get; init; } = "Geral";

    public string SubfamilyDesignation { get; init; } = "Geral";

    public string ArticleDesignation { get; init; } = string.Empty;

    public string Price { get; init; } = "1,00";

    public string? BlockedReason { get; init; }
}

public sealed class ArticleSetupRequest
{
    public string FamilyDesignation { get; init; } = string.Empty;

    public string SubfamilyDesignation { get; init; } = string.Empty;

    public string ArticleDesignation { get; init; } = string.Empty;

    public string Price { get; init; } = string.Empty;
}

public sealed class ArticleSetupResult
{
    public bool Ok { get; init; }

    public string Message { get; init; } = string.Empty;

    public static ArticleSetupResult Done(string message) => new() { Ok = true, Message = message };

    public static ArticleSetupResult Fail(string message) => new() { Ok = false, Message = message };
}

public sealed class LocalArticleSetupWizard : IArticleSetupWizard
{
    private readonly IServiceScopeFactory _scopes;
    private readonly IBackOfficeListingService _listing;

    public LocalArticleSetupWizard(IServiceScopeFactory scopes, IBackOfficeListingService listing)
    {
        _scopes = scopes;
        _listing = listing;
    }

    public async Task<ArticleSetupDraft> LoadAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var blocked = await MissingPrerequisiteAsync(database, cancellationToken);
        return new ArticleSetupDraft
        {
            FamilyDesignation = "Geral",
            SubfamilyDesignation = "Geral",
            ArticleDesignation = string.Empty,
            Price = "1,00",
            BlockedReason = blocked
        };
    }

    public async Task<ArticleSetupResult> CreateAsync(ArticleSetupRequest request, CancellationToken cancellationToken = default)
    {
        var familyName = request.FamilyDesignation.Trim();
        var subfamilyName = request.SubfamilyDesignation.Trim();
        var articleName = request.ArticleDesignation.Trim();
        if (string.IsNullOrWhiteSpace(familyName))
        {
            return ArticleSetupResult.Fail("Indique a designação da família.");
        }

        if (string.IsNullOrWhiteSpace(subfamilyName))
        {
            return ArticleSetupResult.Fail("Indique a designação da subfamília.");
        }

        if (string.IsNullOrWhiteSpace(articleName))
        {
            return ArticleSetupResult.Fail("Indique a designação do artigo.");
        }

        if (FormDecimal.TryParse(request.Price, FormDecimalKind.Money, out var price) == false || price <= 0)
        {
            return ArticleSetupResult.Fail("Indique um preço válido maior do que zero.");
        }

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var blocked = await MissingPrerequisiteAsync(database, cancellationToken);
        if (blocked is not null)
        {
            return ArticleSetupResult.Fail(blocked);
        }

        await EnsureDefaultArticleTypeAsync(database, cancellationToken);

        var button = TruncateButton(articleName);
        var family = await _listing.SaveAsync(
            "Famílias",
            null,
            new Dictionary<string, string>
            {
                ["Designation"] = familyName,
                ["ButtonLabel"] = TruncateButton(familyName)
            },
            cancellationToken);
        if (family.Succeeded == false)
        {
            return ArticleSetupResult.Fail(family.Error ?? "Não foi possível criar a família.");
        }

        var subfamily = await _listing.SaveAsync(
            "Subfamílias",
            null,
            new Dictionary<string, string>
            {
                ["Designation"] = subfamilyName,
                ["FamilyId"] = family.Id.ToString(),
                ["ButtonLabel"] = TruncateButton(subfamilyName)
            },
            cancellationToken);
        if (subfamily.Succeeded == false)
        {
            return ArticleSetupResult.Fail(subfamily.Error ?? "Não foi possível criar a subfamília.");
        }

        var article = await _listing.SaveAsync(
            "Artigos",
            null,
            new Dictionary<string, string>
            {
                ["Designation"] = articleName,
                ["ButtonLabel"] = button,
                ["SubfamilyId"] = subfamily.Id.ToString(),
                ["Price1"] = price.ToString(CultureInfo.InvariantCulture)
            },
            cancellationToken);
        if (article.Succeeded == false)
        {
            return ArticleSetupResult.Fail(article.Error ?? "Não foi possível criar o artigo.");
        }

        return ArticleSetupResult.Done($"Artigo «{articleName}» criado com a família e a subfamília.");
    }

    private static async Task<string?> MissingPrerequisiteAsync(LogicPOSDbContext database, CancellationToken cancellationToken)
    {
        if (await database.ArticleClasses.AsNoTracking().AnyAsync(item => item.IsDeleted == false, cancellationToken) == false)
        {
            return "Faltam classes de artigo na base. Reinstale ou corrija a semente obrigatória.";
        }

        if (await database.MeasurementUnits.AsNoTracking().AnyAsync(item => item.IsDeleted == false, cancellationToken) == false)
        {
            return "Faltam unidades de medida na base.";
        }

        if (await database.SizeUnits.AsNoTracking().AnyAsync(item => item.IsDeleted == false, cancellationToken) == false)
        {
            return "Faltam unidades de tamanho na base.";
        }

        if (await database.VatRates.AsNoTracking().AnyAsync(item => item.IsDeleted == false, cancellationToken) == false)
        {
            return "Faltam taxas de IVA na base.";
        }

        return null;
    }

    private static async Task EnsureDefaultArticleTypeAsync(LogicPOSDbContext database, CancellationToken cancellationToken)
    {
        if (await database.ArticleTypes.AnyAsync(item => item.IsDeleted == false, cancellationToken))
        {
            return;
        }

        database.ArticleTypes.Add(new ArticleType
        {
            Id = Guid.NewGuid(),
            Order = 1,
            Code = "1",
            Designation = "Normal",
            HasPrice = true,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        });
        await database.SaveChangesAsync(cancellationToken);
    }

    private static string TruncateButton(string value)
    {
        var text = value.Trim();
        return text.Length <= 35 ? text : text[..35];
    }
}
