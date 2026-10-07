namespace LogicPOS.Application.Features.System.Licensing;

public interface ICloudAccountLicenseReader
{
    /// <summary>
    /// Null when the process is not in cloud mode or the request has no client id yet.
    /// </summary>
    CloudAccountLicense? GetForCurrentTenant();
}
