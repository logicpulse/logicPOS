using Microsoft.Data.Sqlite;
using LogicPOS.Core.Fiscal;
using LogicPOS.Core.Licensing;
using LogicPOS.Core.FrontOffice;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.Tests;

public sealed class PublicEditionStartupTests
{
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

            SqliteConnection.ClearAllPools();
            root.Delete(recursive: true);
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

            SqliteConnection.ClearAllPools();
            root.Delete(recursive: true);
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

            SqliteConnection.ClearAllPools();
            root.Delete(recursive: true);
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

            SqliteConnection.ClearAllPools();
            root.Delete(recursive: true);
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
    }

    private static string ReadPdfText(string path)
    {
        using var document = UglyToad.PdfPig.PdfDocument.Open(path);
        return string.Concat(document.GetPages().Select(page => page.Text));
    }
}
