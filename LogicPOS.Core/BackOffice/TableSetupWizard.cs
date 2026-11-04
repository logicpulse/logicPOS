using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.BackOffice;

public interface ITableSetupWizard
{
    Task<TableSetupDraft> LoadAsync(CancellationToken cancellationToken = default);

    Task<TableSetupResult> CreateAsync(TableSetupRequest request, CancellationToken cancellationToken = default);
}

public sealed class TableSetupDraft
{
    public string PlaceDesignation { get; init; } = "Sala";

    public string TableDesignation { get; init; } = "Mesa 1";

    public bool HasPlace { get; init; }

    public Guid? ExistingPlaceId { get; init; }

    public string? ExistingPlaceLabel { get; init; }

    public string? BlockedReason { get; init; }
}

public sealed class TableSetupRequest
{
    public string PlaceDesignation { get; init; } = string.Empty;

    public string TableDesignation { get; init; } = string.Empty;

    public Guid? ExistingPlaceId { get; init; }
}

public sealed class TableSetupResult
{
    public bool Ok { get; init; }

    public string Message { get; init; } = string.Empty;

    public static TableSetupResult Done(string message) => new() { Ok = true, Message = message };

    public static TableSetupResult Fail(string message) => new() { Ok = false, Message = message };
}

public sealed class LocalTableSetupWizard : ITableSetupWizard
{
    private readonly IServiceScopeFactory _scopes;
    private readonly IBackOfficeListingService _listing;

    public LocalTableSetupWizard(IServiceScopeFactory scopes, IBackOfficeListingService listing)
    {
        _scopes = scopes;
        _listing = listing;
    }

    public async Task<TableSetupDraft> LoadAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var place = await database.Places.AsNoTracking()
            .Where(item => item.IsDeleted == false)
            .OrderBy(item => item.Order)
            .Select(item => new { item.Id, item.Designation })
            .FirstOrDefaultAsync(cancellationToken);
        var tableCount = await database.Tables.AsNoTracking()
            .CountAsync(item => item.IsDeleted == false, cancellationToken);
        var blocked = await MissingPrerequisiteAsync(database, cancellationToken);
        return new TableSetupDraft
        {
            PlaceDesignation = "Sala",
            TableDesignation = $"Mesa {tableCount + 1}",
            HasPlace = place is not null,
            ExistingPlaceId = place?.Id,
            ExistingPlaceLabel = place?.Designation,
            BlockedReason = blocked
        };
    }

    public async Task<TableSetupResult> CreateAsync(TableSetupRequest request, CancellationToken cancellationToken = default)
    {
        var tableName = request.TableDesignation.Trim();
        if (string.IsNullOrWhiteSpace(tableName))
        {
            return TableSetupResult.Fail("Indique a designação da mesa.");
        }

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var blocked = await MissingPrerequisiteAsync(database, cancellationToken);
        if (blocked is not null)
        {
            return TableSetupResult.Fail(blocked);
        }

        Guid placeId;
        if (request.ExistingPlaceId is Guid existing && existing != Guid.Empty
            && await database.Places.AsNoTracking().AnyAsync(item => item.IsDeleted == false && item.Id == existing, cancellationToken))
        {
            placeId = existing;
        }
        else
        {
            var placeName = request.PlaceDesignation.Trim();
            if (string.IsNullOrWhiteSpace(placeName))
            {
                return TableSetupResult.Fail("Indique a designação do local.");
            }

            var place = await _listing.SaveAsync(
                "Locais",
                null,
                new Dictionary<string, string>
                {
                    ["Designation"] = placeName
                },
                cancellationToken);
            if (place.Succeeded == false)
            {
                return TableSetupResult.Fail(place.Error ?? "Não foi possível criar o local.");
            }

            placeId = place.Id;
        }

        var table = await _listing.SaveAsync(
            "Mesas",
            null,
            new Dictionary<string, string>
            {
                ["Designation"] = tableName,
                ["PlaceId"] = placeId.ToString()
            },
            cancellationToken);
        if (table.Succeeded == false)
        {
            return TableSetupResult.Fail(table.Error ?? "Não foi possível criar a mesa.");
        }

        return TableSetupResult.Done($"Mesa «{tableName}» criada.");
    }

    private static async Task<string?> MissingPrerequisiteAsync(LogicPOSDbContext database, CancellationToken cancellationToken)
    {
        if (await database.PriceTypes.AsNoTracking().AnyAsync(item => item.IsDeleted == false, cancellationToken) == false)
        {
            return "Faltam tipos de preço na base.";
        }

        return null;
    }
}
