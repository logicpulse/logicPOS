using LogicPOS.Application.Features.System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Services.DatabaseBackup;

public class ScheduledBackupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ScheduledBackupService> _logger;

    public ScheduledBackupService(IServiceProvider serviceProvider, ILogger<ScheduledBackupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var nextMidnight = now.Date.AddDays(1);
            var delay = nextMidnight - now;
            _logger.LogInformation($"Next backup scheduled at {nextMidnight}");
            try
            {
                await Task.Delay(delay, stoppingToken);
                await using var scope = _serviceProvider.CreateAsyncScope();
                var backupService = scope.ServiceProvider.GetRequiredService<IDatabaseBackupService>();
                _logger.LogInformation("Starting midnight backup...");
                await backupService.BackupDatabaseAsync(stoppingToken);
                _logger.LogInformation("Midnight backup completed.");
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Midnight backup cancelled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Midnight backup failed.");
            }
        }
    }
}