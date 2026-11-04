using LogicPOS.Core;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.BackOffice;

public sealed class OnboardingStep
{
    public OnboardingStep(
        string key,
        string title,
        string description,
        string? actionPage,
        bool isComplete,
        bool opensCompanySetup = false)
    {
        Key = key;
        Title = title;
        Description = description;
        ActionPage = actionPage;
        IsComplete = isComplete;
        OpensCompanySetup = opensCompanySetup;
    }

    public string Key { get; }

    public string Title { get; }

    public string Description { get; }

    /// <summary>BackOffice page title to open, when <see cref="OpensCompanySetup"/> is false.</summary>
    public string? ActionPage { get; }

    public bool IsComplete { get; }

    public bool OpensCompanySetup { get; }
}

public sealed class OnboardingStatus
{
    public OnboardingStatus(IReadOnlyList<OnboardingStep> steps)
    {
        Steps = steps;
    }

    public IReadOnlyList<OnboardingStep> Steps { get; }

    public int CompletedCount
    {
        get
        {
            var count = 0;
            for (var i = 0; i < Steps.Count; i++)
            {
                if (Steps[i].IsComplete)
                {
                    count++;
                }
            }

            return count;
        }
    }

    public int TotalCount => Steps.Count;

    public int PercentComplete
        => TotalCount == 0
            ? 100
            : (int)Math.Round(100.0 * CompletedCount / TotalCount);

    public bool IsComplete
    {
        get
        {
            for (var i = 0; i < Steps.Count; i++)
            {
                if (Steps[i].IsComplete == false)
                {
                    return false;
                }
            }

            return true;
        }
    }
}

public interface IOnboardingService
{
    Task<OnboardingStatus> GetStatusAsync(CancellationToken cancellationToken = default);
}

public sealed class OnboardingService : IOnboardingService
{
    private const string FinalConsumerName = "Consumidor Final";

    private readonly IServiceScopeFactory _scopes;
    private readonly ICompanySetupService _companySetup;
    private readonly IOperationMode _operationMode;

    public OnboardingService(
        IServiceScopeFactory scopes,
        ICompanySetupService companySetup,
        IOperationMode operationMode)
    {
        _scopes = scopes;
        _companySetup = companySetup;
        _operationMode = operationMode;
    }

    public async Task<OnboardingStatus> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var backOfficeOnly = await _operationMode.IsBackOfficeOnlyAsync(cancellationToken);
        var year = DateTime.Now.Year;

        var companyNeedsSetup = await _companySetup.NeedsSetupAsync(cancellationToken);
        var hasCurrentFiscalYear = await database.FiscalYears.AsNoTracking()
            .AnyAsync(item => item.IsDeleted == false && item.Year == year, cancellationToken);
        var hasSeries = await database.DocumentSeries.AsNoTracking()
            .AnyAsync(item => item.IsDeleted == false, cancellationToken);
        var hasArticle = await database.Articles.AsNoTracking()
            .AnyAsync(item => item.IsDeleted == false, cancellationToken);
        var hasFinalConsumer = await database.Customers.AsNoTracking()
            .AnyAsync(item => item.IsDeleted == false && item.Name == FinalConsumerName, cancellationToken);
        var hasPlace = await database.Places.AsNoTracking()
            .AnyAsync(item => item.IsDeleted == false, cancellationToken);
        var hasTable = await database.Tables.AsNoTracking()
            .AnyAsync(item => item.IsDeleted == false, cancellationToken);
        var hasPaymentMethod = await database.PaymentMethods.AsNoTracking()
            .AnyAsync(item => item.IsDeleted == false, cancellationToken);

        var steps = new List<OnboardingStep>
        {
            new(
                "company",
                "Dados da empresa",
                "Indique o país, a moeda e o NIF da empresa.",
                actionPage: null,
                isComplete: companyNeedsSetup == false,
                opensCompanySetup: true),
            new(
                "fiscal-year",
                "Ano fiscal e séries",
                "Crie o ano fiscal corrente e as séries de documento.",
                "Abertura de ano fiscal",
                hasCurrentFiscalYear && hasSeries),
            new(
                "article",
                "Primeiro artigo",
                "Crie família, subfamília e o primeiro artigo para vender no posto.",
                "Artigos",
                hasArticle),
            new(
                "final-consumer",
                "Cliente de balcão",
                "Crie o «Consumidor Final» para fatura simplificada.",
                "Clientes",
                hasFinalConsumer)
        };

        if (backOfficeOnly == false)
        {
            steps.Add(new OnboardingStep(
                "table",
                "Mesa de venda",
                hasPlace
                    ? "Crie pelo menos uma mesa para gravar pedidos no posto."
                    : "Crie um local e uma mesa para gravar pedidos no posto.",
                hasPlace ? "Mesas" : "Locais",
                hasTable));
        }

        if (hasPaymentMethod == false)
        {
            steps.Add(new OnboardingStep(
                "payment-method",
                "Métodos de pagamento",
                "Crie pelo menos um método de pagamento para fechar vendas.",
                "Métodos de pagamento",
                isComplete: false));
        }

        return new OnboardingStatus(steps);
    }
}
