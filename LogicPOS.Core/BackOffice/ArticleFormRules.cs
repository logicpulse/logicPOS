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

        var childText = Cell(fields, "ChildArticleId");
        if (articleId is not null && string.IsNullOrWhiteSpace(childText))
        {
            return null;
        }

        if (Guid.TryParse(childText, out var child) == false || child == Guid.Empty)
        {
            return "Indique o artigo associado.";
        }

        if (articleId is Guid current && current == child)
        {
            return "O artigo composto não pode incluir o próprio artigo.";
        }

        if (Regex.IsMatch(Cell(fields, "ChildQuantity"), DecimalAboveZeroPattern) == false)
        {
            return "Indique a quantidade do artigo associado.";
        }

        return null;
    }

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
