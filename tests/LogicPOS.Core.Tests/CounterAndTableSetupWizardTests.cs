using LogicPOS.Core.BackOffice;
using LogicPOS.Persistence.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.Tests;

public sealed class CounterAndTableSetupWizardTests
{
    [Fact]
    public async Task Final_consumer_and_table_wizards_create_required_records()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-counter-table-");
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

            var consumer = await services.GetRequiredService<IFinalConsumerSetupWizard>()
                .CreateAsync(new FinalConsumerSetupRequest());
            Assert.True(consumer.Ok, consumer.Message);

            var table = await services.GetRequiredService<ITableSetupWizard>()
                .CreateAsync(new TableSetupRequest
                {
                    PlaceDesignation = "Sala",
                    TableDesignation = "Mesa 1"
                });
            Assert.True(table.Ok, table.Message);

            await using var scope = services.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
            Assert.True(await database.Customers.AnyAsync(item => item.IsDeleted == false && item.Name == "Consumidor Final"));
            Assert.True(await database.Places.AnyAsync(item => item.IsDeleted == false && item.Designation == "Sala"));
            Assert.True(await database.Tables.AnyAsync(item => item.IsDeleted == false && item.Designation == "Mesa 1"));
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
