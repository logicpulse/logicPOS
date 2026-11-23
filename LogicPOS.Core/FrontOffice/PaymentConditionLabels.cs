namespace LogicPOS.Core.FrontOffice;

/// <summary>
/// Display overrides for payment-condition designations still abbreviated in seeds/DB.
/// Seed Designation stays unchanged until the model team updates db_seeds.
/// </summary>
public static class PaymentConditionLabels
{
    public static string Display(string? designation)
    {
        if (string.IsNullOrWhiteSpace(designation))
        {
            return string.Empty;
        }

        return designation.Trim() switch
        {
            "Letra p/Desconto" => "Letra para desconto",
            _ => designation
        };
    }
}
