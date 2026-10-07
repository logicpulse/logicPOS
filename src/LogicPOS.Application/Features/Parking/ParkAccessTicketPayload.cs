using System.Globalization;

namespace LogicPOS.Application.Features.Parking;

public readonly struct ParkAccessTicketLookup
{
    public ParkAccessTicketLookup(int nPedido, string? articleCode)
    {
        NPedido = nPedido;
        ArticleCode = articleCode;
    }

    public int NPedido { get; }

    public string? ArticleCode { get; }
}

public static class ParkAccessTicketPayload
{
    public const string Prefix = "ACCESS_";
    public const int NPedidoWidth = 8;
    public const int MaxNPedido = 99999999;

    public static bool TryParse(string? raw, out ParkAccessTicketLookup lookup)
    {
        lookup = default;
        var value = raw?.Trim() ?? string.Empty;
        if (value.Length == 0)
            return false;

        if (value.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase))
        {
            var rest = value.Substring(Prefix.Length);
            var lastUnderscore = rest.LastIndexOf('_');
            if (lastUnderscore <= 0 || lastUnderscore >= rest.Length - 1)
                return false;

            var article = rest.Substring(0, lastUnderscore).Trim();
            var numberPart = rest.Substring(lastUnderscore + 1).Trim();
            if (article.Length == 0 || !TryParseNPedido(numberPart, out var nPedido))
                return false;

            lookup = new ParkAccessTicketLookup(nPedido, article);
            return true;
        }

        if (TryParseNPedido(value, out var digitsOnly))
        {
            lookup = new ParkAccessTicketLookup(digitsOnly, articleCode: null);
            return true;
        }

        return false;
    }

    private static bool TryParseNPedido(string text, out int nPedido)
    {
        nPedido = 0;
        if (string.IsNullOrWhiteSpace(text) || text.Length > NPedidoWidth)
            return false;

        if (!int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var parsed))
            return false;

        if (parsed < 1 || parsed > MaxNPedido)
            return false;

        nPedido = parsed;
        return true;
    }
}
