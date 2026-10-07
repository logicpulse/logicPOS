using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.Finance.PaymentMethods;

public static class PaymentMethodsSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, ILogger logger)
    {
        logger.LogInformation("Seeding payment methods");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!, "paymentmethods.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Payment methods seed file not found.");
            return false;
        }

        try
        {
            List<PaymentMethod>? paymentMethods = JsonSerializer.Deserialize<List<PaymentMethod>>(File.ReadAllText(seedFilePath));
            if (paymentMethods is null)
            {
                logger.LogError("Payment methods seed file is empty.");
                return false;
            }
            database.PaymentMethods.AddRange(paymentMethods);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding payment methods");
            return false;
        }

        logger.LogInformation("Payment methods seeded successfully");
        return true;
    }
}
