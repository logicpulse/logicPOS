using LogicPOS.Core.BackOffice;
using LogicPOS.Persistence.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.Tests.BackOffice;

[Collection(AppCompositionCollection.Name)]
public sealed class ListingSaveErrorCodeTests
{
    private const string InvalidCheckDigitNif = "123456780";

    [Fact]
    public async Task Company_save_sets_invalid_fiscal_number_error_code()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-u12-company-code-");
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
            Assert.Equal(ListingSaveErrorCodes.InvalidFiscalNumberCode, saved.ErrorCode);
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Customer_save_sets_invalid_fiscal_number_error_code()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-u12-customer-code-");
        try
        {
            var (listing, ptId, typeId, priceId) = await ConfigureListingAsync(root);
            var saved = await listing.SaveAsync(
                "Clientes",
                null,
                CustomerFields("Cliente código NIF", InvalidCheckDigitNif, ptId, typeId, priceId));

            Assert.False(saved.Succeeded);
            Assert.Equal(ListingSaveErrorCodes.InvalidFiscalNumberCode, saved.ErrorCode);
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public void Resolve_listing_save_error_maps_invalid_fiscal_number_code_to_portuguese_message()
    {
        var result = ListingSaveResult.Fail(
            "Invalid Portuguese fiscal number (NIF).",
            ListingSaveErrorCodes.InvalidFiscalNumberCode);

        Assert.Equal(GtkFormRules.InvalidFiscalNumberUserMessage, GtkFormRules.ResolveListingSaveError(result));
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

    private static async Task<(IBackOfficeListingService Listing, Guid PtId, Guid TypeId, Guid PriceId)> ConfigureListingAsync(
        DirectoryInfo root)
    {
        Configure(root, useSeed: true);
        await using var scope = AppComposition.Services!.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var ptId = await database.Countries.AsNoTracking()
            .Where(item => item.IsDeleted == false && item.Code2 == "PT")
            .Select(item => item.Id)
            .FirstAsync();
        var typeId = await database.CustomerTypes.AsNoTracking()
            .Where(item => item.IsDeleted == false)
            .Select(item => item.Id)
            .FirstAsync();
        var priceId = await database.PriceTypes.AsNoTracking()
            .Where(item => item.IsDeleted == false)
            .Select(item => item.Id)
            .FirstAsync();
        var listing = AppComposition.Services!.GetRequiredService<IBackOfficeListingService>();
        return (listing, ptId, typeId, priceId);
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