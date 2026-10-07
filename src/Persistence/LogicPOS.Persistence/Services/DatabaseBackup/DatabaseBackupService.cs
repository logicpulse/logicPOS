using System.Collections.Concurrent;
using LogicPOS.Application.Features.System;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Results;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Services.DatabaseBackup;

public sealed class DatabaseBackupService : IDatabaseBackupService
{
    /// <summary>
    /// One backup or restore at a time per database: two backups started together would get the same version
    /// and file name. Keyed by connection string so cloud tenants don't wait for each other.
    /// </summary>
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> DatabaseLocks = new();

    private readonly LogicPOSDbContext _database;
    private readonly ILogger<DatabaseBackupService> _logger;
    private readonly ILogger<MySqlBackupService> _mysqlLogger;
    private readonly ILogger<SqliteBackupService> _sqliteLogger;
    private readonly ILogger<SqlServerBackupService> _sqlServerLogger;
    private readonly IDatabaseMigrationsService _migrationsService;
    private readonly IDatabaseSeeder _databaseSeeder;

    public DatabaseBackupService(LogicPOSDbContext database, ILogger<DatabaseBackupService> logger,
        ILogger<MySqlBackupService> mysqlLogger, ILogger<SqliteBackupService> sqliteLogger,
        ILogger<SqlServerBackupService> sqlServerLogger, IDatabaseMigrationsService migrationsService,
        IDatabaseSeeder databaseSeeder)
    {
        _database = database;
        _logger = logger;
        _mysqlLogger = mysqlLogger;
        _sqliteLogger = sqliteLogger;
        _sqlServerLogger = sqlServerLogger;
        _migrationsService = migrationsService;
        _databaseSeeder = databaseSeeder;
    }

    public async Task<Result<string>> BackupDatabaseAsync(CancellationToken ct = default)
    {
        var databaseLock = GetDatabaseLock();
        await databaseLock.WaitAsync(ct);
        try
        {
            return await BackupDatabaseCoreAsync(ct);
        }
        finally
        {
            databaseLock.Release();
        }
    }

    public async Task<Result<string>> RestoreAsync(Guid? backupId = null, CancellationToken ct = default)
    {
        var databaseLock = GetDatabaseLock();
        await databaseLock.WaitAsync(ct);
        try
        {
            return await RestoreCoreAsync(backupId, ct);
        }
        finally
        {
            databaseLock.Release();
        }
    }

    private SemaphoreSlim GetDatabaseLock() =>
        DatabaseLocks.GetOrAdd(_database.Database.GetConnectionString() ?? string.Empty, _ => new SemaphoreSlim(1, 1));

    private async Task<Result<string>> BackupDatabaseCoreAsync(CancellationToken ct)
    {
        _logger.LogInformation("Backing up database...");

        if (_database.Database.IsMySql())
        {
            BackupSettings.EnsureBackupsFolderExists();
            var mysqlBackupService = new MySqlBackupService(_database, _mysqlLogger);
            var mysqlBackupResult = mysqlBackupService.Backup();
            return mysqlBackupResult.IsSuccess ? BackupSettings.BackupFilePath : mysqlBackupResult.ToGenericFailure<string>();
        }

        var backupFiles = BackupFiles.For(_database, _logger);
        var isSqlite = _database.Database.IsSqlite();
        SystemBackup backup;
        try
        {
            backup = await AddBackupRecordAsync(backupFiles, isSqlite, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while trying to register the database backup.");
            return Result.Unexpected<string>(ex.Message);
        }

        var backupResult = isSqlite
            ? await new SqliteBackupService(_database, backupFiles, _sqliteLogger).BackupSqliteAsync(backup.FileName!, ct)
            : await new SqlServerBackupService(_database, _sqlServerLogger).BackupAsync(backup.FileName!, ct);

        if (backupResult.IsFailure)
        {
            await RemoveBackupRecordAsync(backup);
        }

        return backupResult;
    }

    /// <summary>
    /// Saved before the backup runs, as in LogicPOS 1.4, so each backup contains its own row and still shows up
    /// in the list after it is restored.
    /// </summary>
    private async Task<SystemBackup> AddBackupRecordAsync(BackupFiles backupFiles, bool isSqlite, CancellationToken ct)
    {
        var lastVersion = await _database.SystemBackups
            .OrderByDescending(record => record.Version)
            .Select(record => record.Version)
            .FirstOrDefaultAsync(ct);

        if (isSqlite)
        {
            // Covers files whose rows were lost, e.g. when keeping them after a restore failed.
            lastVersion = Math.Max(lastVersion, backupFiles.GetHighestLocalVersion());
        }

        var backup = new SystemBackup
        {
            DatabaseType = (int)(isSqlite ? DatabaseType.Sqlite : DatabaseType.SqlServer),
            Version = lastVersion + 1,
            FilePath = isSqlite ? backupFiles.FolderPath : null
        };
        backup.FileName = backupFiles.CreateFileName(backup.Version, backup.CreatedAt);

        _database.SystemBackups.Add(backup);
        await _database.SaveChangesAsync(ct);
        return backup;
    }

    private async Task RemoveBackupRecordAsync(SystemBackup backup)
    {
        try
        {
            _database.SystemBackups.Remove(backup);
            await _database.SaveChangesAsync(CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while trying to remove the record of a failed backup: {BackupFile}", backup.FileName);
        }
    }

    public void EnsureSqliteRestore()
    {
        if (_database.Database.IsSqlite() == false)
        {
            _logger.LogInformation("Database is not sqlite, skipping sqlite restore.");
            return;
        }

        var sqliteBackupService = new SqliteBackupService(_database, BackupFiles.For(_database, _logger), _sqliteLogger);
        sqliteBackupService.EnsureSqliteRestore();
    }

    private async Task<Result<string>> RestoreCoreAsync(Guid? backupId, CancellationToken ct)
    {
        _logger.LogInformation("Restoring database...");

        if (_database.Database.IsMySql())
        {
            BackupSettings.EnsureBackupsFolderExists();
            if (File.Exists(BackupSettings.BackupFilePath) == false)
            {
                _logger.LogWarning("Backup file not found.");
                return BackupSettings.BackupFilePath;
            }

            var mysqlBackupService = new MySqlBackupService(_database, _mysqlLogger);
            var mysqlRestoreResult = mysqlBackupService.Restore();
            return mysqlRestoreResult.IsSuccess ? BackupSettings.BackupFilePath : mysqlRestoreResult.ToGenericFailure<string>();
        }

        var backupFiles = BackupFiles.For(_database, _logger);
        var backup = await FindBackupRecordAsync(backupId, ct);
        if (backupId != null && backup == null)
        {
            return Result.NotFound<string>("Backup", backupId.ToString());
        }

        if (backup != null && backupFiles.IsBackupFileName(backup.FileName) == false)
        {
            _logger.LogWarning("Backup {BackupId} has an invalid file name: {BackupFile}", backup.Id, backup.FileName);
            return Result.InvalidOperation<string>("Invalid backup file name.");
        }

        var backupsBeforeRestore = await _database.SystemBackups.AsNoTracking().ToListAsync(ct);

        if (_database.Database.IsSqlite())
        {
            var sqliteBackupService = new SqliteBackupService(_database, backupFiles, _sqliteLogger);
            var backupFilePath = backup != null ? sqliteBackupService.FindBackupFilePath(backup) : null;
            if (backupFilePath == null && backupId == null)
            {
                backupFilePath = sqliteBackupService.FindLatestLocalFilePath();
            }

            var sqliteRestoreResult = sqliteBackupService.RestoreSqlite(backupFilePath);
            if (sqliteRestoreResult.IsFailure)
            {
                return sqliteRestoreResult;
            }

            _database.ChangeTracker.Clear();
            _migrationsService.ApplyPendingMigrations();
            _databaseSeeder.ApplyNewSeed();
            _databaseSeeder.EnsureCountrySpecificConfiguration();
            await KeepBackupRecordsAsync(backupsBeforeRestore);
            return sqliteRestoreResult;
        }

        if (backup == null)
        {
            _logger.LogWarning("Sql server restore requested but the SystemBackups table has no backup.");
            return Result.NotFound<string>("Backup");
        }

        var sqlServerBackupService = new SqlServerBackupService(_database, _sqlServerLogger);
        var sqlServerRestoreResult = await sqlServerBackupService.RestoreAsync(backup.FileName!, ct);
        if (sqlServerRestoreResult.IsSuccess)
        {
            await KeepBackupRecordsAsync(backupsBeforeRestore);
        }

        return sqlServerRestoreResult;
    }

    /// <summary>
    /// A restore takes the SystemBackups table back to the moment of the backup. Rows of later backups are put back,
    /// so those backups can still be chosen and a restore of the wrong backup can be undone.
    /// The restored data itself is never touched: a failure here only leaves the list incomplete.
    /// </summary>
    private async Task KeepBackupRecordsAsync(IReadOnlyList<SystemBackup> backupsBeforeRestore)
    {
        try
        {
            _database.ChangeTracker.Clear();
            var restoredIds = await _database.SystemBackups.Select(record => record.Id).ToListAsync();
            var missingBackups = backupsBeforeRestore.Where(record => restoredIds.Contains(record.Id) == false).ToList();
            if (missingBackups.Count == 0)
            {
                return;
            }

            // The auditing interceptor stamps added rows with the user and terminal doing the restore.
            var creators = missingBackups.Select(record => (record.Id, record.CreatedBy, record.CreatedWhere)).ToList();

            _database.SystemBackups.AddRange(missingBackups);
            await _database.SaveChangesAsync(CancellationToken.None);
            _database.ChangeTracker.Clear();

            foreach (var (id, createdBy, createdWhere) in creators)
            {
                await _database.SystemBackups
                    .Where(record => record.Id == id)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(record => record.CreatedBy, createdBy)
                        .SetProperty(record => record.CreatedWhere, createdWhere));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while trying to keep the records of backups made after the restored one.");
        }
    }

    private Task<SystemBackup?> FindBackupRecordAsync(Guid? backupId, CancellationToken ct)
    {
        var backups = _database.SystemBackups.AsNoTracking().Where(record => record.IsDeleted == false);
        return backupId != null
            ? backups.FirstOrDefaultAsync(record => record.Id == backupId, ct)
            : backups.OrderByDescending(record => record.Version)
                .ThenByDescending(record => record.CreatedAt)
                .FirstOrDefaultAsync(ct);
    }
}