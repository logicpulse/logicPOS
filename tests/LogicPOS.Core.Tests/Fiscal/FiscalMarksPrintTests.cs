using LogicPOS.Core.BackOffice;
using LogicPOS.Core.Fiscal;
using LogicPOS.Core.FrontOffice;
using LogicPOS.Persistence.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UglyToad.PdfPig;

namespace LogicPOS.Core.Tests.Fiscal;

[Collection(AppCompositionCollection.Name)]
public sealed class FiscalMarksPrintTests
{
    [Fact]
    public async Task Certified_fs_ticket_pdf_shows_atcud_and_fiscal_line_from_plugin()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-fiscal-print-");
        string? pdfPath = null;
        try
        {
            var services = await ConfigureWithFiscalPluginAsync(root);

            await using (var scope = services.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
                var series = await database.DocumentSeries
                    .Include(item => item.DocumentType)
                    .FirstAsync(item => item.IsDeleted == false && item.DocumentType!.Acronym == "FS");
                series.ATDocCodeValidationSeries = "TESTE005";
                await database.SaveChangesAsync();
            }

            var issued = await IssueSimplifiedAsync(services);
            Assert.Null(issued.Error);

            await using (var scope = services.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
                var document = await database.Documents.AsNoTracking()
                    .FirstAsync(item => item.Id == issued.DocumentId);
                Assert.StartsWith("QR ", document.ATQRCode, StringComparison.Ordinal);
                Assert.NotEqual(document.Number, document.ATQRCode);
            }

            pdfPath = await services.GetRequiredService<IPosDocumentService>()
                .CreateA4FileAsync(issued.DocumentId, CancellationToken.None);
            Assert.NotNull(pdfPath);
            var pdf = ReadPdfText(pdfPath);
            Assert.Contains("ATCUD: TESTE005-", pdf, StringComparison.Ordinal);
            Assert.Contains("MARK ", pdf, StringComparison.Ordinal);
        }
        finally
        {
            Cleanup(root, pdfPath);
        }
    }

    [Theory]
    [InlineData(false, "A:1*B:2*", false, false)]
    [InlineData(false, "A:1*B:2*", true, true)]
    [InlineData(true, "A:1*B:2*", false, true)]
    [InlineData(true, "A:1*B:2*", true, true)]
    [InlineData(true, null, true, false)]
    public void ShouldPrintQrImage_respects_certified_portugal_edition(
        bool printQr,
        string? payload,
        bool portugalCertified,
        bool expected)
    {
        Assert.Equal(expected, ThermalFiscalPrintRules.ShouldPrintQrImage(printQr, payload, portugalCertified));
    }

    [Fact]
    public async Task Certified_fs_fiscal_text_remains_when_qr_printing_preference_is_off()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-fiscal-noqr-");
        string? pdfPath = null;
        try
        {
            var services = await ConfigureWithFiscalPluginAsync(root);

            await using (var scope = services.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
                var series = await database.DocumentSeries
                    .Include(item => item.DocumentType)
                    .FirstAsync(item => item.IsDeleted == false && item.DocumentType!.Acronym == "FS");
                series.ATDocCodeValidationSeries = "TESTE005";
                await database.SaveChangesAsync();
            }

            var issued = await IssueSimplifiedAsync(services);
            Assert.Null(issued.Error);

            await using (var scope = services.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
                var document = await database.Documents.AsNoTracking()
                    .FirstAsync(item => item.Id == issued.DocumentId);
                var atcud = AtcudFormat.OrFallback(document.ATCUD, "TESTE005", document.Number);
                Assert.True(AtcudFormat.ShouldPrint(atcud));
                Assert.False(string.IsNullOrWhiteSpace(document.ATQRCode));
                Assert.True(ThermalFiscalPrintRules.ShouldPrintQrImage(
                    printQrCodePreference: false,
                    document.ATQRCode,
                    portugalCertifiedFiscalEdition: true));
            }

            pdfPath = await services.GetRequiredService<IPosDocumentService>()
                .CreateA4FileAsync(issued.DocumentId, CancellationToken.None);
            Assert.NotNull(pdfPath);
            var pdf = ReadPdfText(pdfPath);
            Assert.Contains("ATCUD: TESTE005-", pdf, StringComparison.Ordinal);
            Assert.Contains("MARK ", pdf, StringComparison.Ordinal);
        }
        finally
        {
            Cleanup(root, pdfPath);
        }
    }

    [Fact]
    public async Task Public_edition_fs_pdf_has_no_atcud_or_fiscal_marks()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-fiscal-public-print-");
        string? pdfPath = null;
        try
        {
            var services = await ConfigurePublicEditionAsync(root);
            var issued = await IssueSimplifiedAsync(services);
            Assert.Null(issued.Error);

            pdfPath = await services.GetRequiredService<IPosDocumentService>()
                .CreateA4FileAsync(issued.DocumentId, CancellationToken.None);
            Assert.NotNull(pdfPath);
            var pdf = ReadPdfText(pdfPath);
            Assert.DoesNotContain("ATCUD:", pdf, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("MARK ", pdf, StringComparison.Ordinal);
            Assert.DoesNotContain("programa certificado", pdf, StringComparison.OrdinalIgnoreCase);
            Assert.False(ThermalFiscalPrintRules.IsPortugalCertifiedFiscalEdition(
                services.GetRequiredService<IFiscalModule>(),
                AppComposition.Services!.GetRequiredService<LogicPOS.Application.Features.System.ISystemInformationService>().IsPortugal()));
        }
        finally
        {
            Cleanup(root, pdfPath);
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

        await WriteAppSettingsAsync(root);
        AppComposition.Configure(root.FullName);
        Assert.Null(AppComposition.StartupError);
        var services = AppComposition.Services;
        Assert.NotNull(services);
        Assert.True(services.GetRequiredService<IFiscalModule>().IsAvailable);

        await FinishCompanyAndFiscalYearAsync(services);
        return services;
    }

    private static async Task<IServiceProvider> ConfigurePublicEditionAsync(DirectoryInfo root)
    {
        await WriteAppSettingsAsync(root);
        AppComposition.Configure(root.FullName);
        Assert.Null(AppComposition.StartupError);
        var services = AppComposition.Services;
        Assert.NotNull(services);
        Assert.False(services.GetRequiredService<IFiscalModule>().IsAvailable);

        await FinishCompanyAndFiscalYearAsync(services);
        return services;
    }

    private static async Task WriteAppSettingsAsync(DirectoryInfo root)
    {
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
    }

    private static async Task FinishCompanyAndFiscalYearAsync(IServiceProvider services)
    {
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
    }

    private static string ReadPdfText(string path)
    {
        using var document = PdfDocument.Open(path);
        return string.Concat(document.GetPages().Select(page => page.Text));
    }

    private static void Cleanup(DirectoryInfo root, string? pdfPath)
    {
        if (string.IsNullOrWhiteSpace(pdfPath) == false && File.Exists(pdfPath))
        {
            try
            {
                File.Delete(pdfPath);
            }
            catch (IOException)
            {
            }
        }

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