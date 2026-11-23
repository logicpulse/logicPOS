using LogicPOS.Core;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Entities.Articles.Articles.Common;
using LogicPOS.Persistence.DataFixes;
using LogicPOS.Persistence.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.Tests.Seeds;

[Collection(LogicPOS.Core.Tests.AppCompositionCollection.Name)]
public sealed class FixSeedTexts2035R2MigrationTests
{
    [Fact]
    public async Task R2_statements_fix_texts_undelete_interno_and_soft_delete_orphan_final_consumer()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-fix-seed-r2-");
        try
        {
            ConfigureApp(root, module: "cafe");
            var services = AppComposition.Services!;
            await using var scope = services.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();

            var sdr = await database.Articles.FirstOrDefaultAsync(item => item.Code == SdrConstants.SdrArticleCode)
                ?? await database.Articles.Where(item => item.IsDeleted == false).OrderBy(item => item.Code).FirstAsync();
            sdr.Designation = FixSeedTexts2035R2Statements.OldSdrArticleDesignation;

            var articleClass = await database.ArticleClasses.SingleAsync(item =>
                item.Designation == FixSeedTexts2035R2Statements.NewArticleClassDesignation
                || item.Designation == FixSeedTexts2035R2Statements.OldArticleClassDesignation);
            articleClass.Designation = FixSeedTexts2035R2Statements.OldArticleClassDesignation;

            var mailSubject = await database.PreferenceParameters.SingleAsync(item =>
                item.Token == "SEND_MAIL_FINANCE_DOCUMENTS_SUBJECT");
            mailSubject.Value = FixSeedTexts2035R2Statements.OldMailSubject;

            var mailBody = await database.PreferenceParameters.SingleAsync(item =>
                item.Token == "SEND_MAIL_FINANCE_DOCUMENTS_BODY");
            mailBody.Value = FixSeedTexts2035R2Statements.OldMailBody;

            var interno = await database.Customers.SingleAsync(item =>
                item.Id == FixSeedTexts2035R2Statements.InternoSupplierId);
            interno.IsDeleted = true;
            interno.Supplier = true;

            var seedConsumer = await database.Customers.SingleAsync(item =>
                item.Id == CustomerWellKnownIds.FinalConsumer);
            var orphanDuplicate = CloneCustomer(seedConsumer, Guid.Parse("11111111-2222-3333-4444-555555555555"), code: "11");
            database.Customers.Add(orphanDuplicate);
            await database.SaveChangesAsync();

            var softDeleteSql = FixSeedTexts2035R2Statements.ForSqlite()
                .Single(sql => sql.Contains("SELECT k.", StringComparison.Ordinal));
            Assert.Contains("NOT EXISTS (SELECT 1 FROM Documents", softDeleteSql, StringComparison.Ordinal);

            foreach (var sql in FixSeedTexts2035R2Statements.ForSqlite())
            {
                // ExecuteSqlRaw treats {…} as format holes; mail templates contain ${DOCUMENT_LIST} etc.
                var escaped = sql.Replace("{", "{{", StringComparison.Ordinal).Replace("}", "}}", StringComparison.Ordinal);
                await database.Database.ExecuteSqlRawAsync(escaped);
            }

            await database.Entry(sdr).ReloadAsync();
            await database.Entry(articleClass).ReloadAsync();
            await database.Entry(mailSubject).ReloadAsync();
            await database.Entry(mailBody).ReloadAsync();
            await database.Entry(interno).ReloadAsync();
            await database.Entry(orphanDuplicate).ReloadAsync();
            await database.Entry(seedConsumer).ReloadAsync();

            Assert.Equal(FixSeedTexts2035R2Statements.NewSdrArticleDesignation, sdr.Designation);
            Assert.Equal(FixSeedTexts2035R2Statements.NewArticleClassDesignation, articleClass.Designation);
            Assert.Equal(FixSeedTexts2035R2Statements.NewMailSubject, mailSubject.Value);
            Assert.Equal(FixSeedTexts2035R2Statements.NewMailBody, mailBody.Value);
            Assert.False(interno.IsDeleted);
            Assert.True(orphanDuplicate.IsDeleted);
            Assert.False(seedConsumer.IsDeleted);
            Assert.Equal(
                1,
                await database.Customers.CountAsync(item =>
                    item.IsDeleted == false && item.Name == FixSeedTexts2035R2Statements.FinalConsumerName));
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public void R2_provider_statements_are_generated()
    {
        var sqlite = FixSeedTexts2035R2Statements.ForSqlite().ToList();
        var mysql = FixSeedTexts2035R2Statements.ForMySql().ToList();
        var sqlServer = FixSeedTexts2035R2Statements.ForSqlServer().ToList();
        var postgresql = FixSeedTexts2035R2Statements.ForPostgreSql().ToList();

        Assert.Equal(6, sqlite.Count);
        Assert.Equal(sqlite.Count, mysql.Count);
        Assert.Equal(sqlite.Count, sqlServer.Count);
        Assert.Equal(sqlite.Count, postgresql.Count);
        Assert.Contains(sqlite, sql => sql.Contains("Depósito", StringComparison.Ordinal));
        Assert.Contains(sqlite, sql => sql.Contains("exceto", StringComparison.Ordinal));
        Assert.Contains(sqlite, sql => sql.Contains("SELECT k.", StringComparison.Ordinal));
        Assert.Contains(postgresql, sql => sql.Contains("::uuid", StringComparison.Ordinal));
    }

    private static Customer CloneCustomer(Customer source, Guid id, string code) =>
        new()
        {
            Id = id,
            Code = code,
            Order = source.Order,
            Name = source.Name,
            FiscalNumber = source.FiscalNumber,
            Address = source.Address,
            Locality = source.Locality,
            ZipCode = source.ZipCode,
            City = source.City,
            CountryId = source.CountryId,
            CustomerTypeId = source.CustomerTypeId,
            PriceTypeId = source.PriceTypeId,
            DiscountGroupId = source.DiscountGroupId,
            DiscountType = source.DiscountType,
            Discount = source.Discount,
            CardCredit = source.CardCredit,
            Supplier = false,
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
        SqliteConnection.ClearAllPools();
        AppLogging.CloseAndFlush();
        if (root.Exists)
        {
            try
            {
                root.Delete(recursive: true);
            }
            catch (IOException)
            {
            }
        }
    }
}
