namespace LogicPOS.Core.BackOffice;

/// <summary>AT series registration is required only when appsettings Culture is pt-PT.</summary>
internal static class AtSeriesCommunication
{
    public static bool IsRequired(string? culture)
        => string.IsNullOrWhiteSpace(culture)
            || culture.Trim().Equals("pt-PT", StringComparison.OrdinalIgnoreCase);
}
