using Microsoft.Extensions.Configuration;

namespace LogicPOS.Core;

public interface IOperationMode
{
    Task<bool> IsBackOfficeOnlyAsync(CancellationToken cancellationToken = default);
}

public static class OperationModes
{
    public static bool IsBackOfficeOnly(string? module)
    {
        if (string.IsNullOrWhiteSpace(module))
        {
            return false;
        }

        var value = NormalizeModuleToken(module);
        return value.Equals("backofficemode", StringComparison.OrdinalIgnoreCase)
            || value.Equals("backoffice", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Retail/parking FO uses "Ordens"; restaurant/cafe/default (and similar) use "Mesas".
    /// Matches legacy RESX <c>*_appmode_retail</c> vs <c>*_appmode_default</c>.
    /// </summary>
    public static bool UsesRetailOrderVocabulary(string? module)
    {
        if (string.IsNullOrWhiteSpace(module))
        {
            return false;
        }

        var value = NormalizeModuleToken(module);
        return value.Equals("retail", StringComparison.OrdinalIgnoreCase)
            || value.Equals("parking", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeModuleToken(string module)
        => module.Trim().Replace(" ", string.Empty).Replace("-", string.Empty).Replace("_", string.Empty);
}

public sealed class ConfigurationOperationMode : IOperationMode
{
    private readonly IConfiguration _configuration;

    public ConfigurationOperationMode(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<bool> IsBackOfficeOnlyAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(OperationModes.IsBackOfficeOnly(_configuration["DatabaseSettings:Module"]));
    }
}
