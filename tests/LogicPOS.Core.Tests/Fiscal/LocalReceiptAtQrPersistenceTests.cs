using LogicPOS.Application.Features.Finance.Documents;
using LogicPOS.Core.BackOffice;
using LogicPOS.Core.Fiscal;
using LogicPOS.Core.FrontOffice;
using LogicPOS.Domain.Enums;
using LogicPOS.Persistence.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.Tests.Fiscal;

[Collection(AppCompositionCollection.Name)]
public sealed class LocalReceiptAtQrPersistenceTests
{
    [Fact]
    public void DocumentAtQrPayload_supports_RC_receipt_document_type()
    {
        var payload = DocumentAtQrPayload.Build(
            issuerFiscalNumber: "502757191",
            customerFiscalNumber: "999999990",
            customerCountry: "PT",
            documentType: "RC",
            status: "N",
            dateYyyyMmDd: "2026-11-27",
            uniqueDocumentId: "RC 2026/1",
            atcud: "TESTE001-1",
            totalNet: 80m,
            totalTax: 20m,
            totalFinal: 100m,
            hash4: "AbCd");
        Assert.Contains("D:RC*", payload, StringComparison.Ordinal);
        Assert.StartsWith("A:502757191*", payload, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Certified_local_receipt_emission_persists_non_empty_QrCode()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-receipt-qr-");
        try
        {
            var services = await ConfigureWithFiscalPluginAsync(root);

            await using (var scope = services.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
                var series = await database.DocumentSeries
                    .Include(item => item.DocumentType)
                    .Where(item => item.IsDeleted == false)
                    .ToListAsync();
                foreach (var item in series)
                {
                    if (item.DocumentType!.Acronym is "FT" or "RC")
                    {
                        item.ATDocCodeValidationSeries = "TESTE007";
                    }
                }

                await database.SaveChangesAsync();
            }

            var customer = await services.GetRequiredService<IPosCustomerService>().GetFinalConsumerAsync(CancellationToken.None);
            Assert.NotNull(customer);
            var catalog = await services.GetRequiredService<IPosCatalogService>().LoadAsync(CancellationToken.None);
            var article = catalog.Articles.First();

            Guid deferredPaymentId;
            await using (var scope = services.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
                deferredPaymentId = await database.PaymentConditions.AsNoTracking()
                    .Where(item => item.IsDeleted == false && item.Acronym != "PRONTOPAGAMENTO")
                    .Select(item => item.Id)
                    .FirstAsync();
            }

            var issued = await services.GetRequiredService<IPosDocumentService>().IssueDocumentAsync(
                "FT",
                customer.Id,
                [new PosSaleLine(article.Id, 1, article.CatalogPrice, 0, article.VatRateId)],
                new PosDocumentHeader
                {
                    PaymentConditionId = deferredPaymentId
                },
                cancellationToken: CancellationToken.None);
            Assert.Null(issued.Error);

            decimal totalFinal;
            await using (var scope = services.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
                totalFinal = await database.Documents.AsNoTracking()
                    .Where(item => item.Id == issued.DocumentId)
                    .Select(item => item.TotalFinal)
                    .FirstAsync();
            }

            var catalogPay = await services.GetRequiredService<IReceiptEmission>().CatalogAsync();
            var money = catalogPay.PaymentMethods.First();
            var currency = catalogPay.Currencies.First();

            var pay = await services.GetRequiredService<IReceiptEmission>().PayAsync(
                new ReceiptPayRequest
                {
                    DocumentIds = [issued.DocumentId],
                    PaymentMethodId = money.Id,
                    CurrencyId = currency.Id,
                    Amount = totalFinal,
                    CurrencyAmount = totalFinal,
                    ExchangeRate = 1m
                });
            Assert.True(pay.Ok, pay.Message);

            await using (var scope = services.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
                var qr = await database.Receipts.AsNoTracking()
                    .OrderByDescending(item => item.CreatedAt)
                    .Select(item => item.QrCode)
                    .FirstAsync();
                Assert.False(string.IsNullOrWhiteSpace(qr));
                Assert.Contains("D:RC*", qr, StringComparison.Ordinal);
            }
        }
        finally
        {
            Cleanup(root);
        }
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
        var services = AppComposition.Services;
        Assert.NotNull(services);
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