using Microsoft.Data.Sqlite;
using LogicPOS.Core.BackOffice;
using LogicPOS.Core.Fiscal;
using LogicPOS.Core.Licensing;
using LogicPOS.Core.FrontOffice;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.Tests;

public sealed class PublicEditionStartupTests
{
    private static void CleanupTestDirectory(DirectoryInfo root)
    {
        AppLogging.CloseAndFlush();
        SqliteConnection.ClearAllPools();

        const int maxRetries = 5;
        const int delayMs = 200;

        for (int attempt = 1; attempt <= maxRetries; attempt++)
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
                Thread.Sleep(delayMs * attempt);
            }
            catch (UnauthorizedAccessException) when (attempt < maxRetries)
            {
                Thread.Sleep(delayMs * attempt);
            }
        }
    }

    [Fact]
    public void Startup_uses_a_direct_database_when_the_cloud_piece_is_absent()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-public-");
        try
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
                    "UseCloud": true
                  }
                }
                """);

            AppComposition.Configure(root.FullName);

            Assert.Null(AppComposition.StartupError);
            Assert.NotNull(AppComposition.Services);
            Assert.NotNull(AppComposition.Services.GetService<IPosDocumentService>());
            Assert.False(AppComposition.Services.GetRequiredService<IFiscalModule>().IsAvailable);
            Assert.False(AppComposition.Services.GetRequiredService<ILicenseModule>().RegistrationRequired);
        }
        finally
        {
            if (AppComposition.Services is IDisposable disposable)
            {
                disposable.Dispose();
            }

            CleanupTestDirectory(root);
        }
    }

    [Fact]
    public void Startup_loads_a_fiscal_plugin_from_the_plugins_folder()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-fiscal-plugin-");
        try
        {
            var plugins = Directory.CreateDirectory(Path.Combine(root.FullName, "plugins"));
            var built = Path.Combine(AppContext.BaseDirectory, "SampleFiscalPlugin.dll");
            File.Copy(built, Path.Combine(plugins.FullName, "SampleFiscalPlugin.dll"));
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
            var module = AppComposition.Services!.GetRequiredService<IFiscalModule>();
            Assert.True(module.IsAvailable);
            var mark = module.DescribePrint(new FiscalDocument { Number = "FT 1/1" });
            Assert.Equal("MARK FT 1/1", mark.CodeLine);
            Assert.Equal("QR FT 1/1", mark.QrPayload);
        }
        finally
        {
            if (AppComposition.Services is IDisposable disposable)
            {
                disposable.Dispose();
            }

            CleanupTestDirectory(root);
        }
    }

    [Fact]
    public void Startup_uses_a_direct_database_when_settings_name_a_cloud()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-named-cloud-");
        try
        {
            var databasePath = Path.Combine(root.FullName, "logicpos.db");
            var missing = Path.Combine(root.FullName, "missing", "LogicPOS.Cloud.dll");
            File.WriteAllText(
                Path.Combine(root.FullName, "appsettings.json"),
                $$"""
                {
                  "Culture": "pt-PT",
                  "DatabaseSettings": {
                    "DatabaseType": "Sqlite",
                    "ConnectionString": "Data Source={{databasePath.Replace("\\", "\\\\")}}",
                    "UseCloud": true
                  },
                  "LogicPOS": {
                    "Api": { "BaseAddress": "https://api.logic-pos.com/", "ClientId": "logicpulsept" },
                    "Cloud": { "AssemblyPath": "{{missing.Replace("\\", "\\\\")}}" }
                  }
                }
                """);

            AppComposition.Configure(root.FullName);

            Assert.Null(AppComposition.StartupError);
            Assert.NotNull(AppComposition.Services);
            Assert.NotNull(AppComposition.Services.GetService<IPosDocumentService>());
            Assert.False(AppComposition.Services.GetRequiredService<IFiscalModule>().IsAvailable);
            Assert.False(AppComposition.Services.GetRequiredService<ILicenseModule>().RegistrationRequired);
        }
        finally
        {
            if (AppComposition.Services is IDisposable disposable)
            {
                disposable.Dispose();
            }

            CleanupTestDirectory(root);
        }
    }

    [Fact]
    public void Startup_does_not_require_a_license_when_settings_name_one()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-named-license-");
        try
        {
            var databasePath = Path.Combine(root.FullName, "logicpos.db");
            var missing = Path.Combine(root.FullName, "missing", "LogicPOS.Licence.dll");
            File.WriteAllText(
                Path.Combine(root.FullName, "appsettings.json"),
                $$"""
                {
                  "Culture": "pt-PT",
                  "DatabaseSettings": {
                    "DatabaseType": "Sqlite",
                    "ConnectionString": "Data Source={{databasePath.Replace("\\", "\\\\")}}",
                    "UseCloud": false
                  },
                  "LogicPOS": { "License": { "AssemblyPath": "{{missing.Replace("\\", "\\\\")}}" } }
                }
                """);

            AppComposition.Configure(root.FullName);

            Assert.Null(AppComposition.StartupError);
            Assert.False(AppComposition.Services!.GetRequiredService<ILicenseModule>().RegistrationRequired);
        }
        finally
        {
            if (AppComposition.Services is IDisposable disposable)
            {
                disposable.Dispose();
            }

            CleanupTestDirectory(root);
        }
    }

    [Fact]
    public void Public_settings_have_no_api_address_or_client_id()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && File.Exists(Path.Combine(directory.FullName, "LogicPOS.App", "appsettings.json")) == false)
        {
            directory = directory.Parent;
        }

        Assert.NotNull(directory);
        var json = File.ReadAllText(Path.Combine(directory!.FullName, "LogicPOS.App", "appsettings.json"));
        Assert.DoesNotContain("BaseAddress", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("ClientId", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("api.logic-pos.com", json, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Issued_document_has_a_local_number_and_no_certification_marks()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-issued-");
        string? pdfPath = null;
        try
        {
            var services = await ConfigurePublicEditionReadyToIssueAsync(root);
            var customer = await services.GetRequiredService<IPosCustomerService>().GetFinalConsumerAsync(CancellationToken.None);
            Assert.NotNull(customer);
            var catalog = await services.GetRequiredService<IPosCatalogService>().LoadAsync(CancellationToken.None);
            var article = catalog.Articles.First();
            var documents = services.GetRequiredService<IPosDocumentService>();
            var issued = await documents.IssueDocumentAsync(
                "FS",
                customer.Id,
                [new PosSaleLine(article.Id, 1, article.CatalogPrice, 0, article.VatRateId)],
                new PosDocumentHeader { PaymentToken = "MONEY" },
                CancellationToken.None);

            Assert.Null(issued.Error);
            Assert.False(string.IsNullOrWhiteSpace(issued.Number));
            Assert.DoesNotContain("ATCUD", issued.Number, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("TESTELOCAL", issued.Number, StringComparison.OrdinalIgnoreCase);

            pdfPath = await documents.CreateA4FileAsync(issued.DocumentId, CancellationToken.None);
            var pdf = ReadPdfText(pdfPath!);
            Assert.DoesNotContain("2543/AT", pdf, StringComparison.Ordinal);
            Assert.DoesNotContain("221/AGT", pdf, StringComparison.Ordinal);
            Assert.DoesNotContain("ATCUD", pdf, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("TESTELOCAL", pdf, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("programa certificado", pdf, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("programa validado", pdf, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            CleanupPublicEdition(root, pdfPath);
        }
    }

    [Fact]
    public async Task Customer_search_matches_a_partial_name_ignoring_accents()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-customer-search-");
        try
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
            await using (var scope = services.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<LogicPOS.Persistence.Database.LogicPOSDbContext>();
                var customer = database.Customers.First(item => item.IsDeleted == false);
                customer.Name = "João Silva";
                await database.SaveChangesAsync();
            }

            var customers = services.GetRequiredService<IPosCustomerService>();
            var matches = await customers.SearchAsync("joao");
            Assert.Contains(matches, item => item.Name == "João Silva");
            var byAccent = await customers.SearchAsync("JOÃO");
            Assert.Contains(byAccent, item => item.Name == "João Silva");
            var none = await customers.SearchAsync("nenhum-cliente-xyz");
            Assert.Empty(none);
        }
        finally
        {
            if (AppComposition.Services is IDisposable disposable)
            {
                disposable.Dispose();
            }

            SqliteConnection.ClearAllPools();
            root.Delete(recursive: true);
        }
    }

    [Fact]
    public async Task Issued_document_pdf_prints_atcud_when_document_has_valid_atcud()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-atcud-");
        string? pdfPath = null;
        try
        {
            var services = await ConfigurePublicEditionReadyToIssueAsync(root);
            // Seed a real AT validation code on the FS series so AssignAtcud stores "ABC1234X-1".
            await using (var scope = services.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
                var series = await database.DocumentSeries
                    .Include(item => item.DocumentType)
                    .FirstAsync(item => item.IsDeleted == false && item.DocumentType!.Acronym == "FS");
                series.ATDocCodeValidationSeries = "ABC1234X";
                await database.SaveChangesAsync();
            }

            var customer = await services.GetRequiredService<IPosCustomerService>().GetFinalConsumerAsync(CancellationToken.None);
            Assert.NotNull(customer);
            var catalog = await services.GetRequiredService<IPosCatalogService>().LoadAsync(CancellationToken.None);
            var article = catalog.Articles.First();
            var documents = services.GetRequiredService<IPosDocumentService>();
            var issued = await documents.IssueDocumentAsync(
                "FS",
                customer.Id,
                [new PosSaleLine(article.Id, 1, article.CatalogPrice, 0, article.VatRateId)],
                new PosDocumentHeader { PaymentToken = "MONEY" },
                CancellationToken.None);

            Assert.Null(issued.Error);
            pdfPath = await documents.CreateA4FileAsync(issued.DocumentId, CancellationToken.None);
            var pdf = ReadPdfText(pdfPath!);
            Assert.Contains("ATCUD: ABC1234X-1", pdf, StringComparison.Ordinal);
        }
        finally
        {
            CleanupPublicEdition(root, pdfPath);
        }
    }

    /// <summary>
    /// Company setup + fiscal-year wizard from the public-edition first-run paths.
    /// </summary>
    private static async Task<IServiceProvider> ConfigurePublicEditionReadyToIssueAsync(DirectoryInfo root)
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
        // Fresh installs leave company prefs empty; LoginWindow prompts CompanySetup first.
        var company = services.GetRequiredService<ICompanySetupService>();
        var companyDraft = await company.LoadAsync(CancellationToken.None);
        var companySaved = await company.SaveAsync(new CompanySetupRequest
        {
            CountryId = companyDraft.CountryId ?? Guid.Empty,
            CurrencyId = companyDraft.CurrencyId ?? Guid.Empty,
            Values = LocalCompanySetupService.DemoCompanyValues
        }, CancellationToken.None);
        Assert.True(companySaved.Succeeded, companySaved.Error);

        // DocumentSeries are not seeded; production creates them via
        // "Abertura de ano fiscal" (IFiscalYearWizard) before IssueDocumentAsync.
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

    private static void CleanupPublicEdition(DirectoryInfo root, string? pdfPath)
    {
        if (pdfPath is not null && File.Exists(pdfPath))
        {
            File.Delete(pdfPath);
        }

        if (AppComposition.Services is IDisposable disposable)
        {
            disposable.Dispose();
        }

        SqliteConnection.ClearAllPools();
        root.Delete(recursive: true);
    }

    private static string ReadPdfText(string path)
    {
        using var document = UglyToad.PdfPig.PdfDocument.Open(path);
        return string.Concat(document.GetPages().Select(page => page.Text));
    }
}
