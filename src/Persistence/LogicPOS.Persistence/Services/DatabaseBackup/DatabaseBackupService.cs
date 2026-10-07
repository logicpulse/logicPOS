using LogicPOS.Application.Features.System;
using LogicPOS.Domain.Results;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Services.DatabaseBackup;

public sealed class DatabaseBackupService : IDatabaseBackupService
{
    private readonly LogicPOSDbContext _database;
    private readonly ILogger<DatabaseBackupService> _logger;
    private readonly ILogger<MySqlBackupService> _mysqlLogger;
    private readonly ILogger<SqliteBackupService> _sqliteLogger;
    private readonly ILogger<SqlServerBackupService> _sqlServerLogger;

    public DatabaseBackupService(LogicPOSDbContext database, ILogger<DatabaseBackupService> logger,
        ILogger<MySqlBackupService> mysqlLogger, ILogger<SqliteBackupService> sqliteLogger,
        ILogger<SqlServerBackupService> sqlServerLogger)
    {
        _database = database;
        _logger = logger;
        _mysqlLogger = mysqlLogger;
        _sqliteLogger = sqliteLogger;
        _sqlServerLogger = sqlServerLogger;
    }

    public async Task<Result> BackupDatabaseAsync(CancellationToken ct = default)
    {
        BackupSettings.EnsureBackupsFolderExists();

        _logger.LogInformation("Backing up database...");

        if (_database.Database.IsMySql())
        {
            var mysqlBackupService = new MySqlBackupService(_database, _mysqlLogger);
            return mysqlBackupService.Backup();
        }

        if (_database.Database.IsSqlite())
        {
            var sqliteBackupService = new SqliteBackupService(_database, _sqliteLogger);
            return await sqliteBackupService.BackupSqliteAsync(ct);
        }

        var sqlServerBackupService = new SqlServerBackupService(_database, _sqlServerLogger);
        return await sqlServerBackupService.BackupAsync(ct);
    }

    public void EnsureSqliteRestore()
    {
        if (_database.Database.IsSqlite() == false)
        {
            _logger.LogInformation("Database is not sqlite, skipping sqlite restore.");
            return;
        }

        var sqliteBackupService = new SqliteBackupService(_database, _sqliteLogger);
        sqliteBackupService.EnsureSqliteRestore();
    }

    public async Task<Result> RestoreAsync(CancellationToken ct = default)
    {
        BackupSettings.EnsureBackupsFolderExists();

        _logger.LogInformation("Restoring database...");

        if (File.Exists(BackupSettings.BackupFilePath) == false)
        {
            _logger.LogWarning("Backup file not found.");
            return Result.Success();
        }

        if (_database.Database.IsMySql())
        {
            var mysqlBackupService = new MySqlBackupService(_database, _mysqlLogger);
            return mysqlBackupService.Restore();
        }

        if (_database.Database.IsSqlite())
        {
            var sqliteBackupService = new SqliteBackupService(_database, _sqliteLogger);
            return sqliteBackupService.ScheduleRestore();
        }

        var sqlServerBackupService = new SqlServerBackupService(_database, _sqlServerLogger);
        return await sqlServerBackupService.RestoreAsync(ct);
    }
}