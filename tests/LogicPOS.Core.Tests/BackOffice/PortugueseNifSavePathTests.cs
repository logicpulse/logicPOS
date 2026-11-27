using LogicPOS.Core.BackOffice;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.ValueObjects;
using LogicPOS.Persistence.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.Tests.BackOffice;

[Collection(AppCompositionCollection.Name)]
public sealed class PortugueseNifSavePathTests
{
    // Public/fake test numbers only (mathematically valid or deliberately invalid).
    private const string ValidCompanyNif = "500000000";
    private const string ValidIndividualNif = "123456789";
    private const string InvalidCheckDigitNif = "123456780";
    private const string SpanishFiscal = "B12345678";

    [Fact]
    public async Task Customer_create_rejects_invalid_portuguese_nif()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-u12-cust-create-");
        try
        {
            var (listing, ptId, _, typeId, priceId) = await ConfigureAsync(root);
            var saved = await listing.SaveAsync(
                "Clientes",
                null,
                CustomerFields("Cliente NIF inválido", InvalidCheckDigitNif, ptId, typeId, priceId));

            Assert.False(saved.Succeeded);
            Assert.Equal("Invalid Portuguese fiscal number (NIF).", saved.Error);
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Customer_edit_rejects_invalid_portuguese_nif()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-u12-cust-edit-");
        try
        {
            var (listing, ptId, _, typeId, priceId) = await ConfigureAsync(root);
            var created = await listing.SaveAsync(
                "Clientes",
                null,
                CustomerFields("Cliente NIF edit", ValidIndividualNif, ptId, typeId, priceId));
            Assert.True(created.Succeeded, created.Error);

            var edited = await listing.SaveAsync(
                "Clientes",
                created.Id,
                CustomerFields("Cliente NIF edit", InvalidCheckDigitNif, ptId, typeId, priceId));

            Assert.False(edited.Succeeded);
            Assert.Equal("Invalid Portuguese fiscal number (NIF).", edited.Error);
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Customer_and_company_accept_final_consumer_nif_on_save()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-u12-cf-");
        try
        {
            var (listing, ptId, _, typeId, priceId) = await ConfigureAsync(root);
            // Avoid clashing with the seeded Consumidor Final row: create with a valid
            // individual NIF, then confirm company save accepts 999999990.
            var created = await listing.SaveAsync(
                "Clientes",
                null,
                CustomerFields("Cliente U12 válido", ValidIndividualNif, ptId, typeId, priceId));
            Assert.True(created.Succeeded, created.Error);

            var company = AppComposition.Services!.GetRequiredService<ICompanySetupService>();
            var draft = await company.LoadAsync();
            var portugal = Assert.Single(draft.Countries, item =>
                string.Equals(item.Code, "PT", StringComparison.OrdinalIgnoreCase));
            var euro = Assert.Single(draft.Currencies, item =>
                string.Equals(item.Code, "EUR", StringComparison.OrdinalIgnoreCase));
            var values = new Dictionary<string, string>(LocalCompanySetupService.DemoCompanyValues, StringComparer.OrdinalIgnoreCase)
            {
                ["COMPANY_FISCALNUMBER"] = PortugueseNif.FinalConsumer
            };
            var saved = await company.SaveAsync(new CompanySetupRequest
            {
                CountryId = portugal.Id,
                CurrencyId = euro.Id,
                Values = values
            });
            Assert.True(saved.Succeeded, saved.Error);
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Foreign_customer_skips_portuguese_nif_rules()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-u12-cust-es-");
        try
        {
            var (listing, _, esId, typeId, priceId) = await ConfigureAsync(root);
            Assert.NotEqual(Guid.Empty, esId);

            var saved = await listing.SaveAsync(
                "Clientes",
                null,
                CustomerFields("Cliente ES", SpanishFiscal, esId, typeId, priceId));

            Assert.True(saved.Succeeded, saved.Error);
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Existing_customer_with_invalid_nif_still_loads_and_lists()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-u12-cust-load-");
        try
        {
            var (listing, ptId, _, typeId, priceId) = await ConfigureAsync(root);
            Guid id;
            await using (var scope = AppComposition.Services!.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
                var now = DateTime.Now;
                var customer = new Customer
                {
                    Id = Guid.NewGuid(),
                    Order = 9001,
                    Code = "U12INV",
                    Name = "Cliente NIF legado inválido",
                    FiscalNumber = InvalidCheckDigitNif,
                    CountryId = ptId,
                    CustomerTypeId = typeId,
                    PriceTypeId = priceId,
                    Discount = 0,
                    CreatedAt = now,
                    UpdatedAt = now
                };
                database.Customers.Add(customer);
                await database.SaveChangesAsync();
                id = customer.Id;
            }

            var page = await listing.QueryAsync("Clientes");
            Assert.Contains(page.Rows, row => row.Id == id);

            await using (var scope = AppComposition.Services!.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
                var loaded = await database.Customers.AsNoTracking().SingleAsync(item => item.Id == id);
                Assert.Equal(InvalidCheckDigitNif, loaded.FiscalNumber);
            }
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Company_save_rejects_invalid_portuguese_nif()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-u12-company-");
        try
        {
            Configure(root, useSeed: false);
            var company = AppComposition.Services!.GetRequiredService<ICompanySetupService>();
            var draft = await company.LoadAsync();
            var portugal = Assert.Single(draft.Countries, item =>
                string.Equals(item.Code, "PT", StringComparison.OrdinalIgnoreCase));
            var euro = Assert.Single(draft.Currencies, item =>
                string.Equals(item.Code, "EUR", StringComparison.OrdinalIgnoreCase));

            var values = new Dictionary<string, string>(LocalCompanySetupService.DemoCompanyValues, StringComparer.OrdinalIgnoreCase)
            {
                ["COMPANY_FISCALNUMBER"] = InvalidCheckDigitNif
            };

            var saved = await company.SaveAsync(new CompanySetupRequest
            {
                CountryId = portugal.Id,
                CurrencyId = euro.Id,
                Values = values
            });

            Assert.False(saved.Succeeded);
            Assert.Equal("Invalid Portuguese fiscal number (NIF).", saved.Error);
        }
        finally
        {
            Cleanup(root);
        }
    }

    private static Dictionary<string, string> CustomerFields(
        string name,
        string fiscal,
        Guid countryId,
        Guid typeId,
        Guid priceId) => new()
    {
        ["Name"] = name,
        ["FiscalNumber"] = fiscal,
        ["CountryId"] = countryId.ToString(),
        ["CustomerTypeId"] = typeId.ToString(),
        ["PriceTypeId"] = priceId.ToString(),
        ["Discount"] = "0"
    };

    private static async Task<(IBackOfficeListingService Listing, Guid PtId, Guid EsId, Guid TypeId, Guid PriceId)> ConfigureAsync(
        DirectoryInfo root,
        bool useSeed = true)
    {
        Configure(root, useSeed);
        await using var scope = AppComposition.Services!.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var ptId = await database.Countries.AsNoTracking()
            .Where(item => item.IsDeleted == false && item.Code2 == "PT")
            .Select(item => item.Id)
            .FirstAsync();
        var esId = await database.Countries.AsNoTracking()
            .Where(item => item.IsDeleted == false && item.Code2 == "ES")
            .Select(item => item.Id)
            .FirstOrDefaultAsync();
        var typeId = await database.CustomerTypes.AsNoTracking()
            .Where(item => item.IsDeleted == false)
            .Select(item => item.Id)
            .FirstAsync();
        var priceId = await database.PriceTypes.AsNoTracking()
            .Where(item => item.IsDeleted == false)
            .Select(item => item.Id)
            .FirstAsync();
        var listing = AppComposition.Services!.GetRequiredService<IBackOfficeListingService>();
        return (listing, ptId, esId, typeId, priceId);
    }

    private static void Configure(DirectoryInfo root, bool useSeed)
    {
        var databasePath = Path.Combine(root.FullName, "logicpos.db");
        var useSeedJson = useSeed ? "true" : "false";
        File.WriteAllText(
            Path.Combine(root.FullName, "appsettings.json"),
            $$"""
            {
              "Culture": "pt-PT",
              "DatabaseSettings": {
                "DatabaseType": "Sqlite",
                "ConnectionString": "Data Source={{databasePath.Replace("\\", "\\\\")}}",
                "UseCloud": false,
                "UseSeed": {{useSeedJson}},
                "Module": "cafe"
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
