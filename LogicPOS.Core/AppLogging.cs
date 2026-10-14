using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace LogicPOS.Core;

/// <summary>
/// GTK-compatible daily file logs under install\Logs\logYYYYMMDD.txt.
/// </summary>
public static class AppLogging
{
    private const string OutputTemplate =
        "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] ({SourceContext}) {Message:lj}{NewLine}{Exception}";

    public static void Configure(string? baseDirectory = null)
    {
        var root = string.IsNullOrWhiteSpace(baseDirectory) ? AppContext.BaseDirectory : baseDirectory;
        var logs = Path.Combine(root, "Logs");
        Directory.CreateDirectory(logs);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.File(
                path: Path.Combine(logs, "log.txt"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                shared: true,
                outputTemplate: OutputTemplate)
            .CreateLogger();
    }

    public static void AddToServices(IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddSerilog(dispose: false);
        });
    }

    public static void Information(string messageTemplate, params object?[] propertyValues)
        => Log.Information(messageTemplate, propertyValues);

    public static void Warning(Exception? exception, string messageTemplate, params object?[] propertyValues)
        => Log.Warning(exception, messageTemplate, propertyValues);

    public static void Error(Exception? exception, string messageTemplate, params object?[] propertyValues)
        => Log.Error(exception, messageTemplate, propertyValues);

    public static void Fatal(Exception? exception, string messageTemplate, params object?[] propertyValues)
        => Log.Fatal(exception, messageTemplate, propertyValues);

    public static void CloseAndFlush()
        => Log.CloseAndFlush();
}
