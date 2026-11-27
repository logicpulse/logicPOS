using System.Text.Json;
using System.Text.Json.Nodes;
using LogicPOS.Application.Features.System;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Persistence.Cloud;
using LogicPOS.Domain.ValueObjects;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.BackOffice;

/// <summary>
/// First entry (primeira entrada): one confirmation of operation mode, demo choice and company data.
/// Pending while no confirmation mark exists and the company is still unset.
/// A demo sample loaded before the choice does not close it. A fiscal year does: that database is already in use.
/// </summary>
public sealed class FirstEntryService : IFirstEntryService
{
    public const string ConfirmedToken = "FIRST_ENTRY_CONFIRMED";
    public const string OperationModeToken = "SYSTEM_OPERATION_MODE";

    /// <summary>Modules the seeder knows about (see <c>DatabaseSeeder</c>).</summary>
    public static readonly IReadOnlyList<string> ValidModes =
    [
        "default", "parking", "bakery", "butchery", "cafe", "clothingstore", "hardwarestore",
        "restaurant", "seafoodstore", "shoestore"
    ];

    private readonly IServiceScopeFactory _scopes;
    private readonly LocalCompanySetupService _companySetup;
    private readonly IConfiguration _configuration;
    private readonly string _baseDirectory;
    private readonly Func<IServiceProvider, CancellationToken, Task<bool>> _loadDemoSample;
    private readonly IFirstEntryAccountContext _account;

    // Serialises confirmations inside this process so two open first entries cannot both pass the check.
    private readonly SemaphoreSlim _confirmLock = new(1, 1);

    /// <param name="loadDemoSample">
    /// Loads the demo sample inside the confirmation transaction and returns false (or throws) on failure.
    /// Defaults to <see cref="IDatabaseSeeder.ApplyDemoSample"/>; tests replace it to force a failure.
    /// </param>
    /// <param name="account">Cloud account adapter; defaults to the local edition (no account).</param>
    public FirstEntryService(
        IServiceScopeFactory scopes,
        LocalCompanySetupService companySetup,
        IConfiguration configuration,
        string baseDirectory,
        Func<IServiceProvider, CancellationToken, Task<bool>>? loadDemoSample = null,
        IFirstEntryAccountContext? account = null)
    {
        _scopes = scopes;
        _companySetup = companySetup;
        _configuration = configuration;
        _baseDirectory = baseDirectory;
        _loadDemoSample = loadDemoSample ?? DefaultLoadDemoSample;
        _account = account ?? new LocalFirstEntryAccountContext();
    }

    private static Task<bool> DefaultLoadDemoSample(IServiceProvider services, CancellationToken cancellationToken)
        => Task.FromResult(services.GetRequiredService<IDatabaseSeeder>().ApplyDemoSample());

    public async Task<FirstEntryPending> GetPendingAsync(CancellationToken cancellationToken = default)
    {
        var company = await _companySetup.LoadAsync(cancellationToken);
        var isPending = await IsPendingAsync(cancellationToken);

        // Only an open first entry shows a preselection. Cloud has no installer: the account supplies
        // mode and country, the demo starts refused and the demo-account mark is not read.
        // Local: the installer draft. Invalid values are ignored.
        var cloudPending = _account.IsCloudAccount && isPending;
        var draft = isPending && _account.IsCloudAccount == false ? FirstEntryDraftStore.Read(_baseDirectory) : null;
        var mode = (cloudPending ? _account.AccountOperationMode : draft?.OperationMode)?.Trim().ToLowerInvariant();
        Guid? accountCountry = cloudPending && string.IsNullOrWhiteSpace(_account.AccountCountryCode2) == false
            ? company.Countries.FirstOrDefault(item =>
                string.Equals(item.Code, _account.AccountCountryCode2.Trim(), StringComparison.OrdinalIgnoreCase))?.Id
            : null;
        var countryId = accountCountry
            ?? (draft?.CountryId is { } draftCountry && company.Countries.Any(item => item.Id == draftCountry)
                ? draftCountry
                : (Guid?)null);
        if (cloudPending && countryId is null)
        {
            countryId = company.Countries.FirstOrDefault(item =>
                string.Equals(item.Code, "PT", StringComparison.OrdinalIgnoreCase))?.Id;
        }

        var currencyId = draft?.CurrencyId is { } draftCurrency && company.Currencies.Any(item => item.Id == draftCurrency)
            ? draftCurrency
            : (Guid?)null;
        currencyId ??= await SuggestCurrencyIdAsync(countryId, cancellationToken);
        if (cloudPending && currencyId is null)
        {
            currencyId = company.Currencies.FirstOrDefault(item =>
                string.Equals(item.Code, "EUR", StringComparison.OrdinalIgnoreCase))?.Id;
        }

        return new FirstEntryPending
        {
            IsPending = isPending,
            OperationMode = mode is not null && ValidModes.Contains(mode) ? mode : null,
            DemoAccepted = draft?.DemoAccepted ?? false,
            CountryId = countryId,
            CurrencyId = currencyId,
            FiscalNumber = draft?.FiscalNumber?.Trim() ?? string.Empty,
            Modes = ValidModes,
            Countries = company.Countries,
            Currencies = company.Currencies
        };
    }

    public async Task<bool> TrySaveDraftAsync(FirstEntryDraft draft, CancellationToken cancellationToken = default)
    {
        if (await IsPendingAsync(cancellationToken) == false)
        {
            return false;
        }

        FirstEntryDraftStore.Write(_baseDirectory, draft);
        return true;
    }

    public async Task<FirstEntryConfirmationResult> ConfirmAsync(
        FirstEntryRequest request,
        CancellationToken cancellationToken = default)
    {
        await _confirmLock.WaitAsync(cancellationToken);
        try
        {
            return await ConfirmCoreAsync(request, cancellationToken);
        }
        finally
        {
            _confirmLock.Release();
        }
    }

    private async Task<FirstEntryConfirmationResult> ConfirmCoreAsync(
        FirstEntryRequest request,
        CancellationToken cancellationToken)
    {
        if (await IsPendingAsync(cancellationToken) == false)
        {
            return FirstEntryConfirmationResult.AlreadyConfirmed;
        }

        var mode = request.OperationMode?.Trim().ToLowerInvariant();
        var fiscalNumber = request.FiscalNumber?.Trim();
        if (mode is null
            || ValidModes.Contains(mode) == false
            || request.CountryId is null || request.CountryId == Guid.Empty
            || request.CurrencyId is null || request.CurrencyId == Guid.Empty
            || string.IsNullOrWhiteSpace(fiscalNumber))
        {
            return FirstEntryConfirmationResult.Incomplete;
        }

        var company = await _companySetup.LoadAsync(cancellationToken);
        if (company.Countries.Any(item => item.Id == request.CountryId) == false
            || company.Currencies.Any(item => item.Id == request.CurrencyId) == false)
        {
            return FirstEntryConfirmationResult.Incomplete;
        }

        var countryCode = company.Countries.First(item => item.Id == request.CountryId).Code;
        if (PortugueseNif.AppliesToCountry(countryCode)
            && PortugueseNif.IsValid(fiscalNumber) == false)
        {
            return FirstEntryConfirmationResult.InvalidFiscalNumber;
        }

        // The seeder reads the module from configuration when it is built, so the confirmed mode
        // must be there before the scope below resolves it. Restored unless the confirmation commits.
        var previousModule = _configuration["DatabaseSettings:Module"];
        _configuration["DatabaseSettings:Module"] = mode;
        var committed = false;
        FirstEntryConfirmationResult? stopped = null;
        try
        {
            await using var scope = _scopes.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
            // SqlServerRetryingExecutionStrategy forbids BeginTransaction unless wrapped.
            var strategy = database.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                stopped = null;
                database.ChangeTracker.Clear();
                await using var transaction = await database.Database.BeginTransactionAsync(cancellationToken);

                var saved = await _companySetup.SaveAsync(
                    database,
                    new CompanySetupRequest
                    {
                        CountryId = request.CountryId.Value,
                        CurrencyId = request.CurrencyId.Value,
                        Values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                        {
                            ["COMPANY_FISCALNUMBER"] = fiscalNumber
                        }
                    },
                    cancellationToken);
                if (saved.Succeeded == false)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    stopped = string.Equals(
                        saved.ErrorCode,
                        ListingSaveErrorCodes.InvalidFiscalNumberCode,
                        StringComparison.Ordinal)
                        ? FirstEntryConfirmationResult.InvalidFiscalNumber
                        : FirstEntryConfirmationResult.Incomplete;
                    return;
                }

                await UpsertPreferenceAsync(database, OperationModeToken, mode, 90, cancellationToken);
                await UpsertPreferenceAsync(database, ConfirmedToken, "1", 91, cancellationToken);
                await database.SaveChangesAsync(cancellationToken);

                var countryCode = await database.Countries.AsNoTracking()
                    .Where(item => item.Id == request.CountryId)
                    .Select(item => item.Code2)
                    .FirstOrDefaultAsync(cancellationToken);
                scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>()
                    .PrepareConfirmedEntry(mode, countryCode);

                if (request.DemoAccepted)
                {
                    var loaded = false;
                    try
                    {
                        loaded = await _loadDemoSample(scope.ServiceProvider, cancellationToken);
                    }
                    catch (Exception exception) when (exception is not OperationCanceledException)
                    {
                        AppLogging.Error(exception, "First entry: demo sample failed to load; confirmation rolled back.");
                    }

                    if (loaded == false)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        stopped = FirstEntryConfirmationResult.DemoFailed;
                        return;
                    }
                }

                await transaction.CommitAsync(cancellationToken);
                committed = true;
            });
        }
        finally
        {
            if (committed == false)
            {
                _configuration["DatabaseSettings:Module"] = previousModule;
            }
        }

        if (stopped is not null)
        {
            return stopped.Value;
        }

        // Cloud: the account is the shared source of the mode; no local appsettings to write.
        if (_account.IsCloudAccount == false)
        {
            WriteModuleToAppSettings(mode);
        }

        await using (var reloadScope = _scopes.CreateAsyncScope())
        {
            AppComposition.Services?.GetService<CompanyCurrency>()?.Reload(
                reloadScope.ServiceProvider.GetRequiredService<LogicPOSDbContext>());
        }

        if (_account.IsCloudAccount)
        {
            // The confirmation is already committed. If the account write fails the mode stays in the
            // database and GetResolvedOperationModeAsync falls back to it; the failure is logged.
            try
            {
                await _account.SetAccountOperationModeAsync(mode, cancellationToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                AppLogging.Error(exception, "First entry: confirmed, but the mode could not be stored on the account.");
            }
        }

        return FirstEntryConfirmationResult.Confirmed;
    }

    public async Task<string?> GetResolvedOperationModeAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var confirmed = await database.PreferenceParameters.AsNoTracking()
            .Where(item => item.IsDeleted == false && item.Token == ConfirmedToken)
            .Select(item => item.Value)
            .FirstOrDefaultAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(confirmed))
        {
            return null;
        }

        var stored = await database.PreferenceParameters.AsNoTracking()
            .Where(item => item.IsDeleted == false && item.Token == OperationModeToken)
            .Select(item => item.Value)
            .FirstOrDefaultAsync(cancellationToken);

        var account = _account.IsCloudAccount ? _account.AccountOperationMode?.Trim().ToLowerInvariant() : null;
        if (account is not null
            && string.Equals(account, CloudOperationMode.NeutralModule, StringComparison.OrdinalIgnoreCase) == false
            && ValidModes.Contains(account))
        {
            return account;
        }

        var normalizedStored = string.IsNullOrWhiteSpace(stored) ? null : stored.Trim().ToLowerInvariant();
        if (normalizedStored is not null && ValidModes.Contains(normalizedStored))
        {
            return normalizedStored;
        }

        return account is not null && ValidModes.Contains(account) ? account : stored;
    }

    public async Task<Guid?> SuggestCurrencyIdAsync(Guid? countryId, CancellationToken cancellationToken = default)
    {
        if (countryId is null || countryId == Guid.Empty)
        {
            return null;
        }

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var currencyCode = await database.Countries.AsNoTracking()
            .Where(item => item.Id == countryId && item.IsDeleted == false)
            .Select(item => item.CurrencyCode)
            .FirstOrDefaultAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(currencyCode))
        {
            return null;
        }

        return await database.Currencies.AsNoTracking()
            .Where(item => item.IsDeleted == false && item.Acronym == currencyCode)
            .Select(item => (Guid?)item.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<bool> IsPendingAsync(CancellationToken cancellationToken)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var values = await database.PreferenceParameters.AsNoTracking()
            .Where(item => item.IsDeleted == false)
            .Select(item => new { item.Token, item.Value })
            .ToListAsync(cancellationToken);

        if (values.Any(item => item.Token == ConfirmedToken && string.IsNullOrWhiteSpace(item.Value) == false))
        {
            return false;
        }

        // Update / legacy database: the company is already filled in, or a fiscal year exists.
        // Articles and tables can come from a sample loaded before the person chose the mode.
        if (LocalCompanySetupService.IsCompanyConfigured(values.Select(item => (item.Token, item.Value))))
        {
            return false;
        }

        if (await database.FiscalYears.AsNoTracking().AnyAsync(item => item.IsDeleted == false, cancellationToken))
        {
            return false;
        }

        return true;
    }

    private static async Task UpsertPreferenceAsync(
        LogicPOSDbContext database,
        string token,
        string value,
        uint order,
        CancellationToken cancellationToken)
    {
        var row = await database.PreferenceParameters
            .FirstOrDefaultAsync(item => item.IsDeleted == false && item.Token == token, cancellationToken);
        var now = DateTime.Now;
        if (row is not null)
        {
            row.Value = value;
            row.UpdatedAt = now;
            return;
        }

        database.PreferenceParameters.Add(new PreferenceParameter
        {
            Id = Guid.NewGuid(),
            Token = token,
            Value = value,
            Required = false,
            RegEx = "RegexAlfaNumericExtended",
            ResourceString = $"prefparam_{token.ToLowerInvariant()}",
            FormType = 1,
            FormPageNo = null,
            InputType = PreferenceParameterInputType.Text,
            Order = order,
            Code = order.ToString(),
            CreatedAt = now,
            UpdatedAt = now
        });
    }

    private void WriteModuleToAppSettings(string mode)
    {
        var path = Path.Combine(_baseDirectory, "appsettings.json");
        JsonNode root = File.Exists(path)
            ? JsonNode.Parse(File.ReadAllText(path)) ?? new JsonObject()
            : new JsonObject();

        var database = root["DatabaseSettings"] as JsonObject ?? new JsonObject();
        root["DatabaseSettings"] = database;
        database["Module"] = mode;

        File.WriteAllText(path, root.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
    }
}
