using System.Collections.Concurrent;
using System.Data.Common;
using System.Globalization;
using LogicPOS.Persistence.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Services.DatabaseBackup;

/// <summary>
/// Backup files named as in LogicPOS 1.4: {type}_{database}_{version}_{yyyyMMdd_HHmmss}.{extension}, never overwritten.
/// </summary>
public sealed class BackupFiles
{
    private const string PathBackupsToken = "PATH_BACKUPS";
    private const string DefaultFolderName = "Backups";
    private const string TimestampFormat = "yyyyMMdd_HHmmss";
    private static readonly ConcurrentDictionary<string, byte> UnreachableFolders = new();

    private readonly Lazy<string> _folderPath;
    private readonly string _prefix;
    private readonly string _extension;

    public BackupFiles(string folderPath, string databaseType, string databaseName, string extension)
        : this(() => folderPath, databaseType, databaseName, extension)
    {
    }

    private BackupFiles(Func<string> folderPath, string databaseType, string databaseName, string extension)
    {
        _folderPath = new Lazy<string>(folderPath);
        _prefix = $"{databaseType}_{string.Join("_", databaseName.Split(Path.GetInvalidFileNameChars()))}_".ToLowerInvariant();
        _extension = extension;
    }

    public string FolderPath => _folderPath.Value;

    public static BackupFiles For(LogicPOSDbContext database, ILogger logger)
    {
        Func<string> folderPath = () => GetFolderPath(database, logger);

        if (database.Database.IsSqlite())
        {
            var dataSource = new SqliteConnectionStringBuilder(database.Database.GetConnectionString()).DataSource;
            return new BackupFiles(folderPath, "sqlite", Path.GetFileNameWithoutExtension(dataSource), "db");
        }

        var databaseName = database.Database.GetDbConnection().Database;
        return database.Database.IsMySql()
            ? new BackupFiles(folderPath, "mysql", databaseName, "sql")
            : new BackupFiles(folderPath, "mssqlserver", databaseName, "bak");
    }

    public void EnsureFolderExists() => Directory.CreateDirectory(FolderPath);

    public string CreateFileName(uint version, DateTime backupDate) =>
        $"{_prefix}{version}_{backupDate.ToString(TimestampFormat, CultureInfo.InvariantCulture)}.{_extension}";

    public string GetFilePath(string fileName) => Path.Combine(FolderPath, fileName);

    /// <summary>
    /// Backup names come from the SystemBackups table and end up in a file path or in a RESTORE statement,
    /// so anything with a folder in it is rejected.
    /// </summary>
    public bool IsBackupFileName(string? fileName) =>
        string.IsNullOrWhiteSpace(fileName) == false &&
        Path.GetFileName(fileName) == fileName &&
        fileName.EndsWith("." + _extension, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Only for providers whose backups the API writes itself.
    /// </summary>
    public string? FindHighestVersionFilePath() =>
        EnumerateLocalBackups()
            .OrderByDescending(backup => backup.Version)
            .Select(backup => backup.FilePath)
            .FirstOrDefault();

    public uint GetHighestLocalVersion() =>
        EnumerateLocalBackups().Select(backup => backup.Version).DefaultIfEmpty().Max();

    private IEnumerable<(string FilePath, uint Version)> EnumerateLocalBackups()
    {
        if (Directory.Exists(FolderPath) == false)
        {
            yield break;
        }

        foreach (var filePath in Directory.EnumerateFiles(FolderPath, $"{_prefix}*.{_extension}"))
        {
            if (TryParse(Path.GetFileName(filePath), out var version, out _))
            {
                yield return (filePath, version);
            }
        }
    }

    private bool TryParse(string fileName, out uint version, out DateTime backupDate)
    {
        version = 0;
        backupDate = default;

        if (fileName.StartsWith(_prefix, StringComparison.OrdinalIgnoreCase) == false ||
            fileName.EndsWith("." + _extension, StringComparison.OrdinalIgnoreCase) == false)
        {
            return false;
        }

        var parts = Path.GetFileNameWithoutExtension(fileName[_prefix.Length..]).Split('_');
        return parts.Length == 3 &&
               uint.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out version) &&
               DateTime.TryParseExact($"{parts[1]}_{parts[2]}", TimestampFormat, CultureInfo.InvariantCulture,
                   DateTimeStyles.None, out backupDate);
    }

    public static string DefaultFolderPath => Path.Combine(AppContext.BaseDirectory, DefaultFolderName);

    /// <summary>
    /// PATH_BACKUPS is edited from the POS and may name a folder on the POS machine, while the API can run on another one.
    /// Relative values resolve against the API folder; absolute ones are used only when that folder exists or can be
    /// created on the API machine, otherwise backups go to the API's own Backups folder.
    /// </summary>
    public static string ResolveFolderPath(string? pathBackups, ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(pathBackups))
        {
            return DefaultFolderPath;
        }

        try
        {
            var folderPath = Path.GetFullPath(pathBackups.Trim(), AppContext.BaseDirectory);
            Directory.CreateDirectory(folderPath);
            return folderPath;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
        {
            if (UnreachableFolders.TryAdd(pathBackups, 0))
            {
                logger.LogWarning(ex, "PATH_BACKUPS {PathBackups} is not reachable from the API machine. Backups go to {DefaultFolderPath}.",
                    pathBackups, DefaultFolderPath);
            }

            return DefaultFolderPath;
        }
    }

    private static string GetFolderPath(LogicPOSDbContext database, ILogger logger)
    {
        string? pathBackups = null;
        try
        {
            pathBackups = database.PreferenceParameters.AsNoTracking()
                .Where(parameter => parameter.Token == PathBackupsToken)
                .Select(parameter => parameter.Value)
                .FirstOrDefault();
        }
        catch (DbException)
        {
            // The preferences table doesn't exist before the first migration.
        }

        return ResolveFolderPath(pathBackups, logger);
    }
}
