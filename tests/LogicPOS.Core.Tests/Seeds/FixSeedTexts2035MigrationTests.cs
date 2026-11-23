using LogicPOS.Core;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.DataFixes;
using LogicPOS.Persistence.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.Tests.Seeds;

[Collection(LogicPOS.Core.Tests.AppCompositionCollection.Name)]
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

            database.PreferenceParameters.Add(ClonePreference(existingTransport, Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"), order: 116, code: "116"));

            var smtpUser = await database.PreferenceParameters.SingleAsync(item => item.Token == "SEND_MAIL_SMTP_USERNAME");
            smtpUser.Value = "YOUR-EMAIL@gmail.com";

            var smtpPassword = await database.PreferenceParameters.SingleAsync(item => item.Token == "SEND_MAIL_SMTP_PASSWORD");
            smtpPassword.Value = "YOUR-PASSWORD";

            var virtualMoney = await database.PaymentMethods.SingleAsync(item => item.Token == "VIRTUAL_MONEY");
            virtualMoney.Designation = "Dinheiro Eletrônico";

            var stockGain = await database.PermissionItems.SingleAsync(item => item.Token == "REPORT_LIST_STOCK_GAIN");
            stockGain.Designation = "__designation[4040]__";

            var mousseId = Guid.Parse("2592aba9-f1b9-4c84-95a3-99b21b30bae0");
            var mousse = await database.Articles.SingleAsync(item => item.Id == mousseId);
            mousse.Designation = "Mousse de Chocolate C Chantily";

            var toucinhoId = Guid.Parse("0ebdf4ad-f628-45b4-b4ff-7b2ea55c2bb5");
            var toucinho = await database.Articles.SingleAsync(item => item.Id == toucinhoId);
            toucinho.Designation = "Toucinho Do Ceu";

            var outrasId = Guid.Parse("1c427e91-55f8-4b10-bc5e-96e3016c58e0");
            var outras = await database.ArticleSubfamilies.SingleAsync(item => item.Id == outrasId);
            outras.Designation = "Outras Caf";

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

            Assert.Equal(1, await database.PreferenceParameters.CountAsync(item => item.Token == transportToken));

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
    public async Task Customer_article_with_same_old_name_but_different_id_is_not_changed()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-fix-seed-customer-");
        try
        {
            ConfigureApp(root, module: "cafe");
            var services = AppComposition.Services!;
            await using var scope = services.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();

            var seedCaldo = await database.Articles.SingleAsync(item => item.Designation == "Caldo verde");
            seedCaldo.Designation = "Caldo Verde";
            await database.SaveChangesAsync();

            // Customer-owned row: same old designation text, different Id — must not be rewritten.
            const string customerId = "FEEDFEED-FEED-FEED-FEED-FEEDFEEDFEED";
            await database.Database.ExecuteSqlRawAsync(
                """
                INSERT INTO Articles (
                    Id, Barcode, Button_Image, Button_ImageExtension, Button_Label, ClassId, Code, CodeDealer,
                    CommissionGroupId, CreatedAt, CreatedBy, CreatedWhere, DefaultQuantity, DeletedAt, Designation,
                    Discount, DiscountGroupId, Favorite, IsComposed, IsDeleted, MeasurementUnitId, MinimumStock, Notes,
                    "Order", PVPVariable, Price1_PromotionValue, Price1_UsePromotion, Price1_Value,
                    Price2_PromotionValue, Price2_UsePromotion, Price2_Value, Price3_PromotionValue, Price3_UsePromotion, Price3_Value,
                    Price4_PromotionValue, Price4_UsePromotion, Price4_Value, Price5_PromotionValue, Price5_UsePromotion, Price5_Value,
                    PriceWithVat, SizeUnitId, SubfamilyId, Tare, TypeId, UniqueArticles, UpdatedAt, UpdatedBy, UpdatedWhere,
                    UseWeighingBalance, VatDirectSellingId, VatExemptionReasonId, VatOnTableId, Weight,
                    BarcodeLabelPrintModel, IsSdrPackaging, IsTicketing
                )
                SELECT
                    {0}, Barcode, Button_Image, Button_ImageExtension, Button_Label, ClassId, 'CUST-CALDO', CodeDealer,
                    CommissionGroupId, CreatedAt, CreatedBy, CreatedWhere, DefaultQuantity, DeletedAt, 'Caldo Verde',
                    Discount, DiscountGroupId, Favorite, IsComposed, 0, MeasurementUnitId, MinimumStock, Notes,
                    99999, PVPVariable, Price1_PromotionValue, Price1_UsePromotion, Price1_Value,
                    Price2_PromotionValue, Price2_UsePromotion, Price2_Value, Price3_PromotionValue, Price3_UsePromotion, Price3_Value,
                    Price4_PromotionValue, Price4_UsePromotion, Price4_Value, Price5_PromotionValue, Price5_UsePromotion, Price5_Value,
                    PriceWithVat, SizeUnitId, SubfamilyId, Tare, TypeId, UniqueArticles, UpdatedAt, UpdatedBy, UpdatedWhere,
                    UseWeighingBalance, VatDirectSellingId, VatExemptionReasonId, VatOnTableId, Weight,
                    BarcodeLabelPrintModel, IsSdrPackaging, IsTicketing
                FROM Articles
                WHERE Id = {1}
                LIMIT 1
                """,
                customerId,
                seedCaldo.Id.ToString().ToUpperInvariant());

            foreach (var sql in FixSeedTexts2035Statements.ForSqlite())
            {
                await database.Database.ExecuteSqlRawAsync(sql);
            }

            await database.Entry(seedCaldo).ReloadAsync();
            var customerDesignation = await database.Articles
                .Where(item => item.Id == Guid.Parse(customerId))
                .Select(item => item.Designation)
                .SingleAsync();
            Assert.Equal("Caldo verde", seedCaldo.Designation);
            Assert.Equal("Caldo Verde", customerDesignation);
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
            var beforeRedBull = await database.Articles.CountAsync(item => item.Designation == "Red Bull");

            Assert.Equal(1, beforeTransport);
            Assert.Equal("Dinheiro Eletrónico", beforeVirtualMoney);
            Assert.Equal("Relatório :: Stock por valorização de artigo", beforeStockGain);
            Assert.True(beforeToucinho >= 1);
            Assert.True(beforeOutras >= 1);
            Assert.Equal(string.Empty, beforeSmtpUser);
            Assert.True(beforeRedBull >= 1);

            var appliedMigrations = await database.Database.GetAppliedMigrationsAsync();
            Assert.Contains(appliedMigrations, id => id == "20261123120000_FixSeedTexts2035");

            foreach (var sql in FixSeedTexts2035Statements.ForSqlite())
            {
                await database.Database.ExecuteSqlRawAsync(sql);
            }

            Assert.Equal(1, await database.PreferenceParameters.CountAsync(item => item.Token == "COMPANY_COMMUNICATE_TRANSPORT_DOCUMENTS"));
            Assert.Equal(beforeRedBull, await database.Articles.CountAsync(item => item.Designation == "Red Bull"));
            Assert.Equal(beforeToucinho, await database.Articles.CountAsync(item => item.Designation == "Toucinho do Céu"));
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

        Assert.Equal(257, sqlite.Count);
        Assert.Equal(sqlite.Count, mysql.Count);
        Assert.Equal(sqlite.Count, sqlServer.Count);
        Assert.Equal(sqlite.Count, postgresql.Count);
        Assert.Contains(mysql, sql => sql.Contains("COLLATE utf8mb4_bin", StringComparison.Ordinal));
        Assert.Contains(sqlServer, sql => sql.Contains("COLLATE Latin1_General_100_BIN2", StringComparison.Ordinal));
        Assert.Contains(sqlite, sql => sql.Contains("0D30BF31-ECC4-452E-9B43-EE9D5C1D7FB6", StringComparison.Ordinal));
        Assert.Contains(postgresql, sql => sql.Contains("::uuid", StringComparison.Ordinal) && sql.Contains("\"Articles\"", StringComparison.Ordinal));
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
        // Do not dispose AppComposition.Services: Configure replaces the static provider;
        // disposing it races with other AppComposition tests and leaves a dead global.
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
