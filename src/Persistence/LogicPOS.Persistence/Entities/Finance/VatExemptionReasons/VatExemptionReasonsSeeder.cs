using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.Finance.VatExemptionReasons;

public static class VatExemptionReasonsSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, string countryCode2, ILogger logger)
    {
        logger.LogInformation("Seeding VAT exemption reasons");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!, countryCode2, "vatexemptionreasons.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("VAT exemption reasons seed file not found.");
            return false;
        }

        try
        {
            List<VatExemptionReason>? vatExemptionReasons = JsonSerializer.Deserialize<List<VatExemptionReason>>(File.ReadAllText(seedFilePath));
            if (vatExemptionReasons is null)
            {
                logger.LogError("VAT exemption reasons seed file is empty.");
                return false;
            }
            database.VatExemptionReasons.AddRange(vatExemptionReasons);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding VAT exemption reasons");
            return false;
        }

        logger.LogInformation("VAT exemption reasons seeded successfully");
        return true;
    }
}
