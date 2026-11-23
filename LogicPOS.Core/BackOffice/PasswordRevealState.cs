namespace LogicPOS.Core.BackOffice;

/// <summary>Pure toggle state for masked password fields (no secret values).</summary>
public static class PasswordRevealState
{
    public const char Mask = '•';

    public static bool IsHidden(char passwordChar) => passwordChar != '\0';

    public static char NextPasswordChar(bool currentlyHidden) => currentlyHidden ? '\0' : Mask;

    public static string Tip(bool currentlyHidden)
        => currentlyHidden ? "Mostrar palavra-passe" : "Ocultar palavra-passe";
}
