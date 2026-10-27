namespace LogicPOS.Application.Features.Finance.Documents;

/// <summary>
/// Valor por extenso em português de Portugal.
/// O euro usa «cêntimo»; «centavo» fica para o metical.
/// </summary>
public static class PortugueseMoneyWords
{
    private static readonly string[] UnderTwenty =
    [
        "zero", "um", "dois", "três", "quatro", "cinco", "seis", "sete", "oito", "nove",
        "dez", "onze", "doze", "treze", "catorze", "quinze", "dezasseis", "dezassete", "dezoito", "dezanove"
    ];

    private static readonly string[] Tens =
    [
        "", "", "vinte", "trinta", "quarenta", "cinquenta", "sessenta", "setenta", "oitenta", "noventa"
    ];

    private static readonly string[] Hundreds =
    [
        "", "cento", "duzentos", "trezentos", "quatrocentos", "quinhentos", "seiscentos", "setecentos", "oitocentos", "novecentos"
    ];

    public static string Convert(decimal amount, string? currency)
    {
        var units = Units(currency);
        var negative = amount < 0;
        var rounded = decimal.Round(Math.Abs(amount), 2, MidpointRounding.AwayFromZero);
        var major = (long)decimal.Truncate(rounded);
        var minor = (int)((rounded - major) * 100m);

        var text = major switch
        {
            0 when minor == 0 => "zero " + units.Many,
            0 => Minor(minor, units),
            _ when minor == 0 => Major(major, units),
            _ => Major(major, units) + " e " + Minor(minor, units)
        };

        return negative ? "menos " + text : text;
    }

    private static string Major(long value, MoneyUnits units)
    {
        var words = IntegerToWords(value);
        var link = words.EndsWith("milhão") || words.EndsWith("milhões") ? " de " : " ";
        return words + link + (value == 1 ? units.One : units.Many);
    }

    private static string Minor(int value, MoneyUnits units)
        => IntegerToWords(value) + " " + (value == 1 ? units.Cent : units.Cents);

    private static string IntegerToWords(long value)
    {
        if (value < 20)
        {
            return UnderTwenty[value];
        }

        if (value < 100)
        {
            var ten = value / 10;
            var rest = value % 10;
            return rest == 0 ? Tens[ten] : Tens[ten] + " e " + IntegerToWords(rest);
        }

        if (value < 1000)
        {
            if (value == 100)
            {
                return "cem";
            }

            var hundred = value / 100;
            var rest = value % 100;
            return rest == 0 ? Hundreds[hundred] : Hundreds[hundred] + " e " + IntegerToWords(rest);
        }

        if (value < 1_000_000)
        {
            return Group(value, 1000, "mil", "mil");
        }

        return Group(value, 1_000_000, "milhão", "milhões");
    }

    private static string Group(long value, long scale, string one, string many)
    {
        var count = value / scale;
        var rest = value % scale;
        var prefix = count == 1
            ? (scale == 1000 ? one : "um " + one)
            : IntegerToWords(count) + " " + many;
        if (rest == 0)
        {
            return prefix;
        }

        var join = rest < 100 || rest % 100 == 0 ? " e " : " ";
        return prefix + join + IntegerToWords(rest);
    }

    private static MoneyUnits Units(string? currency)
    {
        var code = (currency ?? string.Empty).Trim().ToUpperInvariant();
        return code switch
        {
            "AOA" => new MoneyUnits("kwanza", "kwanzas", "cêntimo", "cêntimos"),
            "MZN" or "MTZ" => new MoneyUnits("metical", "meticais", "centavo", "centavos"),
            "USD" => new MoneyUnits("dólar", "dólares", "cêntimo", "cêntimos"),
            "EUR" or "" => new MoneyUnits("euro", "euros", "cêntimo", "cêntimos"),
            _ => new MoneyUnits(code.ToLowerInvariant(), code.ToLowerInvariant(), "cêntimo", "cêntimos")
        };
    }

    private readonly record struct MoneyUnits(string One, string Many, string Cent, string Cents);
}
