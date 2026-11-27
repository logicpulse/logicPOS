using LogicPOS.Core.BackOffice;
using LogicPOS.Core.Fiscal;
using LogicPOS.Core.FrontOffice;
using LogicPOS.Core.Tests.FiscalPlugin;
using LogicPOS.Persistence.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.Tests.Fiscal;

[Collection(AppCompositionCollection.Name)]
public sealed class FiscalStoredAtQrPrintTests
{
    [Fact]
    public void ResolveForPrint_keeps_persisted_qr_over_fiscal_plugin_regeneration()
    {
        const string stored = "A:502757191*B:999999990*E:N*H:TESTE-1*";
        var module = new SampleFiscalPlugin();
        var document = new FiscalDocument
        {
            Type = "FS",
            Number = "FS T/99",
            QrPayload = stored,
            Status = "N",
            TotalFinal = 1m
        };
        FiscalQrOnPrint.ResolveForPrint(stored, module, document, out _, out var output);
        Assert.Equal(stored, output);
    }

    [Fact]
    public void AtQr_customer_fiscal_uses_final_consumer_code_on_ticket_blank_nif()
    {
        Assert.Equal(PosDocumentRules.FinalConsumerFiscalNumber,
            PosDocumentRules.AtQrCustomerFiscalNumber(null));
        Assert.Equal(PosDocumentRules.FinalConsumerFiscalNumber,
            PosDocumentRules.AtQrCustomerFiscalNumber(PosDocumentRules.FinalConsumerFiscalNumber));
        Assert.Equal("123456789",
            PosDocumentRules.AtQrCustomerFiscalNumber("123 456 789"));
    }

    [Fact]
    public async Task Issued_document_keeps_ATQRCode_after_company_preference_changes()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-stored-qr-nif-");
        try
        {
            var services = await ConfigureWithFiscalPluginAsync(root);
            var issued = await IssueSimplifiedAsync(services);
            Assert.Null(issued.Error);

            string? persisted;
            await using (var scope = services.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
                persisted = await database.Documents.AsNoTracking()
                    .Where(item => item.Id == issued.DocumentId)
                    .Select(item => item.ATQRCode)
                    .FirstAsync();
                Assert.False(string.IsNullOrWhiteSpace(persisted));
                var pref = await database.PreferenceParameters
                    .FirstAsync(item => item.Token == "COMPANY_FISCALNUMBER");
                pref.Value = "999 888 777";
                await database.SaveChangesAsync();
            }

            _ = await services.GetRequiredService<IPosDocumentService>()
                .CreateA4FileAsync(issued.DocumentId, CancellationToken.None);

            await using (var scope = services.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
                var current = await database.Documents.AsNoTracking()
                    .Where(item => item.Id == issued.DocumentId)
                    .Select(item => item.ATQRCode)
                    .FirstAsync();
                Assert.Equal(persisted, current);
            }
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Cancelled_document_keeps_persisted_ATQRCode_in_database()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-stored-qr-cancel-");
        try
        {
            var services = await ConfigureWithFiscalPluginAsync(root);
            var issued = await IssueSimplifiedAsync(services);
            Assert.Null(issued.Error);

            string? persisted;
            await using (var scope = services.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
                var entity = await database.Documents.FirstAsync(item => item.Id == issued.DocumentId);
                persisted = entity.ATQRCode;
                entity.Status = "A";
                await database.SaveChangesAsync();
            }

            FiscalQrOnPrint.ResolveForPrint(
                persisted,
                new SampleFiscalPlugin(),
                new FiscalDocument { Type = "FS", Number = issued.Number!, Status = "A", QrPayload = persisted },
                out _,
                out var output);
            Assert.Equal(persisted, output);
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

    private static async Task<IServiceProvider> ConfigureWithFiscalPluginAsync(DirectoryInfo root)
    {
        var plugins = Directory.CreateDirectory(Path.Combine(root.FullName, "plugins"));
        var built = Path.Combine(AppContext.BaseDirectory, "SampleFiscalPlugin.dll");
        File.Copy(built, Path.Combine(plugins.FullName, "SampleFiscalPlugin.dll"));

        var databasePath = Path.Combine(root.FullName, "logicpos.db");
        await File.WriteAllTextAsync(
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
        Assert.True(services.GetRequiredService<IFiscalModule>().IsAvailable);

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

        await using (var scope = services.CreateAsyncScope())
        {
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
            var series = await database.DocumentSeries
                .Include(item => item.DocumentType)
                .FirstAsync(item => item.IsDeleted == false && item.DocumentType!.Acronym == "FS");
            series.ATDocCodeValidationSeries = "TESTE005";
            await database.SaveChangesAsync();
        }

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

        try
        {
            root.Delete(recursive: true);
        }
        catch (IOException)
        {
        }
    }
}