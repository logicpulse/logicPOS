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
    public void Startup_fails_when_the_cloud_piece_path_is_missing()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-missing-cloud-");
        try
        {
            var missing = Path.Combine(root.FullName, "missing", "LogicPOS.Cloud.dll");
            File.WriteAllText(
                Path.Combine(root.FullName, "appsettings.json"),
                $$"""
                {
                  "Culture": "pt-PT",
                  "DatabaseSettings": { "UseCloud": true },
                  "LogicPOS": { "Cloud": { "AssemblyPath": "{{missing.Replace("\\", "\\\\")}}" } }
                }
                """);

            AppComposition.Configure(root.FullName);

            Assert.NotNull(AppComposition.StartupError);
            Assert.Contains(
                "Não encontrei LogicPOS.Cloud.dll em LogicPOS:Cloud:AssemblyPath.",
                AppComposition.StartupError);
        }
        finally
        {
            if (AppComposition.Services is IDisposable disposable)
            {
                disposable.Dispose();
            }

            root.Delete(recursive: true);
        }
    }

    [Fact]
    public void Startup_fails_when_the_license_piece_path_is_missing()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-missing-license-");
        try
        {
            var missing = Path.Combine(root.FullName, "missing", "LogicPOS.Licence.dll");
            File.WriteAllText(
                Path.Combine(root.FullName, "appsettings.json"),
                $$"""
                {
                  "Culture": "pt-PT",
                  "DatabaseSettings": { "UseCloud": false },
                  "LogicPOS": { "License": { "AssemblyPath": "{{missing.Replace("\\", "\\\\")}}" } }
                }
                """);

            AppComposition.Configure(root.FullName);

            Assert.Contains(
                "Não encontrei LogicPOS.Licence.dll em LogicPOS:License:AssemblyPath.",
                AppComposition.StartupError);
        }
        finally
        {
            if (AppComposition.Services is IDisposable disposable)
            {
                disposable.Dispose();
            }

            root.Delete(recursive: true);
        }
    }
}
