namespace LogicPOS.Application.Features.System.Licensing;

public interface ICloudAccountLicenseReader
{
    /// <summary>
    /// Null when the process is not in cloud mode or the request has no client id yet.
    /// </summary>
    CloudAccountLicense? GetForCurrentTenant();

    /// <summary>
    /// Drop the cached cloud account snapshot for the current tenant so module flags
    /// (e.g. ModuleStocks) are re-read on the next <see cref="GetForCurrentTenant"/>.
    /// </summary>
    void InvalidateCurrentTenant();
}
