using ErrorOr;
using LogicPOS.ApiServer.Data;
using LogicPOS.ApiServer.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.ApiServer.Services;

public sealed class HealthService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<HealthService> _logger;
    private readonly SystemVersionService _systemVersionService;

    public HealthService(
        ApplicationDbContext dbContext,
        IWebHostEnvironment environment,
        ILogger<HealthService> logger,
        SystemVersionService systemVersionService)
    {
        _dbContext = dbContext;
        _environment = environment;
        _logger = logger;
        _systemVersionService = systemVersionService;
    }

    public async Task<ErrorOr<HealthResponse>> GetHealthAsync(CancellationToken cancellationToken = default)
    {
        var databaseAvailable = false;

        try
        {
            databaseAvailable = await _dbContext.Database.CanConnectAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Database connectivity check failed.");
        }

        return new HealthResponse
        {
            Status = databaseAvailable ? "Healthy" : "Degraded",
            DatabaseAvailable = databaseAvailable,
            Environment = _environment.EnvironmentName,
            TimestampUtc = DateTime.UtcNow,
            Version = _systemVersionService.GetApiVersion()
        };
    }
}
