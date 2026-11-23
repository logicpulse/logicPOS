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
    public async Task Old_database_updates_lpd_and_trv_designations()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-fix-seed-r2-old-lpd-");
        try
        {
            ConfigureApp(root, module: "cafe");
            await using var scope = AppComposition.Services!.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();

            var lpd = await database.PaymentConditions.SingleAsync(item =>
                item.Id == FixSeedTexts2035R2Statements.PaymentConditionLpdId);
            var trv = await database.DocumentTypes.SingleAsync(item =>
                item.Id == FixSeedTexts2035R2Statements.DocumentTypeTrvId);
            lpd.Designation = FixSeedTexts2035R2Statements.OldPaymentConditionLpdDesignation;
            trv.Designation = FixSeedTexts2035R2Statements.OldDocumentTypeTrvDesignation;
            await database.SaveChangesAsync();

            await ApplyR2SqliteAsync(database);

            await database.Entry(lpd).ReloadAsync();
            await database.Entry(trv).ReloadAsync();
            Assert.Equal(FixSeedTexts2035R2Statements.NewPaymentConditionLpdDesignation, lpd.Designation);
            Assert.Equal(FixSeedTexts2035R2Statements.NewDocumentTypeTrvDesignation, trv.Designation);
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task New_database_lpd_and_trv_statements_are_noop()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-fix-seed-r2-new-lpd-");
        try
        {
            ConfigureApp(root, module: "cafe");
            await using var scope = AppComposition.Services!.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();

            var lpd = await database.PaymentConditions.SingleAsync(item =>
                item.Id == FixSeedTexts2035R2Statements.PaymentConditionLpdId);
            var trv = await database.DocumentTypes.SingleAsync(item =>
                item.Id == FixSeedTexts2035R2Statements.DocumentTypeTrvId);
            Assert.Equal(FixSeedTexts2035R2Statements.NewPaymentConditionLpdDesignation, lpd.Designation);
            Assert.Equal(FixSeedTexts2035R2Statements.NewDocumentTypeTrvDesignation, trv.Designation);

            await ApplyR2SqliteAsync(database);

            await database.Entry(lpd).ReloadAsync();
            await database.Entry(trv).ReloadAsync();
            Assert.Equal(FixSeedTexts2035R2Statements.NewPaymentConditionLpdDesignation, lpd.Designation);
            Assert.Equal(FixSeedTexts2035R2Statements.NewDocumentTypeTrvDesignation, trv.Designation);
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Customer_row_with_same_old_text_but_different_id_is_not_changed()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-fix-seed-r2-customer-lpd-");
        try
        {
            ConfigureApp(root, module: "cafe");
            await using var scope = AppComposition.Services!.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();

            var seedLpd = await database.PaymentConditions.SingleAsync(item =>
                item.Id == FixSeedTexts2035R2Statements.PaymentConditionLpdId);
            seedLpd.Designation = FixSeedTexts2035R2Statements.OldPaymentConditionLpdDesignation;

            var seedTrv = await database.DocumentTypes.SingleAsync(item =>
                item.Id == FixSeedTexts2035R2Statements.DocumentTypeTrvId);
            seedTrv.Designation = FixSeedTexts2035R2Statements.OldDocumentTypeTrvDesignation;

            var customerPaymentId = Guid.Parse("FEEDFEED-1111-2222-3333-444444444444");
            var customerDocumentTypeId = Guid.Parse("FEEDFEED-5555-6666-7777-888888888888");
            database.PaymentConditions.Add(new PaymentCondition
            {
                Id = customerPaymentId,
                Acronym = "CLP",
                Code = "9991",
                Order = 9991,
                Designation = FixSeedTexts2035R2Statements.OldPaymentConditionLpdDesignation,
                CreatedAt = DateTime.Now,
                CreatedBy = Guid.Empty,
                CreatedWhere = Guid.Empty,
                UpdatedAt = DateTime.Now,
                UpdatedBy = Guid.Empty,
                UpdatedWhere = Guid.Empty,
                IsDeleted = false,
                DeletedAt = default
            });
            database.DocumentTypes.Add(CloneDocumentType(seedTrv, customerDocumentTypeId, "9992",
                FixSeedTexts2035R2Statements.OldDocumentTypeTrvDesignation));
            await database.SaveChangesAsync();

            await ApplyR2SqliteAsync(database);

            await database.Entry(seedLpd).ReloadAsync();
            await database.Entry(seedTrv).ReloadAsync();
            var customerPayment = await database.PaymentConditions.SingleAsync(item => item.Id == customerPaymentId);
            var customerDocType = await database.DocumentTypes.SingleAsync(item => item.Id == customerDocumentTypeId);

            Assert.Equal(FixSeedTexts2035R2Statements.NewPaymentConditionLpdDesignation, seedLpd.Designation);
            Assert.Equal(FixSeedTexts2035R2Statements.NewDocumentTypeTrvDesignation, seedTrv.Designation);
            Assert.Equal(FixSeedTexts2035R2Statements.OldPaymentConditionLpdDesignation, customerPayment.Designation);
            Assert.Equal(FixSeedTexts2035R2Statements.OldDocumentTypeTrvDesignation, customerDocType.Designation);
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

        Assert.Equal(8, sqlite.Count);
        Assert.Equal(sqlite.Count, mysql.Count);
        Assert.Equal(sqlite.Count, sqlServer.Count);
        Assert.Equal(sqlite.Count, postgresql.Count);
        Assert.Contains(sqlite, sql => sql.Contains("Depósito", StringComparison.Ordinal));
        Assert.Contains(sqlite, sql => sql.Contains("exceto", StringComparison.Ordinal));
        Assert.Contains(sqlite, sql => sql.Contains("SELECT k.", StringComparison.Ordinal));
        Assert.Contains(sqlite, sql => sql.Contains("Letra para desconto", StringComparison.Ordinal)
            && sql.Contains("C07A5C41-37B2-4BF3-9469-5FAF1799A89F", StringComparison.Ordinal));
        Assert.Contains(mysql, sql => sql.Contains("COLLATE utf8mb4_bin", StringComparison.Ordinal)
            && sql.Contains("PaymentConditions", StringComparison.Ordinal));
        Assert.Contains(sqlServer, sql => sql.Contains("COLLATE Latin1_General_100_BIN2", StringComparison.Ordinal)
            && sql.Contains("DocumentTypes", StringComparison.Ordinal));
        Assert.Contains(postgresql, sql => sql.Contains("::uuid", StringComparison.Ordinal)
            && sql.Contains("\"DocumentTypes\"", StringComparison.Ordinal));
    }

    private static async Task ApplyR2SqliteAsync(LogicPOSDbContext database)
    {
        foreach (var sql in FixSeedTexts2035R2Statements.ForSqlite())
        {
            var escaped = sql.Replace("{", "{{", StringComparison.Ordinal).Replace("}", "}}", StringComparison.Ordinal);
            await database.Database.ExecuteSqlRawAsync(escaped);
        }
    }

    private static DocumentType CloneDocumentType(DocumentType source, Guid id, string code, string designation) =>
        new()
        {
            Id = id,
            Code = code,
            Order = source.Order,
            Designation = designation,
            Acronym = source.Acronym + "X",
            AcronymLastSerie = source.AcronymLastSerie,
            Credit = source.Credit,
            CreditDebit = source.CreditDebit,
            PrintCopies = source.PrintCopies,
            PrintRequestMotive = source.PrintRequestMotive,
            PrintRequestConfirmation = source.PrintRequestConfirmation,
            PrintOpenDrawer = source.PrintOpenDrawer,
            WayBill = source.WayBill,
            WsAtDocument = source.WsAtDocument,
            SaftAuditFile = source.SaftAuditFile,
            SaftDocumentType = source.SaftDocumentType,
            StockMode = source.StockMode,
            CreatedAt = source.CreatedAt.AddMinutes(1),
            CreatedBy = source.CreatedBy,
            CreatedWhere = source.CreatedWhere,
            UpdatedAt = source.UpdatedAt.AddMinutes(1),
            UpdatedBy = source.UpdatedBy,
            UpdatedWhere = source.UpdatedWhere,
            IsDeleted = false,
            DeletedAt = source.DeletedAt
        };

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
