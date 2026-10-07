using LogicPOS.Application.Features.System;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Services.DatabaseBackup;

/// <summary>
/// Automatic backups as in LogicPOS 1.4, driven by the DATABASE_BACKUP_* preferences, which are read on every check
/// so changes apply without restarting the API.
/// </summary>
public class ScheduledBackupService : BackgroundService
{
    private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(1);

    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ScheduledBackupService> _logger;
    private DateTime _lastAttempt = DateTime.MinValue;

    public ScheduledBackupService(IServiceProvider serviceProvider, ILogger<ScheduledBackupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(CheckInterval, stoppingToken);
                await BackupIfDueAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Automatic backup cancelled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Automatic backup failed.");
            }
        }
    }

    private async Task BackupIfDueAsync(CancellationToken ct)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();

        var settings = AutomaticBackupSettings.Load(database, _logger);
        if (settings == null)
        {
            return;
        }

        var now = DateTime.Now;
        var lastBackup = await database.SystemBackups
            .Where(backup => backup.IsDeleted == false)
            .OrderByDescending(backup => backup.CreatedAt)
            .Select(backup => (DateTime?)backup.CreatedAt)
            .FirstOrDefaultAsync(ct) ?? DateTime.MinValue;
        // Failed attempts also count, so a persistent error is retried once per interval and not every minute.
        if (settings.IsDue(now, lastBackup > _lastAttempt ? lastBackup : _lastAttempt) == false)
        {
            return;
        }

        _lastAttempt = now;
        _logger.LogInformation("Starting automatic backup...");
        var backupService = scope.ServiceProvider.GetRequiredService<IDatabaseBackupService>();
        var backupResult = await backupService.BackupDatabaseAsync(ct);
        if (backupResult.IsFailure)
        {
            _logger.LogError("Automatic backup failed: {Error}", backupResult.Error.Message);
            return;
        }

        _logger.LogInformation("Automatic backup completed: {BackupFile}", backupResult.Value);
    }
}
