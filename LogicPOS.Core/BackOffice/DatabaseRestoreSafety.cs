using System.Globalization;
using Microsoft.Data.Sqlite;

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
        => NormalizePath(livePath) + "." + stamp.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture) + ".bak";

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
    /// Creates a consistent .bak of the live database via the SQLite online backup API
    /// (includes pages still in the WAL). Returns an error message on failure
    /// (caller must not restore). <paramref name="backup"/> is injectable for tests.
    /// </summary>
    public static string? TryCreateLiveBak(
        string livePath,
        DateTime stamp,
        Action<string, string>? backup = null)
    {
        if (File.Exists(livePath) == false)
        {
            return "A base de dados ativa não foi encontrada para criar a cópia .bak.";
        }

        var bakPath = BuildBakPath(livePath, stamp);
        try
        {
            (backup ?? BackupSqliteFile)(livePath, bakPath);
            return null;
        }
        catch (Exception exception)
        {
            TryDelete(bakPath);
            return "Não foi possível criar a cópia de segurança (.bak): " + exception.Message;
        }
    }

    /// <summary>
    /// Same pattern as <c>BackOfficeListingService.BackupAsync</c>: open source read-only,
    /// open destination, <see cref="SqliteConnection.BackupDatabase"/>, then release pools
    /// so the live file is not left locked for the subsequent restore.
    /// </summary>
    public static void BackupSqliteFile(string sourcePath, string destinationPath)
    {
        SqliteConnection.ClearAllPools();

        using var source = new SqliteConnection(
            new SqliteConnectionStringBuilder
            {
                DataSource = sourcePath,
                Mode = SqliteOpenMode.ReadOnly,
                Pooling = false
            }.ToString());
        using var destination = new SqliteConnection(
            new SqliteConnectionStringBuilder
            {
                DataSource = destinationPath,
                Mode = SqliteOpenMode.ReadWriteCreate,
                Pooling = false
            }.ToString());

        source.Open();
        destination.Open();
        source.BackupDatabase(destination);
        destination.Close();
        source.Close();

        SqliteConnection.ClearPool(source);
        SqliteConnection.ClearPool(destination);
    }

    public static string FileRestorePromptLabel(string selectedPath)
        => "Ficheiro:" + Environment.NewLine
            + selectedPath + Environment.NewLine + Environment.NewLine
            + "Escreva RESTAURAR para substituir a base atual.";

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch
        {
            // Best effort only — failure to delete a partial .bak must not hide the backup error.
        }
    }
}
