using LogicPOS.Persistence.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Services.DemoData;

/// <summary>
/// Hourly poll to generate today's demo sales when Demo licence + flag are on — like qtrack monitor task.
/// </summary>
public sealed class DemoSalesDataBackgroundService : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromHours(1);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly DatabaseSettings _settings;
    private readonly ILogger<DemoSalesDataBackgroundService> _logger;

    public DemoSalesDataBackgroundService(
        IServiceScopeFactory scopeFactory,
        DatabaseSettings settings,
        ILogger<DemoSalesDataBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _settings = settings;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Cloud multi-tenant has no ambient client id here — dashboard/preference updates seed per request.
        if (_settings.UseCloud)
        {
            _logger.LogInformation("DemoSalesDataBackgroundService skipped (UseCloud=true; per-request ensure).");
            return;
        }

        _logger.LogInformation("DemoSalesDataBackgroundService started");
        try
        {
            await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        while (stoppingToken.IsCancellationRequested == false)
        {
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
                await DemoDataPreferenceSeeder.EnsureAsync(database, stoppingToken).ConfigureAwait(false);

                var service = scope.ServiceProvider.GetRequiredService<IDemoSalesDataService>();
                var result = await service.EnsureAsync(force: false, stoppingToken).ConfigureAwait(false);
                if (result.Ran)
                {
                    _logger.LogInformation(
                        "Demo sales background run: {Code} days={Days} docs={Docs}",
                        result.SummaryCode, result.DaysProcessed, result.DocumentsGenerated);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Demo sales background run failed");
            }

            try
            {
                await Task.Delay(Interval, stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        _logger.LogInformation("DemoSalesDataBackgroundService stopped");
    }
}
