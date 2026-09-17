using LogicPOS.ApiServer.Data;
using LogicPOS.ApiServer.Data.Entities;
using LogicPOS.ApiServer.DTOs;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.ApiServer.Services;

public sealed class CompanyService
{
    private const int SingletonCompanyId = 1;
    private readonly ApplicationDbContext _dbContext;

    public CompanyService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CompanyResponse> GetAsync(CancellationToken cancellationToken = default)
    {
        var company = await GetOrCreateAsync(cancellationToken);
        return Map(company);
    }

    public async Task UpdateAsync(UpdateCompanyDetailsRequest request, CancellationToken cancellationToken = default)
    {
        var company = await GetOrCreateAsync(cancellationToken);
        company.Name = request.CompanyName ?? string.Empty;
        company.BusinessName = request.BusinessName ?? string.Empty;
        company.CommercialName = request.BusinessName ?? request.CompanyName ?? string.Empty;
        company.FiscalNumber = request.FiscalNumber ?? string.Empty;
        company.CountryCode2 = string.IsNullOrWhiteSpace(request.CountryCode2) ? "PT" : request.CountryCode2;
        company.TaxEntity = request.TaxEntity ?? string.Empty;
        company.City = request.City ?? string.Empty;
        company.Address = request.Address ?? string.Empty;
        company.StockCapital = request.StockCapital ?? string.Empty;
        company.PostalCode = request.PostalCode ?? string.Empty;
        company.Email = request.Email ?? string.Empty;
        company.Phone = request.Phone ?? string.Empty;
        company.MobilePhone = request.MobilePhone ?? string.Empty;
        company.Website = request.Website ?? string.Empty;
        company.Fax = request.Fax ?? string.Empty;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<ApiCompanyInfo> GetOrCreateAsync(CancellationToken cancellationToken)
    {
        var company = await _dbContext.ApiCompanyInfos.SingleOrDefaultAsync(item => item.Id == SingletonCompanyId, cancellationToken);
        if (company is not null)
        {
            return company;
        }

        company = new ApiCompanyInfo { Id = SingletonCompanyId };
        _dbContext.ApiCompanyInfos.Add(company);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return company;
        }
        catch (DbUpdateException exception) when (exception.InnerException is SqliteException sqliteException && sqliteException.SqliteErrorCode == 19)
        {
            _dbContext.Entry(company).State = EntityState.Detached;
            return await _dbContext.ApiCompanyInfos.SingleAsync(item => item.Id == SingletonCompanyId, cancellationToken);
        }
    }

    private static CompanyResponse Map(ApiCompanyInfo company)
    {
        return new CompanyResponse
        {
            Name = company.Name,
            BusinessName = company.BusinessName,
            CommercialName = company.CommercialName,
            LogoPng = company.LogoPng,
            LogoBmp = company.LogoBmp,
            Address = company.Address,
            City = company.City,
            PostalCode = company.PostalCode,
            CountryCode2 = company.CountryCode2,
            Phone = company.Phone,
            MobilePhone = company.MobilePhone,
            Email = company.Email,
            Website = company.Website,
            FiscalNumber = company.FiscalNumber,
            StockCapital = company.StockCapital,
            DocumentFinalLine1 = company.DocumentFinalLine1,
            DocumentFinalLine2 = company.DocumentFinalLine2,
            TaxEntity = company.TaxEntity,
            Fax = company.Fax,
            TicketFinalLine1 = company.TicketFinalLine1,
            TicketFinalLine2 = company.TicketFinalLine2,
            CurrencyCode = company.CurrencyCode,
            AgtLogo = company.AgtLogo
        };
    }
}
