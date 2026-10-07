using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.Finance.PaymentConditions;

public static class PaymentConditionsSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, ILogger logger)
    {
        logger.LogInformation("Seeding payment conditions");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!, "paymentconditions.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Payment conditions seed file not found.");
            return false;
        }

        try
        {
            List<PaymentCondition>? paymentConditions = JsonSerializer.Deserialize<List<PaymentCondition>>(File.ReadAllText(seedFilePath));
            if (paymentConditions is null)
            {
                logger.LogError("Payment conditions seed file is empty.");
                return false;
            }
            database.PaymentConditions.AddRange(paymentConditions);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding payment conditions");
            return false;
        }

        logger.LogInformation("Payment conditions seeded successfully");
        return true;
    }
}
