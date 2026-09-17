using LogicPOS.ApiServer.Data;
using LogicPOS.ApiServer.Data.Entities;
using LogicPOS.ApiServer.DTOs;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.ApiServer.Services;

public sealed class LicensingService
{
    private const int SingletonLicenseId = 1;
    private readonly ApplicationDbContext _dbContext;
    private readonly SystemVersionService _systemVersionService;

    public LicensingService(ApplicationDbContext dbContext, SystemVersionService systemVersionService)
    {
        _dbContext = dbContext;
        _systemVersionService = systemVersionService;
    }

    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        var license = await GetOrCreateLicenseAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(license.Version))
        {
            license.Version = _systemVersionService.GetApiVersion();
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<LicenseResponse> GetDataAsync(CancellationToken cancellationToken = default)
    {
        var license = await GetOrCreateLicenseAsync(cancellationToken);
        return new LicenseResponse
        {
            Data = Map(license)
        };
    }

    public async Task<HardwareIdResponse> GetHardwareIdAsync(CancellationToken cancellationToken = default)
    {
        var license = await GetOrCreateLicenseAsync(cancellationToken);
        return new HardwareIdResponse
        {
            HardwareId = license.HardwareId
        };
    }

    public ConnectResponse GetConnectionStatus()
    {
        return new ConnectResponse
        {
            Connected = true
        };
    }

    public IReadOnlyList<string> GetCountries()
    {
        return new[] { "Portugal", "Angola" };
    }

    public async Task<ActivateLicenseResponseDto> ActivateAsync(ActivateLicenseRequest request, CancellationToken cancellationToken = default)
    {
        var license = await GetOrCreateLicenseAsync(cancellationToken);
        license.IsLicensed = true;
        license.Version = string.IsNullOrWhiteSpace(request.AssemblyVersion) ? _systemVersionService.GetApiVersion() : request.AssemblyVersion;
        license.HardwareId = string.IsNullOrWhiteSpace(request.HardwareId) ? license.HardwareId : request.HardwareId;
        license.Status = 1;
        license.Date = DateTime.UtcNow;
        license.Name = request.Name ?? string.Empty;
        license.Company = request.Company ?? string.Empty;
        license.Nif = request.FiscalNumber ?? string.Empty;
        license.Address = request.Address ?? string.Empty;
        license.Email = request.Email ?? string.Empty;
        license.Phone = request.Phone ?? string.Empty;
        license.IsValid = true;
        license.HasExpired = false;
        if (license.AllUpdateExpirationDate is null)
        {
            license.AllUpdateExpirationDate = DateTime.UtcNow.AddYears(1);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ActivateLicenseResponseDto
        {
            Success = true,
            LicenseData = string.Empty
        };
    }

    private async Task<ApiLicense> GetOrCreateLicenseAsync(CancellationToken cancellationToken)
    {
        var license = await _dbContext.ApiLicenses.SingleOrDefaultAsync(item => item.Id == SingletonLicenseId, cancellationToken);
        if (license is not null)
        {
            return license;
        }

        license = new ApiLicense
        {
            Id = SingletonLicenseId,
            IsLicensed = false,
            Version = _systemVersionService.GetApiVersion(),
            HardwareId = Guid.NewGuid().ToString().ToUpperInvariant(),
            Status = 0,
            Reseller = "LogicPOS",
            IsValid = false,
            HasExpired = false,
            AllNumberOfDevices = 1
        };

        _dbContext.ApiLicenses.Add(license);
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return license;
        }
        catch (DbUpdateException exception) when (exception.InnerException is SqliteException sqliteException && sqliteException.SqliteErrorCode == 19)
        {
            _dbContext.Entry(license).State = EntityState.Detached;
            return await _dbContext.ApiLicenses.SingleAsync(item => item.Id == SingletonLicenseId, cancellationToken);
        }
    }

    private static LicenseDataResponse Map(ApiLicense license)
    {
        return new LicenseDataResponse
        {
            IsLicensed = license.IsLicensed,
            Version = license.Version,
            HardwareId = license.HardwareId,
            Status = license.Status,
            Date = license.Date,
            Name = license.Name,
            Company = license.Company,
            Nif = license.Nif,
            Address = license.Address,
            Email = license.Email,
            Phone = license.Phone,
            Reseller = license.Reseller,
            StocksModule = license.StocksModule,
            AgtFeModule = license.AgtFeModule,
            AllUpdateExpirationDate = license.AllUpdateExpirationDate,
            AllNumberOfDevices = license.AllNumberOfDevices,
            HasExpired = license.HasExpired,
            IsValid = license.IsValid
        };
    }
}
