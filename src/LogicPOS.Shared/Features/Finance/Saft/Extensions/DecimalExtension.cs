using System.Globalization;

namespace LogicPOS.Shared.Features.Finance.Saft.Extensions;

public static class DecimalExtension
{
    public static string ToInvariantCultureString(this decimal value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }
}