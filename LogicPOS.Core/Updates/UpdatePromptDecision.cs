namespace LogicPOS.Core.Updates;

/// <summary>
/// Whether Sistema → Atualizar may ask to download. Uses <see cref="ApplicationVersionComparer"/> as-is.
/// </summary>
public readonly record struct UpdatePromptDecision(bool Ask, string Version)
{
    public static UpdatePromptDecision For(string? published, string? installed)
    {
        if (string.IsNullOrWhiteSpace(published)
            || ApplicationVersionComparer.IsNewer(published, installed) == false)
        {
            return new UpdatePromptDecision(false, string.Empty);
        }

        return new UpdatePromptDecision(true, published.Trim());
    }

    public string Question
        => Ask
            ? $"Descarregar a atualização {Version} e reiniciar? A configuração (appsettings) e a base de dados local mantêm-se."
            : string.Empty;
}
