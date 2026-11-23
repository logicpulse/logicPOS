using LogicPOS.Core;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Entities.Articles.Articles.Common;
using LogicPOS.Domain.Entities.Common;
using LogicPOS.Domain.ValueObjects;
using LogicPOS.Persistence.DataFixes;
using LogicPOS.Persistence.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.Tests.Seeds;

[Collection(LogicPOS.Core.Tests.AppCompositionCollection.Name)]
public sealed class FixSeedTexts2035R3MigrationTests
{
    [Fact]
    public async Task Old_database_updates_sdr_subfamily_and_reviewer_article_names()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-fix-seed-r3-old-");
        try
        {
            ConfigureApp(root, module: "restaurant");
            await using var scope = AppComposition.Services!.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();

            var sdr = await database.Articles.SingleAsync(item => item.Code == SdrConstants.SdrArticleCode);
            var subfamily = await database.ArticleSubfamilies.SingleAsync(item => item.Id == sdr.SubfamilyId);
            subfamily.Designation = FixSeedTexts2035R3Statements.OldSdrSubfamilyDesignation;

            foreach (var (id, oldValue, _) in FixSeedTexts2035R3Statements.ArticleDesignations)
            {
                var article = await database.Articles.SingleAsync(item => item.Id == id);
                article.Designation = oldValue;
            }

            await database.SaveChangesAsync();
            await ApplyR3SqliteAsync(database);

            await database.Entry(subfamily).ReloadAsync();
            Assert.Equal(FixSeedTexts2035R3Statements.NewSdrSubfamilyDesignation, subfamily.Designation);

            foreach (var (id, _, newValue) in FixSeedTexts2035R3Statements.ArticleDesignations)
            {
                var article = await database.Articles.SingleAsync(item => item.Id == id);
                await database.Entry(article).ReloadAsync();
                Assert.Equal(newValue, article.Designation);
            }
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task New_database_r3_statements_are_noop()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-fix-seed-r3-new-");
        try
        {
            ConfigureApp(root, module: "restaurant");
            await using var scope = AppComposition.Services!.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();

            var sdr = await database.Articles.SingleAsync(item => item.Code == SdrConstants.SdrArticleCode);
            var subfamily = await database.ArticleSubfamilies.SingleAsync(item => item.Id == sdr.SubfamilyId);
            Assert.Equal(FixSeedTexts2035R3Statements.NewSdrSubfamilyDesignation, subfamily.Designation);

            foreach (var (id, _, newValue) in FixSeedTexts2035R3Statements.ArticleDesignations)
            {
                var article = await database.Articles.SingleAsync(item => item.Id == id);
                Assert.Equal(newValue, article.Designation);
            }

            await ApplyR3SqliteAsync(database);

            await database.Entry(subfamily).ReloadAsync();
            Assert.Equal(FixSeedTexts2035R3Statements.NewSdrSubfamilyDesignation, subfamily.Designation);
            foreach (var (id, _, newValue) in FixSeedTexts2035R3Statements.ArticleDesignations)
            {
                var article = await database.Articles.SingleAsync(item => item.Id == id);
                Assert.Equal(newValue, article.Designation);
            }
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Customer_subfamily_with_same_old_text_but_different_id_is_not_changed()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-fix-seed-r3-customer-");
        try
        {
            ConfigureApp(root, module: "cafe");
            await using var scope = AppComposition.Services!.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();

            var sdr = await database.Articles.SingleAsync(item => item.Code == SdrConstants.SdrArticleCode);
            var seedSubfamily = await database.ArticleSubfamilies.SingleAsync(item => item.Id == sdr.SubfamilyId);
            seedSubfamily.Designation = FixSeedTexts2035R3Statements.OldSdrSubfamilyDesignation;

            var customerId = Guid.Parse("FEEDFEED-AAAA-BBBB-CCCC-DDDDDDDDDDDD");
            database.ArticleSubfamilies.Add(new ArticleSubfamily
            {
                Id = customerId,
                FamilyId = seedSubfamily.FamilyId,
                Code = "CUSTDEP",
                Designation = FixSeedTexts2035R3Statements.OldSdrSubfamilyDesignation,
                Order = 99991,
                Button = new Button(),
                VatDirectSellingId = seedSubfamily.VatDirectSellingId,
                CreatedAt = DateTime.Now,
                CreatedBy = Guid.Empty,
                CreatedWhere = Guid.Empty,
                UpdatedAt = DateTime.Now,
                UpdatedBy = Guid.Empty,
                UpdatedWhere = Guid.Empty,
                IsDeleted = false,
                DeletedAt = default
            });

            var tarteId = Guid.Parse("b90be65a-983e-4e1b-a46d-cdb3716c3c47");
            var seedTarte = await database.Articles.SingleAsync(item => item.Id == tarteId);
            seedTarte.Designation = "Tarte de Amêndoa";

            var customerArticleId = Guid.Parse("FEEDFEED-1111-2222-3333-444444444444");
            database.Articles.Add(CloneArticle(seedTarte, customerArticleId, "9991", "Tarte de Amêndoa"));
            await database.SaveChangesAsync();

            await ApplyR3SqliteAsync(database);

            await database.Entry(seedSubfamily).ReloadAsync();
            await database.Entry(seedTarte).ReloadAsync();
            var customerSubfamily = await database.ArticleSubfamilies.SingleAsync(item => item.Id == customerId);
            var customerArticle = await database.Articles.SingleAsync(item => item.Id == customerArticleId);

            Assert.Equal(FixSeedTexts2035R3Statements.NewSdrSubfamilyDesignation, seedSubfamily.Designation);
            Assert.Equal("Tarte de amêndoa", seedTarte.Designation);
            Assert.Equal(FixSeedTexts2035R3Statements.OldSdrSubfamilyDesignation, customerSubfamily.Designation);
            Assert.Equal("Tarte de Amêndoa", customerArticle.Designation);
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public void R3_provider_statements_are_generated()
    {
        var sqlite = FixSeedTexts2035R3Statements.ForSqlite().ToList();
        var mysql = FixSeedTexts2035R3Statements.ForMySql().ToList();
        var sqlServer = FixSeedTexts2035R3Statements.ForSqlServer().ToList();
        var postgresql = FixSeedTexts2035R3Statements.ForPostgreSql().ToList();

        Assert.Equal(1 + FixSeedTexts2035R3Statements.ArticleDesignations.Length, sqlite.Count);
        Assert.Equal(sqlite.Count, mysql.Count);
        Assert.Equal(sqlite.Count, sqlServer.Count);
        Assert.Equal(sqlite.Count, postgresql.Count);

        Assert.Contains(sqlite, sql => sql.Contains("ArticleSubfamilies", StringComparison.Ordinal)
            && sql.Contains("Depósito Volta", StringComparison.Ordinal)
            && sql.Contains("SDRVDEP", StringComparison.Ordinal)
            && sql.Contains("SubfamilyId", StringComparison.Ordinal));
        Assert.Contains(mysql, sql => sql.Contains("COLLATE utf8mb4_bin", StringComparison.Ordinal)
            && sql.Contains("ArticleSubfamilies", StringComparison.Ordinal));
        Assert.Contains(sqlServer, sql => sql.Contains("COLLATE Latin1_General_100_BIN2", StringComparison.Ordinal)
            && sql.Contains("ArticleSubfamilies", StringComparison.Ordinal));
        Assert.Contains(postgresql, sql => sql.Contains("\"ArticleSubfamilies\"", StringComparison.Ordinal));
        Assert.Contains(postgresql, sql => sql.Contains("::uuid", StringComparison.Ordinal)
            && sql.Contains("\"Articles\"", StringComparison.Ordinal));
        Assert.Contains(sqlite, sql => sql.Contains("Tarte de amêndoa", StringComparison.Ordinal)
            && sql.Contains("B90BE65A-983E-4E1B-A46D-CDB3716C3C47", StringComparison.Ordinal));
    }

    private static async Task ApplyR3SqliteAsync(LogicPOSDbContext database)
    {
        foreach (var sql in FixSeedTexts2035R3Statements.ForSqlite())
        {
            var escaped = sql.Replace("{", "{{", StringComparison.Ordinal).Replace("}", "}}", StringComparison.Ordinal);
            await database.Database.ExecuteSqlRawAsync(escaped);
        }
    }

    private static Article CloneArticle(Article source, Guid id, string code, string designation) =>
        new()
        {
            Id = id,
            Code = code,
            Order = source.Order,
            Designation = designation,
            ClassId = source.ClassId,
            SubfamilyId = source.SubfamilyId,
            TypeId = source.TypeId,
            MeasurementUnitId = source.MeasurementUnitId,
            SizeUnitId = source.SizeUnitId,
            VatDirectSellingId = source.VatDirectSellingId,
            VatExemptionReasonId = source.VatExemptionReasonId,
            Button = new Button(),
            Price1 = source.Price1,
            Price2 = source.Price2,
            Price3 = source.Price3,
            Price4 = source.Price4,
            Price5 = source.Price5,
            PriceWithVat = source.PriceWithVat,
            Discount = source.Discount,
            DefaultQuantity = source.DefaultQuantity,
            IsComposed = source.IsComposed,
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
