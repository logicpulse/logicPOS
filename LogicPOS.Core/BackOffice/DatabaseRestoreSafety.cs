namespace LogicPOS.Core.BackOffice;

/// <summary>
/// Pure checks for SQLite file restore: refuse the live DB, require typed RESTAURAR,
/// and create a timestamped .bak of the live file before overwriting.
/// </summary>
public static class DatabaseRestoreSafety
{
    public const string ConfirmWord = "RESTAURAR";

    public static bool IsConfirmWord(string? text)
        => string.Equals(text, ConfirmWord, StringComparison.Ordinal);

    public static string NormalizePath(string path)
    {
        var full = Path.GetFullPath(path.Trim());
        return full.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    public static bool IsSameDatabaseFile(string livePath, string candidatePath)
    {
        var live = NormalizePath(livePath);
        var candidate = NormalizePath(candidatePath);
        var comparison = OperatingSystem.IsWindows()
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;
        return string.Equals(live, candidate, comparison);
    }

    /// <summary>Same folder as the live DB: <c>logicpos.db.yyyyMMdd-HHmmss.bak</c>.</summary>
    public static string BuildBakPath(string livePath, DateTime stamp)
        => NormalizePath(livePath) + "." + stamp.ToString("yyyyMMdd-HHmmss") + ".bak";

    public static string? ValidateCandidateFile(string livePath, string? candidatePath)
    {
        if (string.IsNullOrWhiteSpace(candidatePath) || File.Exists(candidatePath) == false)
        {
            return "Selecione um ficheiro de backup.";
        }

        if (candidatePath.EndsWith(".db", StringComparison.OrdinalIgnoreCase) == false)
        {
            return "O ficheiro de backup não é válido.";
        }

        if (IsSameDatabaseFile(livePath, candidatePath))
        {
            return "Não é possível restaurar a base de dados ativa. Escolha outro ficheiro.";
        }

        return null;
    }

    /// <summary>
    /// Copies the live database to a .bak beside it. Returns an error message on failure
    /// (caller must not restore). <paramref name="copy"/> is injectable for tests.
    /// </summary>
    public static string? TryCreateLiveBak(
        string livePath,
        DateTime stamp,
        Action<string, string>? copy = null)
    {
        if (File.Exists(livePath) == false)
        {
            return "A base de dados ativa não foi encontrada para criar a cópia .bak.";
        }

        var bakPath = BuildBakPath(livePath, stamp);
        try
        {
            (copy ?? File.Copy)(livePath, bakPath);
            return null;
        }
        catch (Exception exception)
        {
            return "Não foi possível criar a cópia de segurança (.bak): " + exception.Message;
        }
    }

    public static string FileRestorePromptLabel(string selectedPath)
        => "Ficheiro:" + Environment.NewLine
            + selectedPath + Environment.NewLine + Environment.NewLine
            + "Escreva RESTAURAR para substituir a base atual.";
}
