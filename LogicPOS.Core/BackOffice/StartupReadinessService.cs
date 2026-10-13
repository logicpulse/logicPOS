using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.BackOffice;

public sealed class StartupReadinessIssue
{
    public StartupReadinessIssue(string key, string message, string? actionPage = null, string? actionLabel = null)
    {
        Key = key;
        Message = message;
        ActionPage = actionPage;
        ActionLabel = actionLabel;
    }

    public string Key { get; }

    public string Message { get; }

    /// <summary>BackOffice page title to open when the user clicks the action.</summary>
    public string? ActionPage { get; }

    public string? ActionLabel { get; }
}

public interface IStartupReadinessService
{
    Task<IReadOnlyList<StartupReadinessIssue>> GetIssuesAsync(CancellationToken cancellationToken = default);
}

public sealed class StartupReadinessService : IStartupReadinessService
{
    private readonly IServiceScopeFactory _scopes;
    private readonly IConfiguration _configuration;

    public StartupReadinessService(IServiceScopeFactory scopes, IConfiguration configuration)
    {
        _scopes = scopes;
        _configuration = configuration;
    }

    public async Task<IReadOnlyList<StartupReadinessIssue>> GetIssuesAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var issues = new List<StartupReadinessIssue>();

        var year = DateTime.Now.Year;
        var hasFiscalYear = await database.FiscalYears.AsNoTracking()
            .AnyAsync(item => item.IsDeleted == false, cancellationToken);
        var hasCurrentYear = await database.FiscalYears.AsNoTracking()
            .AnyAsync(item => item.IsDeleted == false && item.Year == year, cancellationToken);
        var seriesCount = await database.DocumentSeries.AsNoTracking()
            .CountAsync(item => item.IsDeleted == false, cancellationToken);

        if (hasFiscalYear == false)
        {
            issues.Add(new StartupReadinessIssue(
                "fiscal-year",
                "Falta criar o ano fiscal antes de emitir documentos.",
                "Abertura de ano fiscal",
                "Abrir"));
        }
        else if (hasCurrentYear == false)
        {
            issues.Add(new StartupReadinessIssue(
                "fiscal-year-current",
                $"Não existe ano fiscal para {year}. Crie a abertura do ano corrente.",
                "Abertura de ano fiscal",
                "Abrir"));
        }

        if (seriesCount == 0)
        {
            issues.Add(new StartupReadinessIssue(
                "series",
                "Faltam séries de documento. Use a Abertura de ano fiscal para as criar.",
                "Abertura de ano fiscal",
                "Abrir"));
        }
        else if (IsPortugal())
        {
            var withoutAt = await database.DocumentSeries.AsNoTracking()
                .Where(item => item.IsDeleted == false
                    && (item.ATDocCodeValidationSeries == null || item.ATDocCodeValidationSeries == ""))
                .CountAsync(cancellationToken);
            if (withoutAt > 0)
            {
                issues.Add(new StartupReadinessIssue(
                    "series-at",
                    $"{withoutAt} série(s) ainda sem comunicação à AT. Registe as séries em Séries.",
                    "Séries",
                    "Abrir"));
            }
        }

        return issues;
    }

    private bool IsPortugal()
    {
        var country = _configuration["Country"] ?? _configuration["SystemCountry"] ?? string.Empty;
        return string.IsNullOrWhiteSpace(country)
            || country.Equals("PT", StringComparison.OrdinalIgnoreCase)
            || country.Equals("Portugal", StringComparison.OrdinalIgnoreCase);
    }
}
