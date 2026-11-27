using LogicPOS.Application.Features.Finance.Documents;
using LogicPOS.Core.BackOffice;
using LogicPOS.Core.Fiscal;
using LogicPOS.Core.FrontOffice;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Persistence.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.Tests.Fiscal;

[Collection(AppCompositionCollection.Name)]
public sealed class LocalReceiptAtQrDiscriminatedTests
{
    private static readonly Guid VatInt13 = Guid.Parse("f73e3b41-4967-48c6-9f9a-260abf2146e1");
    private static readonly Guid VatRed6 = Guid.Parse("7e89eaed-ce56-4565-8eec-98f2e8d004a5");
    private static readonly Guid VatNor23 = Guid.Parse("cee00590-7317-41b8-af46-66560401096b");
    private static readonly Guid VatIse0 = Guid.Parse("e74faad7-f5c9-4206-a662-f95820014195");
    private static readonly Guid VatNorPtAc18 = Guid.Parse("389661c1-05f6-4830-bc06-176e2fdb3dc2");

    [Fact]
    public async Task Local_receipt_full_INT_13_percent_emits_I5_I6()
    {
        await AssertQrAfterFullReceiptAsync(
            [new PosSaleLine(Guid.Empty, 1, 8m, 0, VatInt13)],
            qr =>
            {
                Assert.Contains("*I1:PT*I5:8.00*I6:1.04*", qr, StringComparison.Ordinal);
                Assert.Contains("*D:RC*", qr, StringComparison.Ordinal);
                Assert.Contains("*N:1.04*", qr, StringComparison.Ordinal);
                Assert.Contains("*O:9.04*", qr, StringComparison.Ordinal);
            });
    }

    [Fact]
    public async Task Local_receipt_full_RED_6_percent_emits_I3_I4()
    {
        await AssertQrAfterFullReceiptAsync(
            [new PosSaleLine(Guid.Empty, 1, 4m, 0, VatRed6)],
            qr =>
            {
                Assert.Contains("*I1:PT*I3:4.00*I4:0.24*", qr, StringComparison.Ordinal);
                Assert.Contains("*N:0.24*", qr, StringComparison.Ordinal);
                Assert.Contains("*O:4.24*", qr, StringComparison.Ordinal);
            });
    }

    [Fact]
    public async Task Local_receipt_multi_rate_full_payment()
    {
        await AssertQrAfterFullReceiptAsync(
            [
                new PosSaleLine(Guid.Empty, 1, 4m, 0, VatRed6),
                new PosSaleLine(Guid.Empty, 1, 10m, 0, VatInt13),
                new PosSaleLine(Guid.Empty, 1, 10m, 0, VatNor23),
                new PosSaleLine(Guid.Empty, 1, 5m, 0, VatIse0),
            ],
            qr =>
            {
                Assert.Contains("*I2:5.00*", qr, StringComparison.Ordinal);
                Assert.Contains("*I3:4.00*I4:0.24*", qr, StringComparison.Ordinal);
                Assert.Contains("*I5:10.00*I6:1.30*", qr, StringComparison.Ordinal);
                Assert.Contains("*I7:10.00*I8:2.30*", qr, StringComparison.Ordinal);
                Assert.Contains("*N:3.84*", qr, StringComparison.Ordinal);
                Assert.Contains("*O:32.84*", qr, StringComparison.Ordinal);
            });
    }

    [Fact]
    public async Task Local_receipt_PT_AC_full_payment_emits_J_space()
    {
        await AssertQrAfterFullReceiptAsync(
            [new PosSaleLine(Guid.Empty, 1, 10m, 0, VatNorPtAc18)],
            qr =>
            {
                Assert.Contains("*J1:PT-AC*", qr, StringComparison.Ordinal);
                Assert.Contains("*J7:10.00*", qr, StringComparison.Ordinal);
                Assert.Contains("*J8:1.80*", qr, StringComparison.Ordinal);
                Assert.Contains("*N:1.80*", qr, StringComparison.Ordinal);
                Assert.Contains("*O:11.80*", qr, StringComparison.Ordinal);
            },
            enablePtAcVat: true);
    }

    [Fact]
    public async Task Local_receipt_partial_after_prior_payment_uses_proportional_NOR_buckets()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-rc-qr-partial-");
        try
        {
            var services = await ConfigureWithFiscalPluginAsync(root);
            var articleId = await FirstArticleIdAsync(services);
            var customer = await services.GetRequiredService<IPosCustomerService>()
                .GetFinalConsumerAsync(CancellationToken.None);
            Assert.NotNull(customer);

            var issued = await IssueDeferredFtAsync(
                services,
                customer!.Id,
                articleId,
                [new PosSaleLine(articleId, 1, 100m, 0, VatNor23)]);
            Assert.Null(issued.Error);

            var catalogPay = await services.GetRequiredService<IReceiptEmission>().CatalogAsync();
            await using (var scope = services.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
                var prior = new Receipt
                {
                    PaymentMethodId = catalogPay.PaymentMethods.First().Id,
                    DocumentType = "RC",
                    RefNo = "RC PREV/1",
                    Amount = 50m,
                    TaxPayable = 0m,
                    AmountInWords = "cinquenta",
                    CurrencyCode = "EUR",
                    CurrencyAmount = 50m,
                    ExchangeRate = 1m,
                    Mechanism = "NU",
                    SourceId = "test-prior"
                };
                database.Receipts.Add(prior);
                await database.SaveChangesAsync(cancellationToken: CancellationToken.None);
                database.Payments.Add(new Payment
                {
                    ReceiptId = prior.Id,
                    DocumentId = issued.DocumentId,
                    CreditAmount = 50m
                });
                await database.SaveChangesAsync(cancellationToken: CancellationToken.None);
            }

            var qr = await PayReceiptAndReadQrAsync(services, issued.DocumentId);
            // Prior 50 € on FT 123 € (100 net NOR + 23 tax). This receipt pays 73 € → scale 73/123.
            // I7 = 100×73/123 = 59.35; I8 = 23×73/123 = 13.65; N/O from receipt settlement.
            Assert.Contains("*I1:PT*I7:59.35*I8:13.65*", qr, StringComparison.Ordinal);
            Assert.Contains("*N:13.65*", qr, StringComparison.Ordinal);
            Assert.Contains("*O:73.00*", qr, StringComparison.Ordinal);
            Assert.DoesNotContain("*L:", qr, StringComparison.Ordinal);
            Assert.DoesNotContain("I1:0", qr, StringComparison.Ordinal);
            AssertStoredQrMatchesPdfSourceField(services, qr);
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Local_receipt_partial_multi_rate_rounding_adjusts_largest_base()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-rc-qr-partial-multi-");
        try
        {
            var services = await ConfigureWithFiscalPluginAsync(root);
            var articleId = await FirstArticleIdAsync(services);
            var customer = await services.GetRequiredService<IPosCustomerService>()
                .GetFinalConsumerAsync(CancellationToken.None);
            Assert.NotNull(customer);

            var issued = await IssueDeferredFtAsync(
                services,
                customer!.Id,
                articleId,
                [
                    new PosSaleLine(articleId, 1, 4m, 0, VatRed6),
                    new PosSaleLine(articleId, 1, 10m, 0, VatInt13),
                    new PosSaleLine(articleId, 1, 10m, 0, VatNor23),
                ]);
            Assert.Null(issued.Error);

            var catalogPay = await services.GetRequiredService<IReceiptEmission>().CatalogAsync();
            await using (var scope = services.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
                var prior = new Receipt
                {
                    PaymentMethodId = catalogPay.PaymentMethods.First().Id,
                    DocumentType = "RC",
                    RefNo = "RC PREV/2",
                    Amount = 10m,
                    TaxPayable = 0m,
                    AmountInWords = "dez",
                    CurrencyCode = "EUR",
                    CurrencyAmount = 10m,
                    ExchangeRate = 1m,
                    Mechanism = "NU",
                    SourceId = "test-prior-multi"
                };
                database.Receipts.Add(prior);
                await database.SaveChangesAsync(cancellationToken: CancellationToken.None);
                database.Payments.Add(new Payment
                {
                    ReceiptId = prior.Id,
                    DocumentId = issued.DocumentId,
                    CreditAmount = 10m
                });
                await database.SaveChangesAsync(cancellationToken: CancellationToken.None);
            }

            var qr = await PayReceiptAndReadQrAsync(services, issued.DocumentId);
            // FT gross 27.84 €; prior 10 € leaves 17.84 €. Scale = 17.84/27.84.
            // Rounded buckets: RED 2.56+0.15, INT 6.42+0.83 (largest base +0.01), NOR 6.41+1.47 → sum 17.84.
            Assert.Contains("*I3:2.56*I4:0.15*", qr, StringComparison.Ordinal);
            Assert.Contains("*I5:6.42*I6:0.83*", qr, StringComparison.Ordinal);
            Assert.Contains("*I7:6.41*I8:1.47*", qr, StringComparison.Ordinal);
            Assert.Contains("*O:17.84*", qr, StringComparison.Ordinal);
            Assert.DoesNotContain("*L:", qr, StringComparison.Ordinal);
            Assert.DoesNotContain("I1:0", qr, StringComparison.Ordinal);
            AssertStoredQrMatchesPdfSourceField(services, qr);
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public void Receipt_partial_proportional_constant_changes_breakdown_vs_Undiscriminated()
    {
        Assert.Equal(
            AtQrPartialPaymentAllocation.Proportional,
            ReceiptEmissionDefaults.ReceiptAtQrPartialPaymentAllocation);

        var settlement = new AtQrReceiptSettlement(
            ReceiptAmount: 61.50m,
            ReceiptTaxPayable: 11.50m,
            ReceiptWithholdingTaxAmount: 0m,
            SettledDocuments:
            [
                new AtQrSettledDocument(
                    61.50m, 123.00m, 23.00m, 0m,
                    [new AtQrSettledDocumentLine("PT", "NOR", 100.00m, 23.00m)])
            ]);

        var undiscriminated = AtQrReceiptBreakdown.FromSettledDocuments(
            settlement,
            AtQrPartialPaymentAllocation.Undiscriminated);
        var proportional = AtQrReceiptBreakdown.FromSettledDocuments(
            settlement,
            ReceiptEmissionDefaults.ReceiptAtQrPartialPaymentAllocation);

        Assert.Contains("L:", DocumentAtQrPayload.Build(BuildPayload("Und", undiscriminated)), StringComparison.Ordinal);
        var proportionalPayload = DocumentAtQrPayload.Build(BuildPayload("Pro", proportional));
        Assert.Contains("*I7:50.00*I8:11.50*", proportionalPayload, StringComparison.Ordinal);
        Assert.DoesNotContain("*L:", proportionalPayload, StringComparison.Ordinal);
    }

    [Fact]
    public void Withholding_field_P_when_breakdown_includes_retention()
    {
        var result = AtQrReceiptBreakdown.FromSettledDocuments(new AtQrReceiptSettlement(
            123.00m, 23.00m, 10.00m,
            SettledDocuments:
            [
                new AtQrSettledDocument(
                    123.00m, 123.00m, 23.00m, 10.00m,
                    [new AtQrSettledDocumentLine("PT", "NOR", 100.00m, 23.00m)])
            ]));

        var payload = DocumentAtQrPayload.Build(BuildPayload("Wh", result));
        Assert.Contains("*P:10.00*", payload, StringComparison.Ordinal);
    }

    private static AtQrPayloadInput BuildPayload(string hash4, AtQrReceiptBreakdownResult result)
        => new(
            "503504564",
            "999999990",
            "PT",
            "RC",
            "N",
            "20261127",
            "RC TEST/1",
            "TESTE-1",
            result.TaxBreakdown,
            result.TotalTax,
            result.TotalFinal,
            hash4,
            WithholdingTaxAmount: result.WithholdingTaxAmount,
            CurrencyAcronym: result.CurrencyAcronym,
            ExchangeRate: result.ExchangeRate);

    private static async Task AssertQrAfterFullReceiptAsync(
        IReadOnlyList<PosSaleLine> lines,
        Action<string> assertPayload,
        bool enablePtAcVat = false)
    {
        var root = Directory.CreateTempSubdirectory("logicpos-rc-qr-disc-");
        try
        {
            var services = await ConfigureWithFiscalPluginAsync(root, enablePtAcVat);
            var articleId = await FirstArticleIdAsync(services);
            var customer = await services.GetRequiredService<IPosCustomerService>()
                .GetFinalConsumerAsync(CancellationToken.None);
            Assert.NotNull(customer);

            var saleLines = lines
                .Select(line => new PosSaleLine(articleId, line.Quantity, line.NetUnitPrice, line.Discount, line.VatRateId))
                .ToList();
            var issued = await IssueDeferredFtAsync(services, customer!.Id, articleId, saleLines);
            Assert.Null(issued.Error);

            var qr = await PayReceiptAndReadQrAsync(services, issued.DocumentId);
            assertPayload(qr);
            AssertStoredQrMatchesPdfSourceField(services, qr);
        }
        finally
        {
            Cleanup(root);
        }
    }

    private static void AssertStoredQrMatchesPdfSourceField(IServiceProvider services, string storedQr)
    {
        using var scope = services.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var fromDb = database.Receipts.AsNoTracking()
            .OrderByDescending(item => item.CreatedAt)
            .Select(item => item.QrCode)
            .First();
        Assert.Equal(storedQr, fromDb);
    }

    private static async Task<Guid> FirstArticleIdAsync(IServiceProvider services)
    {
        var catalog = await services.GetRequiredService<IPosCatalogService>().LoadAsync(CancellationToken.None);
        return catalog.Articles.First().Id;
    }

    private static async Task<string> PayReceiptAndReadQrAsync(IServiceProvider services, Guid documentId)
    {
        decimal outstanding;
        await using (var scope = services.CreateAsyncScope())
        {
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
            var document = await database.Documents.AsNoTracking()
                .FirstAsync(item => item.Id == documentId);
            var paid = await database.Payments.AsNoTracking()
                .Where(item => item.IsDeleted == false && item.DocumentId == documentId)
                .SumAsync(item => item.CreditAmount);
            outstanding = document.TotalFinal - paid;
        }

        var catalogPay = await services.GetRequiredService<IReceiptEmission>().CatalogAsync();
        var pay = await services.GetRequiredService<IReceiptEmission>().PayAsync(
            new ReceiptPayRequest
            {
                DocumentIds = [documentId],
                PaymentMethodId = catalogPay.PaymentMethods.First().Id,
                CurrencyId = catalogPay.Currencies.First().Id,
                Amount = outstanding,
                CurrencyAmount = outstanding,
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
            return qr!;
        }
    }

    private static async Task<PosDocumentResult> IssueDeferredFtAsync(
        IServiceProvider services,
        Guid customerId,
        Guid articleId,
        IReadOnlyList<PosSaleLine> lines)
    {
        Guid deferredPaymentId;
        await using (var scope = services.CreateAsyncScope())
        {
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
            deferredPaymentId = await database.PaymentConditions.AsNoTracking()
                .Where(item => item.IsDeleted == false && item.Acronym != "PRONTOPAGAMENTO")
                .Select(item => item.Id)
                .FirstAsync();
        }

        return await services.GetRequiredService<IPosDocumentService>().IssueDocumentAsync(
            "FT",
            customerId,
            lines,
            new PosDocumentHeader { PaymentConditionId = deferredPaymentId },
            cancellationToken: CancellationToken.None);
    }

    private static async Task<IServiceProvider> ConfigureWithFiscalPluginAsync(
        DirectoryInfo root,
        bool enablePtAcVat = false)
    {
        var plugins = Directory.CreateDirectory(Path.Combine(root.FullName, "plugins"));
        File.Copy(
            Path.Combine(AppContext.BaseDirectory, "SampleFiscalPlugin.dll"),
            Path.Combine(plugins.FullName, "SampleFiscalPlugin.dll"));

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
                    item.ATDocCodeValidationSeries = "TESTE160";
                }
            }

            if (enablePtAcVat)
            {
                var ptAc = await database.VatRates.FirstAsync(item => item.Id == VatNorPtAc18);
                ptAc.IsDeleted = false;
            }

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