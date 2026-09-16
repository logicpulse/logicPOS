using LogicPOS.ApiServer.Data;
using LogicPOS.ApiServer.Data.Entities;
using LogicPOS.ApiServer.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.ApiServer.Services;

public sealed class FiscalYearService
{
    private readonly ApplicationDbContext _dbContext;

    public FiscalYearService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<FiscalYearResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _dbContext.ApiFiscalYears.AsNoTracking().Where(item => !item.IsDeleted).OrderByDescending(item => item.Year).ToListAsync(cancellationToken);
        return items.Select(Map).ToList();
    }

    public async Task<FiscalYearResponse?> GetCurrentAsync(CancellationToken cancellationToken = default)
    {
        var currentYear = DateTime.UtcNow.Year;
        var item = await _dbContext.ApiFiscalYears.AsNoTracking()
            .Where(x => !x.IsDeleted && !x.IsClosed)
            .OrderByDescending(x => x.Year == currentYear)
            .ThenByDescending(x => x.Year)
            .FirstOrDefaultAsync(cancellationToken);

        return item is null ? null : Map(item);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ApiFiscalYears.AnyAsync(item => item.Id == id && !item.IsClosed, cancellationToken);
    }

    public async Task<FiscalYearResponse?> GetByIdInternalAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.ApiFiscalYears.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        return item is null ? null : Map(item);
    }

    public async Task<AddEntityIdResponse> CreateAsync(AddFiscalYearRequest request, CancellationToken cancellationToken = default)
    {
        var count = await _dbContext.ApiFiscalYears.CountAsync(cancellationToken);
        var item = new ApiFiscalYear
        {
            Id = Guid.NewGuid(),
            Order = (uint)(count + 1),
            Code = $"FY{request.Year}",
            Designation = request.Designation.Trim(),
            Year = request.Year,
            Acronym = request.Acronym,
            SeriesForEachTerminal = request.SeriesForEachTerminal,
            Notes = request.Notes ?? string.Empty,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
        };

        _dbContext.ApiFiscalYears.Add(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new AddEntityIdResponse { Id = item.Id };
    }

    public async Task<bool> CloseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.ApiFiscalYears.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null)
        {
            return false;
        }

        item.IsClosed = true;
        item.UpdatedUtc = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    internal static FiscalYearResponse Map(ApiFiscalYear item)
    {
        return new FiscalYearResponse
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
            Year = item.Year,
            Acronym = item.Acronym,
            SeriesForEachTerminal = item.SeriesForEachTerminal
        };
    }
}
