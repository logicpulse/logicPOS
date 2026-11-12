using LogicPOS.Core.BackOffice;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.Tests;

public sealed class ArticleSetupWizardTests
{
    [Fact]
    public async Task Create_builds_family_subfamily_and_article_on_clean_database()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-article-wizard-");
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
                    "UseCloud": false,
                    "UseSeed": false,
                    "Module": "restaurant"
                  }
                }
                """);

            AppComposition.Configure(root.FullName);
            Assert.Null(AppComposition.StartupError);
            var services = AppComposition.Services!;
            var wizard = services.GetRequiredService<IArticleSetupWizard>();

            var result = await wizard.CreateAsync(new ArticleSetupRequest
            {
                FamilyDesignation = "Bebidas",
                SubfamilyDesignation = "Café",
                ArticleDesignation = "Expresso",
                Price = "1,20"
            });

            Assert.True(result.Ok, result.Message);

            await using var scope = services.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
            Assert.True(await database.ArticleFamilies.AnyAsync(item => item.IsDeleted == false && item.Designation == "Bebidas"));
            Assert.True(await database.ArticleSubfamilies.AnyAsync(item => item.IsDeleted == false && item.Designation == "Café"));
            var article = await database.Articles.SingleAsync(item => item.IsDeleted == false && item.Designation == "Expresso");
            Assert.Equal(1.20m, article.Price1.Value);
        }
        finally
        {
            if (AppComposition.Services is IDisposable disposable)
            {
                disposable.Dispose();
            }

            SqliteConnection.ClearAllPools();
            AppLogging.CloseAndFlush();
            root.Delete(recursive: true);
        }
    }

    [Fact]
    public async Task Bulk_edit_needs_exemption_reason_for_zero_vat_and_applies_only_ticked_fields()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-article-bulk-");
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
                    "UseCloud": false,
                    "UseSeed": false,
                    "Module": "restaurant"
                  }
                }
                """);

            AppComposition.Configure(root.FullName);
            Assert.Null(AppComposition.StartupError);
            var services = AppComposition.Services!;
            var created = await services.GetRequiredService<IArticleSetupWizard>().CreateAsync(new ArticleSetupRequest
            {
                FamilyDesignation = "Bebidas",
                SubfamilyDesignation = "Café",
                ArticleDesignation = "Expresso",
                Price = "1,20"
            });
            Assert.True(created.Ok, created.Message);

            Guid articleId, zeroVatId, reasonId, originalVatId;
            await using (var scope = services.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
                var article = await database.Articles.SingleAsync(item => item.Designation == "Expresso");
                articleId = article.Id;
                originalVatId = article.VatDirectSellingId;
                var zeroVat = new VatRate
                {
                    Code = "BULK0", Designation = "Isento", Value = 0, TaxType = "IVA", TaxCode = "ISE",
                    CountryRegion = "PT", Description = "Isento"
                };
                var reason = new VatExemptionReason
                {
                    Code = "BULKM07", Designation = "Isento artigo 9.º do CIVA", Acronym = "M07", StandardApplicable = "Artigo 9.º do CIVA"
                };
                database.AddRange(zeroVat, reason);
                await database.SaveChangesAsync();
                zeroVatId = zeroVat.Id;
                reasonId = reason.Id;
            }

            var bulk = services.GetRequiredService<IBulkEditService>();
            var rejected = await bulk.ApplyAsync("Artigos", [articleId], new Dictionary<string, string>
            {
                [BulkEditService.ArticleVatKey] = zeroVatId.ToString()
            });
            Assert.False(rejected.Succeeded);
            Assert.Contains("motivo de isenção", rejected.Error);

            var applied = await bulk.ApplyAsync("Artigos", [articleId], new Dictionary<string, string>
            {
                [BulkEditService.ArticleVatKey] = zeroVatId.ToString(),
                ["VatExemptionReasonId"] = reasonId.ToString()
            });
            Assert.True(applied.Succeeded, applied.Error);
            Assert.Equal("1 artigo(s) atualizado(s).", applied.Message);

            await using (var scope = services.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
                var article = await database.Articles.AsNoTracking().SingleAsync(item => item.Id == articleId);
                Assert.Equal(zeroVatId, article.VatOnTableId);
                Assert.Equal(zeroVatId, article.VatDirectSellingId);
                Assert.Equal(reasonId, article.VatExemptionReasonId);
                Assert.False(article.Favorite);
                Assert.Equal(1.20m, article.Price1.Value);
            }

            var favorite = await bulk.ApplyAsync("Artigos", [articleId], new Dictionary<string, string> { ["Favorite"] = "true" });
            Assert.True(favorite.Succeeded, favorite.Error);

            var cleared = await bulk.ApplyAsync("Artigos", [articleId], new Dictionary<string, string>
            {
                [BulkEditService.ArticleVatKey] = originalVatId.ToString()
            });
            Assert.True(cleared.Succeeded, cleared.Error);

            await using (var scope = services.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
                var article = await database.Articles.AsNoTracking().SingleAsync(item => item.Id == articleId);
                Assert.True(article.Favorite);
                Assert.Equal(originalVatId, article.VatDirectSellingId);
                Assert.Null(article.VatExemptionReasonId);
            }
        }
        finally
        {
            if (AppComposition.Services is IDisposable disposable)
            {
                disposable.Dispose();
            }

            SqliteConnection.ClearAllPools();
            AppLogging.CloseAndFlush();
            root.Delete(recursive: true);
        }
    }
}
