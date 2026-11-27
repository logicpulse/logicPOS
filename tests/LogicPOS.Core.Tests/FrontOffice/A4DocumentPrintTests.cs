using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using LogicPOS.Core.FrontOffice;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite;

namespace LogicPOS.Core.Tests.FrontOffice;

public sealed class A4DocumentPrintTests
{
    [Fact]
    public async Task Registered_A4_print_makes_second_pdf_generation_show_segunda_via()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-a4-print-");
        string? pdfFirst = null;
        string? pdfSecond = null;
        try
        {
            var services = await ConfigureReadyToIssueFtAsync(root);
            var customer = await services.GetRequiredService<IPosCustomerService>().GetFinalConsumerAsync(CancellationToken.None);
            Assert.NotNull(customer);
            var catalog = await services.GetRequiredService<IPosCatalogService>().LoadAsync(CancellationToken.None);
            var article = catalog.Articles.First();
            var documents = services.GetRequiredService<IPosDocumentService>();
            var issued = await documents.IssueDocumentAsync(
                "FT",
                customer.Id,
                [new PosSaleLine(article.Id, 1, article.CatalogPrice, 0, article.VatRateId)],
                new PosDocumentHeader { PaymentToken = "MONEY" },
                cancellationToken: CancellationToken.None);

            Assert.Null(issued.Error);

            pdfFirst = await documents.CreateA4FileAsync(issued.DocumentId, CancellationToken.None);
            Assert.NotNull(pdfFirst);
            var firstText = ReadPdfText(pdfFirst);
            Assert.DoesNotContain("2ª Via", firstText, StringComparison.Ordinal);

            var printSource = services.GetRequiredService<IThermalPrintSource>();
            await printSource.RegisterPrintAsync(issued.DocumentId, isThermalPrint: false);

            pdfSecond = await documents.CreateA4FileAsync(issued.DocumentId, CancellationToken.None);
            Assert.NotNull(pdfSecond);
            var secondText = ReadPdfText(pdfSecond);
            Assert.Contains("2ª Via", secondText, StringComparison.Ordinal);
        }
        finally
        {
            Cleanup(root, pdfFirst, pdfSecond);
        }
    }

    [Fact]
    public async Task A4_reprint_registers_second_print_and_motive()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-a4-reprint-");
        try
        {
            var services = await ConfigureReadyToIssueFtAsync(root);
            var customer = await services.GetRequiredService<IPosCustomerService>().GetFinalConsumerAsync(CancellationToken.None);
            Assert.NotNull(customer);
            var catalog = await services.GetRequiredService<IPosCatalogService>().LoadAsync(CancellationToken.None);
            var article = catalog.Articles.First();
            var documents = services.GetRequiredService<IPosDocumentService>();
            var issued = await documents.IssueDocumentAsync(
                "FT",
                customer.Id,
                [new PosSaleLine(article.Id, 1, article.CatalogPrice, 0, article.VatRateId)],
                new PosDocumentHeader { PaymentToken = "MONEY" },
                cancellationToken: CancellationToken.None);

            Assert.Null(issued.Error);

            var printSource = services.GetRequiredService<IThermalPrintSource>();
            await printSource.RegisterPrintAsync(issued.DocumentId, isThermalPrint: false);
            await printSource.RegisterReprintAsync(issued.DocumentId, 2, "Teste motivo", isThermalPrint: false, secondPrint: true);

            await using var scope = services.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
            var prints = await database.DocumentPrints
                .AsNoTracking()
                .Where(item => item.IsDeleted == false && item.DocumentId == issued.DocumentId)
                .OrderBy(item => item.CreatedAt)
                .ToListAsync();

            Assert.Equal(2, prints.Count);
            Assert.False(prints[0].SecondPrint);
            Assert.False(prints[0].IsThermalPrint);
            Assert.True(prints[1].SecondPrint);
            Assert.False(prints[1].IsThermalPrint);
            Assert.Equal("Teste motivo", prints[1].PrintMotive);
            Assert.Equal(2, prints[1].PrintCopies);
        }
        finally
        {
            Cleanup(root, null, null);
        }
    }

    [Fact]
    public async Task WasPrinted_is_false_until_first_A4_registration()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-wasprinted-");
        try
        {
            var services = await ConfigureReadyToIssueFtAsync(root);
            var customer = await services.GetRequiredService<IPosCustomerService>().GetFinalConsumerAsync(CancellationToken.None);
            Assert.NotNull(customer);
            var catalog = await services.GetRequiredService<IPosCatalogService>().LoadAsync(CancellationToken.None);
            var article = catalog.Articles.First();
            var documents = services.GetRequiredService<IPosDocumentService>();
            var issued = await documents.IssueDocumentAsync(
                "FT",
                customer.Id,
                [new PosSaleLine(article.Id, 1, article.CatalogPrice, 0, article.VatRateId)],
                new PosDocumentHeader { PaymentToken = "MONEY" },
                cancellationToken: CancellationToken.None);

            Assert.Null(issued.Error);
            var printSource = services.GetRequiredService<IThermalPrintSource>();
            Assert.False(await printSource.WasPrintedAsync(issued.DocumentId));
            await printSource.RegisterPrintAsync(issued.DocumentId, isThermalPrint: false);
            Assert.True(await printSource.WasPrintedAsync(issued.DocumentId));

            var secondPdf = await documents.CreateA4FileAsync(issued.DocumentId, CancellationToken.None);
            Assert.NotNull(secondPdf);
            var secondText = ReadPdfText(secondPdf);
            Assert.Contains("2ª Via", secondText, StringComparison.Ordinal);
        }
        finally
        {
            Cleanup(root, null, null);
        }
    }

    [Fact]
    public async Task Second_A4_registration_after_wasPrinted_uses_second_print_flag()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-second-flag-");
        try
        {
            var services = await ConfigureReadyToIssueFtAsync(root);
            var customer = await services.GetRequiredService<IPosCustomerService>().GetFinalConsumerAsync(CancellationToken.None);
            Assert.NotNull(customer);
            var catalog = await services.GetRequiredService<IPosCatalogService>().LoadAsync(CancellationToken.None);
            var article = catalog.Articles.First();
            var documents = services.GetRequiredService<IPosDocumentService>();
            var issued = await documents.IssueDocumentAsync(
                "FT",
                customer.Id,
                [new PosSaleLine(article.Id, 1, article.CatalogPrice, 0, article.VatRateId)],
                new PosDocumentHeader { PaymentToken = "MONEY" },
                cancellationToken: CancellationToken.None);

            Assert.Null(issued.Error);
            var printSource = services.GetRequiredService<IThermalPrintSource>();
            await printSource.RegisterPrintAsync(issued.DocumentId, isThermalPrint: false);
            Assert.True(await printSource.WasPrintedAsync(issued.DocumentId));
            await printSource.RegisterReprintAsync(
                issued.DocumentId,
                1,
                "Segunda via",
                isThermalPrint: false,
                secondPrint: true);

            await using var scope = services.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
            var last = await database.DocumentPrints
                .AsNoTracking()
                .Where(item => item.IsDeleted == false && item.DocumentId == issued.DocumentId)
                .OrderByDescending(item => item.CreatedAt)
                .FirstAsync();

            Assert.True(last.SecondPrint);
            Assert.False(last.IsThermalPrint);
        }
        finally
        {
            Cleanup(root, null, null);
        }
    }

    [Fact]
    public async Task A4_reprint_registration_stores_copy_count_from_spooler()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-reprint-copies-");
        try
        {
            var services = await ConfigureReadyToIssueFtAsync(root);
            var customer = await services.GetRequiredService<IPosCustomerService>().GetFinalConsumerAsync(CancellationToken.None);
            Assert.NotNull(customer);
            var catalog = await services.GetRequiredService<IPosCatalogService>().LoadAsync(CancellationToken.None);
            var article = catalog.Articles.First();
            var documents = services.GetRequiredService<IPosDocumentService>();
            var issued = await documents.IssueDocumentAsync(
                "FT",
                customer.Id,
                [new PosSaleLine(article.Id, 1, article.CatalogPrice, 0, article.VatRateId)],
                new PosDocumentHeader { PaymentToken = "MONEY" },
                cancellationToken: CancellationToken.None);

            Assert.Null(issued.Error);
            var printSource = services.GetRequiredService<IThermalPrintSource>();
            await printSource.RegisterPrintAsync(issued.DocumentId, isThermalPrint: false);
            await printSource.RegisterReprintAsync(
                issued.DocumentId,
                1,
                "Uma cópia",
                isThermalPrint: false,
                secondPrint: true);

            await using var scope = services.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
            var last = await database.DocumentPrints
                .AsNoTracking()
                .Where(item => item.IsDeleted == false && item.DocumentId == issued.DocumentId)
                .OrderByDescending(item => item.CreatedAt)
                .FirstAsync();

            Assert.Equal(1, last.PrintCopies);
        }
        finally
        {
            Cleanup(root, null, null);
        }
    }

    private static async Task<IServiceProvider> ConfigureReadyToIssueFtAsync(DirectoryInfo root)
    {
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

    private static void Cleanup(DirectoryInfo root, string? pdfFirst, string? pdfSecond)
    {
        foreach (var path in new[] { pdfFirst, pdfSecond })
        {
            if (path is not null && File.Exists(path))
            {
                File.Delete(path);
            }
        }

        if (AppComposition.Services is IDisposable disposable)
        {
            disposable.Dispose();
        }

        AppLogging.CloseAndFlush();
        SqliteConnection.ClearAllPools();
        try
        {
            root.Delete(true);
        }
        catch
        {
            // Temp directories may stay locked briefly on Linux CI.
        }
    }

    private static string ReadPdfText(string path)
    {
        using var document = UglyToad.PdfPig.PdfDocument.Open(path);
        return string.Concat(document.GetPages().Select(page => page.Text));
    }
}