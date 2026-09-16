using LogicPOS.ApiServer.Data;
using LogicPOS.ApiServer.Data.Entities;
using LogicPOS.ApiServer.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.ApiServer.Services;

public sealed class VatRateService
{
    private readonly ApplicationDbContext _dbContext;

    public VatRateService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<VatRateResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _dbContext.ApiVatRates.AsNoTracking().Where(item => !item.IsDeleted).OrderBy(item => item.Order).ToListAsync(cancellationToken);
        return items.Select(Map).ToList();
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ApiVatRates.AnyAsync(item => item.Id == id, cancellationToken);
    }

    public async Task<VatRateResponse?> GetByIdInternalAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.ApiVatRates.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        return item is null ? null : Map(item);
    }

    public async Task<AddEntityIdResponse> CreateAsync(AddVatRateRequest request, CancellationToken cancellationToken = default)
    {
        var count = await _dbContext.ApiVatRates.CountAsync(cancellationToken);
        var item = new ApiVatRate
        {
            Id = Guid.NewGuid(),
            Order = (uint)(count + 1),
            Code = $"VAT{count + 1}",
            Designation = request.Designation.Trim(),
            Value = request.Value,
            ReasonCode = request.ReasonCode ?? string.Empty,
            TaxType = request.TaxType ?? string.Empty,
            TaxCode = request.TaxCode ?? string.Empty,
            CountryRegion = request.CountryRegionCode ?? string.Empty,
            ExpirationDate = request.ExpirationDate,
            Description = request.Description ?? string.Empty,
            Notes = request.Notes ?? string.Empty,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
        };

        _dbContext.ApiVatRates.Add(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new AddEntityIdResponse { Id = item.Id };
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateVatRateRequest request, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.ApiVatRates.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null)
        {
            return false;
        }

        item.Order = request.Order;
        item.Code = string.IsNullOrWhiteSpace(request.Code) ? item.Code : request.Code;
        item.Designation = request.Designation.Trim();
        item.Value = request.Value;
        item.ReasonCode = request.ReasonCode ?? string.Empty;
        item.TaxType = request.TaxType ?? string.Empty;
        item.TaxCode = request.TaxCode ?? string.Empty;
        item.CountryRegion = request.CountryRegionCode ?? string.Empty;
        item.ExpirationDate = request.ExpirationDate;
        item.Description = request.Description ?? string.Empty;
        item.Notes = request.Notes ?? string.Empty;
        item.IsDeleted = request.IsDeleted;
        item.UpdatedUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.ApiVatRates.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null)
        {
            return false;
        }

        _dbContext.ApiVatRates.Remove(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    internal static VatRateResponse Map(ApiVatRate item)
    {
        return new VatRateResponse
        {
            Id = item.Id,
            Notes = item.Notes,
            CreatedAt = item.CreatedUtc,
            UpdatedAt = item.UpdatedUtc,
            UpdatedBy = Guid.Empty,
            IsDeleted = item.IsDeleted,
            Order = item.Order,
            Code = item.Code,
            Designation = item.Designation,
            Value = item.Value,
            ReasonCode = item.ReasonCode,
            TaxType = item.TaxType,
            TaxCode = item.TaxCode,
            CountryRegion = item.CountryRegion,
            ExpirationDate = item.ExpirationDate,
            Description = item.Description
        };
    }
}
