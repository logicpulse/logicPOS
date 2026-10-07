using System.Collections;

namespace LogicPOS.Application.Features.System.Licensing;

public interface ILicenseService
{
    public bool Initialize();
    public Task RefreshLicenseAsync();
    Task<byte[]> ActivateLicenseAsync(string name, string company, string fiscalNumber, string address,
        string email, string phone, string hardwareId, string assemblyVersion,
        int countryId, string softwareKey);

    Task AddMessageAsync(string hardwareId, string name, string contact, string message, string client);
    Task<bool> ConnectToWsAsync();
    Task<string?> GetLogicPosLicensingSystemVersionAsync();
    Task<byte[]?> DownloadLicenseFileAsync(string hardwareId, string version, bool haveLicense);
    Task<int> UpdateAppVersionInServerAsync(string hardwareId, string productId, string versionApp);
    public ILicenseData Information { get; }
}