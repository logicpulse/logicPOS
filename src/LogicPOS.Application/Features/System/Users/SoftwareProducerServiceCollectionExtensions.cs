using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LogicPOS.Application.Features.System.Users;

/// <summary>
/// DI registration for SoftwareProducer authentication.
/// </summary>
public static class SoftwareProducerServiceCollectionExtensions
{
    /// <summary>
    /// Optional environment variable overlay when <c>SoftwareProducer:Verifier</c> is empty.
    /// Standard configuration also maps <c>SoftwareProducer__Verifier</c>.
    /// </summary>
    public const string VerifierEnvironmentVariableName = "LOGICPOS_SOFTWARE_PRODUCER_VERIFIER";

    /// <summary>
    /// Binds <see cref="SoftwareProducerOptions"/> from section <c>SoftwareProducer</c>,
    /// registers <see cref="ISoftwareProducerPasswordVerifier"/>, and wires
    /// <see cref="SoftwareProducerAuth.Matches"/> when the verifier singleton is resolved
    /// (or call <see cref="UseSoftwareProducerAuth"/> after building the provider).
    /// </summary>
    public static IServiceCollection AddSoftwareProducerAuth(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions<SoftwareProducerOptions>()
            .Bind(configuration.GetSection(SoftwareProducerOptions.SectionName));

        services.PostConfigure<SoftwareProducerOptions>(options =>
        {
            if (!string.IsNullOrWhiteSpace(options.Verifier))
                return;

            var fromEnv = Environment.GetEnvironmentVariable(VerifierEnvironmentVariableName);
            if (!string.IsNullOrWhiteSpace(fromEnv))
                options.Verifier = fromEnv;
        });

        services.AddSingleton<ISoftwareProducerPasswordVerifier>(sp =>
        {
            var verifier = new SoftwareProducerPasswordVerifier(
                sp.GetRequiredService<IOptionsMonitor<SoftwareProducerOptions>>(),
                sp.GetRequiredService<ILogger<SoftwareProducerPasswordVerifier>>());

            var logger = sp.GetService<ILoggerFactory>()?.CreateLogger(typeof(SoftwareProducerAuth).FullName ?? nameof(SoftwareProducerAuth));
            SoftwareProducerAuth.Configure(verifier, logger);
            return verifier;
        });

        return services;
    }

    /// <summary>
    /// Ensures the static <see cref="SoftwareProducerAuth.Matches"/> entry point uses the DI verifier.
    /// Call once after building the <see cref="IServiceProvider"/> (e.g. in Program.cs).
    /// </summary>
    public static IServiceProvider UseSoftwareProducerAuth(this IServiceProvider services)
    {
        ArgumentNullException.ThrowIfNull(services);
        _ = services.GetRequiredService<ISoftwareProducerPasswordVerifier>();
        return services;
    }
}
