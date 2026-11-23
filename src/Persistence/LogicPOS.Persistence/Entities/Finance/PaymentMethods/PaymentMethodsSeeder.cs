using System.Text.Json;
using LogicPOS.Application.Features.Finance.PaymentMethods;
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

    /// <summary>
    /// Existing databases: the cash-machine row labeled «MB Way» becomes Multibanco.
    /// Portugal databases also gain a separate MB Way method. Other rows are left as they are.
    /// </summary>
    public static void EnsureMultibancoAndMbWay(LogicPOSDbContext database, bool portugal, ILogger logger)
    {
        var cashMachine = database.PaymentMethods
            .FirstOrDefault(method => method.IsDeleted == false && method.Token == PosPaymentMethods.MultibancoToken);
        var renamed = cashMachine is not null
            ? PosPaymentMethods.RenameCashMachine(cashMachine.Designation)
            : null;
        if (cashMachine is not null && renamed is not null)
        {
            cashMachine.Designation = renamed;
            logger.LogInformation("Payment method {Token} designation is now {Designation}.", cashMachine.Token, renamed);
        }

        if (portugal
            && database.PaymentMethods.Any(method => method.IsDeleted == false && method.Token == PosPaymentMethods.MbWayToken) == false)
        {
            database.PaymentMethods.Add(new PaymentMethod
            {
                Id = Guid.Parse("6c0e2a41-7b14-4d4e-9a1f-3e8b5d0c7a22"),
                Token = PosPaymentMethods.MbWayToken,
                Acronym = "MW",
                Designation = PosPaymentMethods.MbWayDesignation,
                Code = "81",
                Order = 81,
                ButtonIcon = cashMachine?.ButtonIcon,
                ResourceString = cashMachine?.ResourceString
            });
            logger.LogInformation("Added Portugal payment method {Token}.", PosPaymentMethods.MbWayToken);
        }

        database.SaveChanges();
    }
}
