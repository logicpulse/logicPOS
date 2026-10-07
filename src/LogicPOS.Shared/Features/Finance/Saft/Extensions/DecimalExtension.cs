using System.Globalization;

namespace LogicPOS.Shared.Features.Finance.Saft.Extensions;

public static class DecimalExtension
{
    public static string To2DecimalPlacesString(this decimal value)
    {
        return value.ToString("0.00",CultureInfo.InvariantCulture);
    }
    
    public static string ToInvariantCultureString(this decimal value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }
}