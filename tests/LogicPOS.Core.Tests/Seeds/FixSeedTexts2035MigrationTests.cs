using LogicPOS.Core;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.DataFixes;
using LogicPOS.Persistence.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.Tests.Seeds;

public sealed class FixSeedTexts2035MigrationTests
{
    [Fact]
    public async Task Old_database_values_are_corrected_and_user_edits_are_preserved()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-fix-seed-old-");
        try
        {
            ConfigureApp(root, module: "cafe");
            var services = AppComposition.Services!;
            await using var scope = services.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();

            var transportToken = "COMPANY_COMMUNICATE_TRANSPORT_DOCUMENTS";
            var existingTransport = await database.PreferenceParameters
                .SingleAsync(item => item.Token == transportToken);

            // Simulate a second legacy row with the same token (different Id).
            database.PreferenceParameters.Add(ClonePreference(existingTransport, Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"), order: 116, code: "116"));

            var smtpUser = await database.PreferenceParameters.SingleAsync(item => item.Token == "SEND_MAIL_SMTP_USERNAME");
            smtpUser.Value = "YOUR-EMAIL@gmail.com";

            var smtpPassword = await database.PreferenceParameters.SingleAsync(item => item.Token == "SEND_MAIL_SMTP_PASSWORD");
            smtpPassword.Value = "YOUR-PASSWORD";

            var virtualMoney = await database.PaymentMethods.SingleAsync(item => item.Token == "VIRTUAL_MONEY");
            virtualMoney.Designation = "Dinheiro Eletrônico";

            var stockGain = await database.PermissionItems.SingleAsync(item => item.Token == "REPORT_LIST_STOCK_GAIN");
            stockGain.Designation = "__designation[4040]__";

            var mousse = await database.Articles.SingleAsync(item => item.Designation == "Mousse de Chocolate com Chantilly");
            mousse.Designation = "Mousse de Chocolate C Chantily";

            var toucinho = await database.Articles.SingleAsync(item => item.Designation == "Toucinho do Céu");
            toucinho.Designation = "Toucinho Do Ceu";

            var outras = await database.ArticleSubfamilies.SingleAsync(item => item.Designation == "Outros de cafetaria");
            outras.Designation = "Outras Caf";

            // Customer-edited value must not be rewritten by the conditional UPDATEs.
            var customArticle = await database.Articles
                .Where(item => item.IsDeleted == false)
                .OrderBy(item => item.Code)
                .FirstAsync();
            const string customerDesignation = "Artigo personalizado pelo cliente";
            customArticle.Designation = customerDesignation;

            await database.SaveChangesAsync();

            foreach (var sql in FixSeedTexts2035Statements.ForSqlite())
            {
                await database.Database.ExecuteSqlRawAsync(sql);
            }

            var transportCount = await database.PreferenceParameters.CountAsync(item => item.Token == transportToken);
            Assert.Equal(1, transportCount);

            await database.Entry(smtpUser).ReloadAsync();
            await database.Entry(smtpPassword).ReloadAsync();
            await database.Entry(virtualMoney).ReloadAsync();
            await database.Entry(stockGain).ReloadAsync();
            await database.Entry(mousse).ReloadAsync();
            await database.Entry(toucinho).ReloadAsync();
            await database.Entry(outras).ReloadAsync();
            await database.Entry(customArticle).ReloadAsync();

            Assert.Equal(string.Empty, smtpUser.Value);
            Assert.Equal(string.Empty, smtpPassword.Value);
            Assert.Equal("Dinheiro Eletrónico", virtualMoney.Designation);
            Assert.Equal("Relatório :: Stock por valorização de artigo", stockGain.Designation);
            Assert.Equal("Mousse de Chocolate com Chantilly", mousse.Designation);
            Assert.Equal("Toucinho do Céu", toucinho.Designation);
            Assert.Equal("Outros de cafetaria", outras.Designation);
            Assert.Equal(customerDesignation, customArticle.Designation);
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task New_database_keeps_corrected_seed_values_after_idempotent_fix()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-fix-seed-new-");
        try
        {
            ConfigureApp(root, module: "cafe");
            var services = AppComposition.Services!;
            await using var scope = services.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();

            var beforeTransport = await database.PreferenceParameters
                .CountAsync(item => item.Token == "COMPANY_COMMUNICATE_TRANSPORT_DOCUMENTS");
            var beforeVirtualMoney = await database.PaymentMethods
                .Where(item => item.Token == "VIRTUAL_MONEY")
                .Select(item => item.Designation)
                .SingleAsync();
            var beforeStockGain = await database.PermissionItems
                .Where(item => item.Token == "REPORT_LIST_STOCK_GAIN")
                .Select(item => item.Designation)
                .SingleAsync();
            var beforeToucinho = await database.Articles.CountAsync(item => item.Designation == "Toucinho do Céu");
            var beforeOutras = await database.ArticleSubfamilies.CountAsync(item => item.Designation == "Outros de cafetaria");
            var beforeSmtpUser = await database.PreferenceParameters
                .Where(item => item.Token == "SEND_MAIL_SMTP_USERNAME")
                .Select(item => item.Value)
                .SingleAsync();

            Assert.Equal(1, beforeTransport);
            Assert.Equal("Dinheiro Eletrónico", beforeVirtualMoney);
            Assert.Equal("Relatório :: Stock por valorização de artigo", beforeStockGain);
            Assert.True(beforeToucinho >= 1);
            Assert.True(beforeOutras >= 1);
            Assert.Equal(string.Empty, beforeSmtpUser);

            var appliedMigrations = await database.Database.GetAppliedMigrationsAsync();
            Assert.Contains(appliedMigrations, id => id == "20261123120000_FixSeedTexts2035");

            foreach (var sql in FixSeedTexts2035Statements.ForSqlite())
            {
                await database.Database.ExecuteSqlRawAsync(sql);
            }

            Assert.Equal(1, await database.PreferenceParameters.CountAsync(item => item.Token == "COMPANY_COMMUNICATE_TRANSPORT_DOCUMENTS"));
            Assert.Equal(
                "Dinheiro Eletrónico",
                await database.PaymentMethods.Where(item => item.Token == "VIRTUAL_MONEY").Select(item => item.Designation).SingleAsync());
            Assert.Equal(
                "Relatório :: Stock por valorização de artigo",
                await database.PermissionItems.Where(item => item.Token == "REPORT_LIST_STOCK_GAIN").Select(item => item.Designation).SingleAsync());
            Assert.Equal(beforeToucinho, await database.Articles.CountAsync(item => item.Designation == "Toucinho do Céu"));
            Assert.Equal(beforeOutras, await database.ArticleSubfamilies.CountAsync(item => item.Designation == "Outros de cafetaria"));
            Assert.Equal(
                string.Empty,
                await database.PreferenceParameters.Where(item => item.Token == "SEND_MAIL_SMTP_USERNAME").Select(item => item.Value).SingleAsync());
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public void MySql_SqlServer_and_PostgreSql_statements_are_generated()
    {
        var mysql = FixSeedTexts2035Statements.ForMySql().ToList();
        var sqlServer = FixSeedTexts2035Statements.ForSqlServer().ToList();
        var postgresql = FixSeedTexts2035Statements.ForPostgreSql().ToList();
        var sqlite = FixSeedTexts2035Statements.ForSqlite().ToList();

        Assert.NotEmpty(mysql);
        Assert.NotEmpty(sqlServer);
        Assert.NotEmpty(postgresql);
        Assert.Equal(259, sqlite.Count);
        Assert.Equal(sqlite.Count, mysql.Count);
        Assert.Equal(sqlite.Count, sqlServer.Count);
        Assert.Equal(sqlite.Count, postgresql.Count);
        Assert.Contains(mysql, sql => sql.Contains("KeepTransport", StringComparison.Ordinal));
        Assert.Contains(sqlServer, sql => sql.Contains("RankedTransport", StringComparison.Ordinal));
        Assert.Contains(sqlite, sql => sql.Contains("rowid", StringComparison.Ordinal));
        Assert.Contains(
            postgresql,
            sql => sql.Contains("ROW_NUMBER()", StringComparison.Ordinal)
                   && sql.Contains("\"PreferenceParameters\"", StringComparison.Ordinal)
                   && sql.Contains("COMPANY_COMMUNICATE_TRANSPORT_DOCUMENTS", StringComparison.Ordinal));
        Assert.Contains(
            postgresql,
            sql => sql.Contains("UPDATE \"Articles\" SET \"Designation\"", StringComparison.Ordinal));
        Assert.True(FixSeedTexts2035Statements.ArticleDesignations.Length >= 200);
        Assert.True(FixSeedTexts2035Statements.ArticleSubfamilyDesignations.Length >= 8);
    }

    private static PreferenceParameter ClonePreference(PreferenceParameter source, Guid id, uint order, string code)
        => new()
        {
            Id = id,
            Token = source.Token,
            Value = source.Value,
            ValueTip = source.ValueTip,
            Required = source.Required,
            RegEx = source.RegEx,
            ResourceString = source.ResourceString,
            ResourceStringInfo = source.ResourceStringInfo,
            FormType = source.FormType,
            FormPageNo = source.FormPageNo,
            InputType = source.InputType,
            Order = order,
            Code = code,
            Notes = source.Notes,
            CreatedAt = source.CreatedAt.AddMinutes(1),
            CreatedBy = source.CreatedBy,
            CreatedWhere = source.CreatedWhere,
            UpdatedAt = source.UpdatedAt.AddMinutes(1),
            UpdatedBy = source.UpdatedBy,
            UpdatedWhere = source.UpdatedWhere,
            IsDeleted = false,
            DeletedAt = source.DeletedAt
        };

    private static void ConfigureApp(DirectoryInfo root, string module)
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
                "UseCloud": false,
                "Module": "{{module}}"
              }
            }
            """);

        AppComposition.Configure(root.FullName);
        Assert.Null(AppComposition.StartupError);
        Assert.NotNull(AppComposition.Services);
    }

    private static void Cleanup(DirectoryInfo root)
    {
        if (AppComposition.Services is IDisposable disposable)
        {
            disposable.Dispose();
        }

        SqliteConnection.ClearAllPools();
        AppLogging.CloseAndFlush();

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
