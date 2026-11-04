using LogicPOS.Core.BackOffice;
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
}
