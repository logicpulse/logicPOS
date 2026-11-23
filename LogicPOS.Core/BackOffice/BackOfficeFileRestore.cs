namespace LogicPOS.Core.BackOffice;

/// <summary>
/// Sistema → «Restaurar de ficheiro» is a menu action (not a listing page title key for navigation).
/// </summary>
public static class BackOfficeFileRestore
{
    public const string MenuTitle = "Restaurar de ficheiro";

    public static bool IsMenuTitle(string? title)
        => string.Equals(title, MenuTitle, StringComparison.Ordinal);
}
