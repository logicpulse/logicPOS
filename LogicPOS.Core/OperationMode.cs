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

        var value = module.Trim().Replace(" ", string.Empty).Replace("-", string.Empty).Replace("_", string.Empty);
        return value.Equals("backofficemode", StringComparison.OrdinalIgnoreCase)
            || value.Equals("backoffice", StringComparison.OrdinalIgnoreCase);
    }
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
