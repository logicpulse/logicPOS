using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Core;

/// <summary>
/// Resolves the company / fiscal country. Prefer the configured company country, then Portugal (PT).
/// </summary>
public static class CompanyCountryResolver
{
    public static async Task<(Guid Id, string Designation)?> ResolveAsync(
        LogicPOSDbContext database,
        CancellationToken cancellationToken = default)
    {
        var storedId = await database.PreferenceParameters.AsNoTracking()
            .Where(item => item.IsDeleted == false && item.Token == "COMPANY_COUNTRY_OID")
            .Select(item => item.Value)
            .FirstOrDefaultAsync(cancellationToken);
        if (Guid.TryParse(storedId, out var companyId) && companyId != Guid.Empty)
        {
            var company = await database.Countries.AsNoTracking()
                .Where(item => item.Id == companyId && item.IsDeleted == false)
                .Select(item => new { item.Id, item.Designation })
                .FirstOrDefaultAsync(cancellationToken);
            if (company is not null)
            {
                return (company.Id, company.Designation);
            }
        }

        var portugal = await database.Countries.AsNoTracking()
            .Where(item => item.IsDeleted == false
                && (item.Code2 == "PT" || item.Designation == "Portugal"))
            .OrderBy(item => item.Code2 == "PT" ? 0 : 1)
            .Select(item => new { item.Id, item.Designation })
            .FirstOrDefaultAsync(cancellationToken);
        if (portugal is not null)
        {
            return (portugal.Id, portugal.Designation);
        }

        var fallback = await database.Countries.AsNoTracking()
            .Where(item => item.IsDeleted == false)
            .OrderBy(item => item.Designation)
            .Select(item => new { item.Id, item.Designation })
            .FirstOrDefaultAsync(cancellationToken);
        return fallback is null ? null : (fallback.Id, fallback.Designation);
    }
}
