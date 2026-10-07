using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.Finance.VatRates;

public static class VatRatesSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, string countryCode2, ILogger logger)
    {
        logger.LogInformation("Seeding VAT rates");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!, countryCode2, "vatrates.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("VAT rates seed file not found.");
            return false;
        }

        try
        {
            List<VatRate>? vatRates = JsonSerializer.Deserialize<List<VatRate>>(File.ReadAllText(seedFilePath));
            if (vatRates is null)
            {
                logger.LogError("VAT rates seed file is empty.");
                return false;
            }
            database.VatRates.AddRange(vatRates);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding VAT rates");
            return false;
        }

        logger.LogInformation("VAT rates seeded successfully");
        return true;
    }
}
