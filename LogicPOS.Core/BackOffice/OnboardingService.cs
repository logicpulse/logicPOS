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
        bool opensCompanySetup = false,
        bool isOptional = false,
        string? statusText = null)
    {
        Key = key;
        Title = title;
        Description = description;
        ActionPage = actionPage;
        IsComplete = isComplete;
        OpensCompanySetup = opensCompanySetup;
        IsOptional = isOptional;
        StatusText = statusText;
    }

    public string Key { get; }

    public string Title { get; }

    public string Description { get; }

    /// <summary>BackOffice page title to open, when <see cref="OpensCompanySetup"/> is false.</summary>
    public string? ActionPage { get; }

    public bool IsComplete { get; }

    public bool OpensCompanySetup { get; }

    /// <summary>Optional steps never block setup and are left out of the counts and percentage.</summary>
    public bool IsOptional { get; }

    /// <summary>Short extra status shown on the card, e.g. the number of users.</summary>
    public string? StatusText { get; }
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
                if (Steps[i].IsOptional == false && Steps[i].IsComplete)
                {
                    count++;
                }
            }

            return count;
        }
    }

    public int TotalCount
    {
        get
        {
            var count = 0;
            for (var i = 0; i < Steps.Count; i++)
            {
                if (Steps[i].IsOptional == false)
                {
                    count++;
                }
            }

            return count;
        }
    }

    /// <summary>First required step still pending, or null when setup is complete.</summary>
    public OnboardingStep? NextStep
    {
        get
        {
            for (var i = 0; i < Steps.Count; i++)
            {
                if (Steps[i].IsOptional == false && Steps[i].IsComplete == false)
                {
                    return Steps[i];
                }
            }

            return null;
        }
    }

    public int PercentComplete
        => TotalCount == 0
            ? 100
            : (int)Math.Round(100.0 * CompletedCount / TotalCount);

    public bool IsComplete => NextStep is null;
}

/// <summary>
/// What a status source found. A null value means the source could not check it
/// (e.g. the cloud API refused the request); that step is then shown as optional.
/// </summary>
public sealed class OnboardingFacts
{
    public bool BackOfficeOnly { get; init; }

    public bool? CompanyComplete { get; init; }

    public int? UserCount { get; init; }

    public bool? HasFiscalYearAndSeries { get; init; }

    public bool? HasArticle { get; init; }

    public bool? HasFinalConsumer { get; init; }

    public bool HasPlace { get; init; }

    public bool? HasTable { get; init; }

    public bool? HasPaymentMethod { get; init; }

    public bool? HasTerminalPrinter { get; init; }
}

public interface IOnboardingService
{
    Task<OnboardingStatus> GetStatusAsync(CancellationToken cancellationToken = default);
}

public sealed class OnboardingService : IOnboardingService
{
    public const string FinalConsumerName = "Consumidor Final";

    private const string UncheckedText = "Não foi possível verificar";

    private readonly IServiceScopeFactory _scopes;
    private readonly ICompanySetupService _companySetup;
    private readonly IOperationMode _operationMode;
    private readonly IPrinterSetupWizard _printerSetup;

    public OnboardingService(
        IServiceScopeFactory scopes,
        ICompanySetupService companySetup,
        IOperationMode operationMode,
        IPrinterSetupWizard printerSetup)
    {
        _scopes = scopes;
        _companySetup = companySetup;
        _operationMode = operationMode;
        _printerSetup = printerSetup;
    }

    public async Task<OnboardingStatus> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var backOfficeOnly = await _operationMode.IsBackOfficeOnlyAsync(cancellationToken);
        var year = DateTime.Now.Year;

        var companyNeedsSetup = await _companySetup.NeedsSetupAsync(cancellationToken);
        var userCount = await database.Users.AsNoTracking()
            .CountAsync(item => item.IsDeleted == false, cancellationToken);
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
        var hasTerminalPrinter = await _printerSetup.HasTerminalPrinterAsync(cancellationToken);

        return Build(new OnboardingFacts
        {
            BackOfficeOnly = backOfficeOnly,
            CompanyComplete = companyNeedsSetup == false,
            UserCount = userCount,
            HasFiscalYearAndSeries = hasCurrentFiscalYear && hasSeries,
            HasArticle = hasArticle,
            HasFinalConsumer = hasFinalConsumer,
            HasPlace = hasPlace,
            HasTable = hasTable,
            HasPaymentMethod = hasPaymentMethod,
            HasTerminalPrinter = hasTerminalPrinter
        });
    }

    /// <summary>Step list shared by the local database and the cloud API status sources.</summary>
    public static OnboardingStatus Build(OnboardingFacts facts)
    {
        var userCount = facts.UserCount;
        var steps = new List<OnboardingStep>
        {
            Step(
                "first-user",
                "Primeiro utilizador",
                "Crie o utilizador administrador e o PIN de acesso ao posto.",
                "Utilizadores",
                userCount is null ? null : userCount > 0),
            Step(
                "company",
                "Dados da empresa",
                "Indique o país, a moeda e o NIF da empresa.",
                actionPage: null,
                facts.CompanyComplete,
                opensCompanySetup: true),
            Step(
                "fiscal-year",
                "Ano fiscal e séries",
                "Crie o ano fiscal corrente e as séries de documento.",
                "Abertura de ano fiscal",
                facts.HasFiscalYearAndSeries),
            Step(
                "article",
                "Primeiro artigo",
                "Crie família, subfamília e o primeiro artigo para vender no posto.",
                "Artigos",
                facts.HasArticle),
            Step(
                "final-consumer",
                "Cliente de balcão",
                "Crie o «Consumidor Final» para fatura simplificada.",
                "Clientes",
                facts.HasFinalConsumer)
        };

        if (facts.BackOfficeOnly == false)
        {
            steps.Add(Step(
                "table",
                "Mesa de venda",
                facts.HasPlace
                    ? "Crie pelo menos uma mesa para gravar pedidos no posto."
                    : "Crie um local e uma mesa para gravar pedidos no posto.",
                facts.HasPlace ? "Mesas" : "Locais",
                facts.HasTable));
        }

        if (facts.HasPaymentMethod != true)
        {
            steps.Add(Step(
                "payment-method",
                "Métodos de pagamento",
                "Crie pelo menos um método de pagamento para fechar vendas.",
                "Métodos de pagamento",
                facts.HasPaymentMethod));
        }

        steps.Add(Step(
            "more-users",
            "Adicionar utilizadores",
            "Crie mais utilizadores (empregados, gerentes) e atribua perfis.",
            "Utilizadores",
            userCount is null ? null : userCount > 1,
            isOptional: true,
            statusText: userCount > 1 ? $"{userCount} utilizadores" : null));

        steps.Add(Step(
            "printer",
            "Impressora",
            "Escolha a impressora do Windows e o papel e associe-a a este terminal.",
            "Impressoras",
            facts.HasTerminalPrinter,
            isOptional: true));

        return new OnboardingStatus(steps);
    }

    private static OnboardingStep Step(
        string key,
        string title,
        string description,
        string? actionPage,
        bool? isComplete,
        bool opensCompanySetup = false,
        bool isOptional = false,
        string? statusText = null)
        => new(
            key,
            title,
            description,
            actionPage,
            isComplete == true,
            opensCompanySetup,
            isOptional || isComplete is null,
            isComplete is null ? UncheckedText : statusText);
}
