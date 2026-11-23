using LogicPOS.Core.BackOffice;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.Tests;

[Collection(AppCompositionCollection.Name)]
public sealed class FinalConsumerSetupWizardTests
{
    [Fact]
    public async Task Create_once_inserts_single_final_consumer()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-cf-once-");
        try
        {
            var wizard = await ConfigureWithoutSeedAsync(root);
            var result = await wizard.CreateAsync(new FinalConsumerSetupRequest());
            Assert.True(result.Ok, result.Message);

            await using var scope = AppComposition.Services!.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
            Assert.Equal(1, await CountFinalConsumersAsync(database));
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Create_twice_is_idempotent()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-cf-twice-");
        try
        {
            var wizard = await ConfigureWithoutSeedAsync(root);
            var first = await wizard.CreateAsync(new FinalConsumerSetupRequest());
            var second = await wizard.CreateAsync(new FinalConsumerSetupRequest());
            Assert.True(first.Ok, first.Message);
            Assert.True(second.Ok, second.Message);
            Assert.Contains("já existe", second.Message, StringComparison.OrdinalIgnoreCase);

            await using var scope = AppComposition.Services!.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
            Assert.Equal(1, await CountFinalConsumersAsync(database));
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Create_concurrent_calls_leave_single_final_consumer()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-cf-concurrent-");
        try
        {
            var wizard = await ConfigureWithoutSeedAsync(root);
            var results = await Task.WhenAll(
                wizard.CreateAsync(new FinalConsumerSetupRequest()),
                wizard.CreateAsync(new FinalConsumerSetupRequest()),
                wizard.CreateAsync(new FinalConsumerSetupRequest()));

            Assert.All(results, item => Assert.True(item.Ok, item.Message));

            await using var scope = AppComposition.Services!.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
            Assert.Equal(1, await CountFinalConsumersAsync(database));
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Create_when_seed_row_already_exists_does_not_insert_another()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-cf-seed-");
        try
        {
            ConfigureWithSeed(root);
            Assert.Null(AppComposition.StartupError);

            await using (var scope = AppComposition.Services!.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
                Assert.Equal(1, await CountFinalConsumersAsync(database));
                Assert.True(await database.Customers.AnyAsync(item =>
                    item.IsDeleted == false && item.Id == CustomerWellKnownIds.FinalConsumer));
            }

            var wizard = AppComposition.Services!.GetRequiredService<IFinalConsumerSetupWizard>();
            var result = await wizard.CreateAsync(new FinalConsumerSetupRequest());
            Assert.True(result.Ok, result.Message);

            await using var after = AppComposition.Services!.CreateAsyncScope();
            var db = after.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
            Assert.Equal(1, await CountFinalConsumersAsync(db));
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Listing_save_rejects_second_final_consumer_with_same_nif()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-cf-listing-");
        try
        {
            ConfigureWithSeed(root);
            Assert.Null(AppComposition.StartupError);
            var listing = AppComposition.Services!.GetRequiredService<IBackOfficeListingService>();
            var saved = await listing.SaveAsync(
                "Clientes",
                null,
                new Dictionary<string, string>
                {
                    ["Name"] = "Consumidor Final",
                    ["FiscalNumber"] = "999999990"
                });

            Assert.False(saved.Succeeded);
            Assert.Contains("já existe", saved.Error ?? string.Empty, StringComparison.OrdinalIgnoreCase);

            await using var scope = AppComposition.Services!.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
            Assert.Equal(1, await CountFinalConsumersAsync(database));
        }
        finally
        {
            Cleanup(root);
        }
    }

    private static async Task<IFinalConsumerSetupWizard> ConfigureWithoutSeedAsync(DirectoryInfo root)
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
        Assert.NotNull(AppComposition.Services);
        return AppComposition.Services.GetRequiredService<IFinalConsumerSetupWizard>();
    }

    private static void ConfigureWithSeed(DirectoryInfo root)
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
                "Module": "cafe"
              }
            }
            """);

        AppComposition.Configure(root.FullName);
        Assert.NotNull(AppComposition.Services);
    }

    private static Task<int> CountFinalConsumersAsync(LogicPOSDbContext database) =>
        database.Customers.CountAsync(item => item.IsDeleted == false && item.Name == "Consumidor Final");

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
                // Temp cleanup is best-effort under parallel file locks.
            }
        }
    }
}
