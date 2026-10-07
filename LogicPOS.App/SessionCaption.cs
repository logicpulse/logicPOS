namespace LogicPOS.App;

internal static class SessionCaption
{
    public static string Format(string? terminalName, string userName)
    {
        return string.IsNullOrWhiteSpace(terminalName) ? userName : $"{terminalName} : {userName}";
    }
}
