using LogicPOS.Core.BackOffice;
using LogicPOS.Core.Fiscal;
using LogicPOS.Core.FrontOffice;
using LogicPOS.Persistence.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.Tests.Fiscal;

[Collection(LogicPOS.Core.Tests.AppCompositionCollection.Name)]
public sealed class FiscalSeriesAtIssueTests
{
    [Fact]
    public async Task Without_fiscal_plugin_local_issue_continues_without_at_code()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-at-series-public-");
        try
        {
            var services = await ConfigureReadyToIssueAsync(root, withFiscalPlugin: false);
            Assert.False(services.GetRequiredService<IFiscalModule>().IsAvailable);

            var document = await IssueSimplifiedAsync(services);
            Assert.Null(document.Error);
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task With_fiscal_plugin_issue_is_refused_until_series_is_communicated()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-at-series-fiscal-");
        try
        {
            var services = await ConfigureReadyToIssueAsync(root, withFiscalPlugin: true);
            Assert.True(services.GetRequiredService<IFiscalModule>().IsAvailable);

            var refused = await IssueSimplifiedAsync(services);
            Assert.NotNull(refused.Error);
            Assert.Contains("não foi comunicada", refused.Error, StringComparison.OrdinalIgnoreCase);

            await using (var scope = services.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
                var series = await database.DocumentSeries
                    .Include(item => item.DocumentType)
                    .FirstAsync(item => item.IsDeleted == false && item.DocumentType!.Acronym == "FS");
                series.ATDocCodeValidationSeries = "ABC1234X";
                await database.SaveChangesAsync();
            }

            var issued = await IssueSimplifiedAsync(services);
            Assert.Null(issued.Error);
            await using (var scope = services.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
                var document = await database.Documents.AsNoTracking()
                    .FirstAsync(item => item.Id == issued.DocumentId);
                Assert.StartsWith("ABC1234X-", document.ATCUD, StringComparison.Ordinal);
            }
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Without_fiscal_plugin_startup_still_warns_about_uncommunicated_series()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-at-series-startup-");
        try
        {
            var services = await ConfigureReadyToIssueAsync(root, withFiscalPlugin: false);
            var issues = await services.GetRequiredService<IStartupReadinessService>().GetIssuesAsync();
            Assert.Contains(issues, issue => issue.Key == "series-at");
        }
        finally
        {
            Cleanup(root);
        }
    }

    private static async Task<PosDocumentResult> IssueSimplifiedAsync(IServiceProvider services)
    {
        var customer = await services.GetRequiredService<IPosCustomerService>().GetFinalConsumerAsync(CancellationToken.None);
        Assert.NotNull(customer);
        var catalog = await services.GetRequiredService<IPosCatalogService>().LoadAsync(CancellationToken.None);
        var article = catalog.Articles.First();
        return await services.GetRequiredService<IPosDocumentService>().IssueDocumentAsync(
            "FS",
            customer.Id,
            [new PosSaleLine(article.Id, 1, article.CatalogPrice, 0, article.VatRateId)],
            new PosDocumentHeader { PaymentToken = "MONEY" },
            cancellationToken: CancellationToken.None);
    }

    private static async Task<IServiceProvider> ConfigureReadyToIssueAsync(DirectoryInfo root, bool withFiscalPlugin)
    {
        if (withFiscalPlugin)
        {
            var plugins = Directory.CreateDirectory(Path.Combine(root.FullName, "plugins"));
            var built = Path.Combine(AppContext.BaseDirectory, "SampleFiscalPlugin.dll");
            File.Copy(built, Path.Combine(plugins.FullName, "SampleFiscalPlugin.dll"));
        }

        var databasePath = Path.Combine(root.FullName, "logicpos.db");
        File.WriteAllText(
            Path.Combine(root.FullName, "appsettings.json"),
            $$"""
            {
              "Culture": "pt-PT",
              "DatabaseSettings": {
                "DatabaseType": "Sqlite",
                "ConnectionString": "Data Source={{databasePath.Replace("\\", "\\\\")}}",
                "UseCloud": false
              }
            }
            """);

        AppComposition.Configure(root.FullName);
        Assert.Null(AppComposition.StartupError);

        var services = AppComposition.Services!;
        var company = services.GetRequiredService<ICompanySetupService>();
        var companyDraft = await company.LoadAsync(CancellationToken.None);
        var companySaved = await company.SaveAsync(new CompanySetupRequest
        {
            CountryId = companyDraft.CountryId ?? Guid.Empty,
            CurrencyId = companyDraft.CurrencyId ?? Guid.Empty,
            Values = LocalCompanySetupService.DemoCompanyValues
        }, CancellationToken.None);
        Assert.True(companySaved.Succeeded, companySaved.Error);

        var wizard = services.GetRequiredService<IFiscalYearWizard>();
        var draft = await wizard.LoadAsync(CancellationToken.None);
        var setup = await wizard.CreateAsync(new FiscalYearSetupRequest
        {
            Designation = draft.Designation,
            Year = draft.Year,
            Acronym = draft.Acronym,
            CloseCurrent = false,
            CreateSeries = true,
            SeriesForEachTerminal = false,
            CommunicateWithAt = false,
            TerminalIds = draft.Terminals.Select(item => item.Id).ToList(),
            DocumentTypeIds = draft.DocumentTypes.Select(item => item.Id).ToList()
        }, CancellationToken.None);
        Assert.True(setup.Ok, setup.Message);
        return services;
    }

    private static void Cleanup(DirectoryInfo root)
    {
        AppLogging.CloseAndFlush();
        SqliteConnection.ClearAllPools();

        if (AppComposition.Services is IDisposable disposable)
        {
            disposable.Dispose();
        }

        const int maxRetries = 5;
        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                if (root.Exists)
                {
                    root.Delete(recursive: true);
                }

                return;
            }
            catch (IOException) when (attempt < maxRetries)
            {
                Thread.Sleep(200 * attempt);
            }
            catch (UnauthorizedAccessException) when (attempt < maxRetries)
            {
                Thread.Sleep(200 * attempt);
            }
        }
    }
}