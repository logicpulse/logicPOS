using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.BackOffice;

public interface IFinalConsumerSetupWizard
{
    Task<FinalConsumerSetupDraft> LoadAsync(CancellationToken cancellationToken = default);

    Task<FinalConsumerSetupResult> CreateAsync(FinalConsumerSetupRequest request, CancellationToken cancellationToken = default);
}

public sealed class FinalConsumerSetupDraft
{
    public string Name { get; init; } = "Consumidor Final";

    public string FiscalNumber { get; init; } = "999999990";

    public bool AlreadyExists { get; init; }

    public string? BlockedReason { get; init; }
}

public sealed class FinalConsumerSetupRequest
{
    public string Name { get; init; } = "Consumidor Final";

    public string FiscalNumber { get; init; } = "999999990";
}

public sealed class FinalConsumerSetupResult
{
    public bool Ok { get; init; }

    public string Message { get; init; } = string.Empty;

    public static FinalConsumerSetupResult Done(string message) => new() { Ok = true, Message = message };

    public static FinalConsumerSetupResult Fail(string message) => new() { Ok = false, Message = message };
}

public sealed class LocalFinalConsumerSetupWizard : IFinalConsumerSetupWizard
{
    private const string DefaultName = "Consumidor Final";
    private const string DefaultFiscalNumber = "999999990";

    private readonly IServiceScopeFactory _scopes;
    private readonly IBackOfficeListingService _listing;

    public LocalFinalConsumerSetupWizard(IServiceScopeFactory scopes, IBackOfficeListingService listing)
    {
        _scopes = scopes;
        _listing = listing;
    }

    public async Task<FinalConsumerSetupDraft> LoadAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var exists = await database.Customers.AsNoTracking()
            .AnyAsync(item => item.IsDeleted == false && item.Name == DefaultName, cancellationToken);
        var blocked = await MissingPrerequisiteAsync(database, cancellationToken);
        return new FinalConsumerSetupDraft
        {
            Name = DefaultName,
            FiscalNumber = DefaultFiscalNumber,
            AlreadyExists = exists,
            BlockedReason = blocked
        };
    }

    public async Task<FinalConsumerSetupResult> CreateAsync(FinalConsumerSetupRequest request, CancellationToken cancellationToken = default)
    {
        var name = string.IsNullOrWhiteSpace(request.Name) ? DefaultName : request.Name.Trim();
        var fiscal = string.IsNullOrWhiteSpace(request.FiscalNumber) ? DefaultFiscalNumber : request.FiscalNumber.Trim();

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var blocked = await MissingPrerequisiteAsync(database, cancellationToken);
        if (blocked is not null)
        {
            return FinalConsumerSetupResult.Fail(blocked);
        }

        var existing = await database.Customers.AsNoTracking()
            .FirstOrDefaultAsync(item => item.IsDeleted == false && item.Name == name, cancellationToken);
        if (existing is not null)
        {
            return FinalConsumerSetupResult.Done($"O cliente «{name}» já existe.");
        }

        var saved = await _listing.SaveAsync(
            "Clientes",
            null,
            new Dictionary<string, string>
            {
                ["Name"] = name,
                ["FiscalNumber"] = fiscal
            },
            cancellationToken);
        if (saved.Succeeded == false)
        {
            return FinalConsumerSetupResult.Fail(saved.Error ?? "Não foi possível criar o cliente de balcão.");
        }

        return FinalConsumerSetupResult.Done($"Cliente «{name}» criado.");
    }

    private static async Task<string?> MissingPrerequisiteAsync(LogicPOSDbContext database, CancellationToken cancellationToken)
    {
        if (await database.CustomerTypes.AsNoTracking().AnyAsync(item => item.IsDeleted == false, cancellationToken) == false)
        {
            return "Faltam tipos de cliente na base.";
        }

        if (await database.PriceTypes.AsNoTracking().AnyAsync(item => item.IsDeleted == false, cancellationToken) == false)
        {
            return "Faltam tipos de preço na base.";
        }

        if (await database.Countries.AsNoTracking().AnyAsync(item => item.IsDeleted == false, cancellationToken) == false)
        {
            return "Faltam países na base.";
        }

        return null;
    }
}
