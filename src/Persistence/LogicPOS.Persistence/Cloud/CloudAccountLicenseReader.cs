using LogicPOS.Application.Features.System.Licensing;
using LogicPOS.Persistence.Database;

namespace LogicPOS.Persistence.Cloud;

public sealed class CloudAccountLicenseReader : ICloudAccountLicenseReader
{
    private readonly DatabaseSettings _settings;
    private readonly ICloudAccountCatalog _catalog;

    public CloudAccountLicenseReader(DatabaseSettings settings, ICloudAccountCatalog catalog)
    {
        _settings = settings;
        _catalog = catalog;
    }

    public CloudAccountLicense? GetForCurrentTenant()
    {
        if (_settings.UseCloud == false)
            return null;

        var clientId = TenantContext.AmbientClientId;
        if (string.IsNullOrWhiteSpace(clientId))
            return null;

        var account = _catalog.Find(clientId);
        if (account.Found == false)
            return CloudAccountLicense.Inactive;

        return new CloudAccountLicense(
            IsActive: true,
            ModuleStocks: account.ModuleStocks,
            ModuleOnlineInvoicing: account.ModuleOnlineInvoicing,
            ModuleSms: account.ModuleSms);
    }
}
