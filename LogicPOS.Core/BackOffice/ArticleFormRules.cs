using System.Text.RegularExpressions;

namespace LogicPOS.Core.BackOffice;

public static class ArticleFormRules
{
    // Patterns copied from the GTK SettingsApp article dialog.
    private const string CodePattern = @"^[0-9A-Za-z€$&@#%()\[\]+\-_ \.\\\/]{2,25}$";
    private const string DealerPattern = @"^[À-ÿ\-_ \s\w]*$";
    private const string DesignationPattern = @"^[À-ÿ€$&@#%ºª(){}\[\]';:!?<>+\-_ \.\,\s\\\/\*\w]{1,200}$";
    private const string ButtonPattern = @"^[0-9A-Za-zéúíóáÉÚÍÓÁèùìòàÈÙÌÒÀõãñÕÃÑêûîôâÊÛÎÔÂçÇ€$&@#%ºª()\[\]';:!?<>+\-_ \.\,\s\\\/\*]{2,35}$";
    private const string DecimalPattern = @"^\s*(?=.*[0-9])\d*(?:[\.,]\d{1,4})?\s*$";
    private const string DecimalFromZeroPattern = @"^\s*(?=.*[0-9])\d*(?:[\.,]\d{1,})?\s*$";
    private const string DecimalAboveZeroPattern = @"^\s*(?=.*[1-9])\d*(?:[\.,]\d{1,})?\s*$";
    private const string PercentPattern = @"^(100([\.\,][0]{1,})?$|[0-9]{1,2}([\.\,][0-9]{1,})?)$";
    private const string BarcodePattern = @"^[0-9A-Za-z]+$";

    public static string? Validate(IReadOnlyDictionary<string, string> fields, bool vatRequiresExemption, Guid? articleId)
    {
        var error = Match(fields, "Code", "o código", CodePattern, required: true)
            ?? Match(fields, "CodeDealer", "o código de fornecedor", DealerPattern, required: false)
            ?? Match(fields, "Designation", "a designação", DesignationPattern, required: true)
            ?? Match(fields, "ButtonLabel", "a etiqueta do botão", ButtonPattern, required: false)
            ?? NeedGuid(fields, "SubfamilyId", "a subfamília")
            ?? NeedGuid(fields, "TypeId", "o tipo")
            ?? NeedGuid(fields, "ClassId", "a classe")
            ?? NeedGuid(fields, "MeasurementUnitId", "a unidade de medida")
            ?? NeedGuid(fields, "SizeUnitId", "a unidade de tamanho")
            ?? NeedGuid(fields, "VatDirectSellingId", "o IVA de venda direta")
            ?? NeedGuid(fields, "VatOnTableId", "o IVA de mesa")
            ?? Match(fields, "Price1", "o preço 1", DecimalFromZeroPattern, required: true)
            ?? Match(fields, "Price2", "o preço 2", DecimalFromZeroPattern, required: false)
            ?? Match(fields, "Price3", "o preço 3", DecimalFromZeroPattern, required: false)
            ?? Match(fields, "Price4", "o preço 4", DecimalFromZeroPattern, required: false)
            ?? Match(fields, "Price5", "o preço 5", DecimalFromZeroPattern, required: false)
            ?? Match(fields, "Price1Promotion", "a promoção 1", DecimalFromZeroPattern, required: false)
            ?? Match(fields, "Price2Promotion", "a promoção 2", DecimalFromZeroPattern, required: false)
            ?? Match(fields, "Price3Promotion", "a promoção 3", DecimalFromZeroPattern, required: false)
            ?? Match(fields, "Price4Promotion", "a promoção 4", DecimalFromZeroPattern, required: false)
            ?? Match(fields, "Price5Promotion", "a promoção 5", DecimalFromZeroPattern, required: false)
            ?? Match(fields, "Discount", "o desconto", PercentPattern, required: false)
            ?? Match(fields, "Barcode", "o código de barras", BarcodePattern, required: false)
            ?? Match(fields, "MinimumStock", "o stock mínimo", DecimalPattern, required: false)
            ?? Match(fields, "Tare", "a tara", DecimalPattern, required: false)
            ?? Match(fields, "Weight", "o peso", DecimalPattern, required: false)
            ?? Match(fields, "DefaultQuantity", "a quantidade predefinida", DecimalPattern, required: false);

        if (error is not null)
        {
            return error;
        }

        if (vatRequiresExemption && (Guid.TryParse(Cell(fields, "VatExemptionReasonId"), out var reason) == false || reason == Guid.Empty))
        {
            return "Indique o motivo de isenção de IVA.";
        }

        if (IsOn(fields, "IsComposed") == false)
        {
            return null;
        }

        var lines = ReadComposition(fields, articleId, out var compositionError);
        if (compositionError is not null)
        {
            return compositionError;
        }

        // Creating a composed article needs at least one child. Saving an edit with none clears the children.
        return lines.Count == 0 && articleId is null
            ? "Indique o artigo associado."
            : null;
    }

    public readonly record struct CompositionPlan(bool ClearExisting, IReadOnlyList<CompositionLine> Lines, string? Error);

    /// <summary>
    /// Decides how a save treats associated articles. Unchecking the composed flag, or leaving no children, removes them.
    /// A read error is returned and must not be ignored.
    /// </summary>
    public static CompositionPlan PlanComposition(IReadOnlyDictionary<string, string> fields, Guid? articleId)
    {
        if (fields.ContainsKey("IsComposed") == false)
        {
            return new CompositionPlan(false, [], null);
        }

        if (IsOn(fields, "IsComposed") == false)
        {
            return new CompositionPlan(true, [], null);
        }

        var lines = ReadComposition(fields, articleId, out var error);
        if (error is not null)
        {
            return new CompositionPlan(false, [], error);
        }

        return new CompositionPlan(lines.Count == 0, lines, null);
    }

    public readonly record struct CompositionLine(Guid ChildId, decimal Quantity);

    /// <summary>
    /// Reads every associated article. Indexed keys (ChildArticleId:0) and the legacy single key are both accepted.
    /// Blank rows are ignored so the editor can offer an empty extra line.
    /// </summary>
    public static IReadOnlyList<CompositionLine> ReadComposition(IReadOnlyDictionary<string, string> fields, Guid? articleId, out string? error)
    {
        error = null;
        var lines = new List<CompositionLine>();
        var seen = new HashSet<Guid>();
        foreach (var suffix in CompositionSuffixes(fields))
        {
            var childText = Cell(fields, ArticleKey(suffix));
            if (Guid.TryParse(childText, out var child) == false || child == Guid.Empty)
            {
                continue;
            }

            if (articleId is Guid current && current == child)
            {
                error = "O artigo composto não pode incluir o próprio artigo.";
                return [];
            }

            if (seen.Add(child) == false)
            {
                error = "O artigo associado está repetido.";
                return [];
            }

            var quantityText = Cell(fields, QuantityKey(suffix));
            if (Regex.IsMatch(quantityText, DecimalAboveZeroPattern) == false)
            {
                error = "Indique a quantidade do artigo associado.";
                return [];
            }

            if (TryQuantity(quantityText, out var quantity) == false)
            {
                error = "Indique a quantidade do artigo associado.";
                return [];
            }

            lines.Add(new CompositionLine(child, quantity));
        }

        return lines;
    }

    private static IEnumerable<string> CompositionSuffixes(IReadOnlyDictionary<string, string> fields)
    {
        var suffixes = new List<string>();
        foreach (var key in fields.Keys)
        {
            string? suffix = null;
            if (key == "ChildArticleId" || key == "ChildQuantity")
            {
                suffix = string.Empty;
            }
            else if (key.StartsWith("ChildArticleId:", StringComparison.Ordinal))
            {
                suffix = key["ChildArticleId:".Length..];
            }
            else if (key.StartsWith("ChildQuantity:", StringComparison.Ordinal))
            {
                suffix = key["ChildQuantity:".Length..];
            }

            if (suffix is not null && suffixes.Contains(suffix) == false)
            {
                suffixes.Add(suffix);
            }
        }

        return suffixes;
    }

    private static bool TryQuantity(string text, out decimal value)
        => FormDecimal.TryParse(text, out value) && value > 0;

    private static string ArticleKey(string suffix) => suffix.Length == 0 ? "ChildArticleId" : "ChildArticleId:" + suffix;

    private static string QuantityKey(string suffix) => suffix.Length == 0 ? "ChildQuantity" : "ChildQuantity:" + suffix;

    private static string? Match(IReadOnlyDictionary<string, string> fields, string key, string label, string pattern, bool required)
    {
        var value = Cell(fields, key);
        if (string.IsNullOrWhiteSpace(value))
        {
            return required ? $"Indique {label}." : null;
        }

        return Regex.IsMatch(value, pattern) ? null : $"O valor de {label} não é válido.";
    }

    private static string? NeedGuid(IReadOnlyDictionary<string, string> fields, string key, string label)
    {
        return Guid.TryParse(Cell(fields, key), out var id) && id != Guid.Empty
            ? null
            : $"Indique {label}.";
    }

    private static bool IsOn(IReadOnlyDictionary<string, string> fields, string key)
        => string.Equals(Cell(fields, key), "true", StringComparison.OrdinalIgnoreCase);

    private static string Cell(IReadOnlyDictionary<string, string> fields, string key)
        => fields.TryGetValue(key, out var value) ? value.Trim() : string.Empty;
}
