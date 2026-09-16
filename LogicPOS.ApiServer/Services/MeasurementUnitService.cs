using LogicPOS.ApiServer.Data;
using LogicPOS.ApiServer.Data.Entities;
using LogicPOS.ApiServer.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.ApiServer.Services;

public sealed class MeasurementUnitService
{
    private readonly ApplicationDbContext _dbContext;

    public MeasurementUnitService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<MeasurementUnitResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _dbContext.ApiMeasurementUnits.AsNoTracking().Where(item => !item.IsDeleted).OrderBy(item => item.Order).ToListAsync(cancellationToken);
        return items.Select(Map).ToList();
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ApiMeasurementUnits.AnyAsync(item => item.Id == id, cancellationToken);
    }

    public async Task<MeasurementUnitResponse?> GetByIdInternalAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.ApiMeasurementUnits.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        return item is null ? null : Map(item);
    }

    public async Task<AddEntityIdResponse> CreateAsync(AddMeasurementUnitRequest request, CancellationToken cancellationToken = default)
    {
        var count = await _dbContext.ApiMeasurementUnits.CountAsync(cancellationToken);
        var item = new ApiMeasurementUnit
        {
            Id = Guid.NewGuid(),
            Order = (uint)(count + 1),
            Code = $"MU{count + 1}",
            Designation = request.Designation.Trim(),
            Acronym = request.Acronym ?? string.Empty,
            Notes = request.Notes ?? string.Empty,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
        };

        _dbContext.ApiMeasurementUnits.Add(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new AddEntityIdResponse { Id = item.Id };
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateMeasurementUnitRequest request, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.ApiMeasurementUnits.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null)
        {
            return false;
        }

        item.Order = request.Order;
        item.Code = string.IsNullOrWhiteSpace(request.Code) ? item.Code : request.Code;
        item.Designation = request.Designation.Trim();
        item.Acronym = request.Acronym ?? string.Empty;
        item.Notes = request.Notes ?? string.Empty;
        item.IsDeleted = request.IsDeleted;
        item.UpdatedUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.ApiMeasurementUnits.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null)
        {
            return false;
        }

        _dbContext.ApiMeasurementUnits.Remove(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    internal static MeasurementUnitResponse Map(ApiMeasurementUnit item)
    {
        return new MeasurementUnitResponse
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
            Acronym = item.Acronym
        };
    }
}
