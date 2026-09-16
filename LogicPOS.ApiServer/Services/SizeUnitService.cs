using LogicPOS.ApiServer.Data;
using LogicPOS.ApiServer.Data.Entities;
using LogicPOS.ApiServer.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.ApiServer.Services;

public sealed class SizeUnitService
{
    private readonly ApplicationDbContext _dbContext;

    public SizeUnitService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<SizeUnitResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _dbContext.ApiSizeUnits.AsNoTracking().Where(item => !item.IsDeleted).OrderBy(item => item.Order).ToListAsync(cancellationToken);
        return items.Select(Map).ToList();
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ApiSizeUnits.AnyAsync(item => item.Id == id, cancellationToken);
    }

    public async Task<SizeUnitResponse?> GetByIdInternalAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.ApiSizeUnits.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        return item is null ? null : Map(item);
    }

    public async Task<AddEntityIdResponse> CreateAsync(AddSizeUnitRequest request, CancellationToken cancellationToken = default)
    {
        var count = await _dbContext.ApiSizeUnits.CountAsync(cancellationToken);
        var item = new ApiSizeUnit
        {
            Id = Guid.NewGuid(),
            Order = (uint)(count + 1),
            Code = $"SU{count + 1}",
            Designation = request.Designation.Trim(),
            Notes = request.Notes ?? string.Empty,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
        };

        _dbContext.ApiSizeUnits.Add(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new AddEntityIdResponse { Id = item.Id };
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateSizeUnitRequest request, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.ApiSizeUnits.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null)
        {
            return false;
        }

        item.Order = request.Order;
        item.Code = string.IsNullOrWhiteSpace(request.Code) ? item.Code : request.Code;
        item.Designation = request.Designation.Trim();
        item.Notes = request.Notes ?? string.Empty;
        item.IsDeleted = request.IsDeleted;
        item.UpdatedUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.ApiSizeUnits.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null)
        {
            return false;
        }

        _dbContext.ApiSizeUnits.Remove(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    internal static SizeUnitResponse Map(ApiSizeUnit item)
    {
        return new SizeUnitResponse
        {
            Id = item.Id,
            Notes = item.Notes,
            CreatedAt = item.CreatedUtc,
            UpdatedAt = item.UpdatedUtc,
            UpdatedBy = Guid.Empty,
            IsDeleted = item.IsDeleted,
            Order = item.Order,
            Code = item.Code,
            Designation = item.Designation
        };
    }
}
