namespace LogicPOS.Core.FrontOffice;

/// <summary>
/// Thermal paper widths stored on the printer as ThermalMaxCharsPerLine* (font A normal / bold, font B small).
/// </summary>
public static class ThermalPaper
{
    public const string FieldKey = "PaperWidth";

    public const string FieldLabel = "Largura do papel";

    private static readonly (int Millimetres, int Normal, int Bold, int Small)[] Sizes =
    [
        (80, 48, 44, 64),
        (70, 42, 38, 56),
        (60, 36, 33, 48),
        (58, 32, 30, 42)
    ];

    public static IReadOnlyList<string> Labels { get; } = Sizes.Select(size => Label(size.Millimetres)).ToList();

    public static string DefaultLabel => Label(80);

    public static string Label(int millimetres) => $"{millimetres} mm";

    /// <summary>Paper width that best matches the stored normal column count (80 mm when unset).</summary>
    public static int WidthOf(int? normalColumns)
    {
        if (normalColumns is not > 0)
        {
            return 80;
        }

        return Sizes.OrderBy(size => Math.Abs(size.Normal - normalColumns.Value)).First().Millimetres;
    }

    public static string LabelOf(int? normalColumns) => Label(WidthOf(normalColumns));

    public static (int Normal, int Bold, int Small) Columns(string? label)
    {
        var digits = new string((label ?? string.Empty).TakeWhile(char.IsDigit).ToArray());
        var millimetres = int.TryParse(digits, out var parsed) ? parsed : 80;
        var size = Sizes.FirstOrDefault(item => item.Millimetres == millimetres);
        return size.Millimetres == 0 ? (Sizes[0].Normal, Sizes[0].Bold, Sizes[0].Small) : (size.Normal, size.Bold, size.Small);
    }
}
