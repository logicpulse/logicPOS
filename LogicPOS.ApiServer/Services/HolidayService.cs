using LogicPOS.ApiServer.Data;
using LogicPOS.ApiServer.Data.Entities;
using LogicPOS.ApiServer.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.ApiServer.Services;

public sealed class HolidayService
{
    private readonly ApplicationDbContext _dbContext;

    public HolidayService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<HolidayResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var holidays = await _dbContext.ApiHolidays
            .AsNoTracking()
            .Where(item => !item.IsDeleted)
            .OrderBy(item => item.Order)
            .ToListAsync(cancellationToken);

        return holidays.Select(Map).ToList();
    }

    public async Task<AddEntityIdResponse> CreateAsync(AddHolidayRequest request, CancellationToken cancellationToken = default)
    {
        var count = await _dbContext.ApiHolidays.CountAsync(cancellationToken);
        var holiday = new ApiHoliday
        {
            Id = Guid.NewGuid(),
            Order = (uint)(count + 1),
            Code = $"H{count + 1}",
            Designation = request.Designation.Trim(),
            Description = request.Description ?? string.Empty,
            Day = request.Day,
            Month = request.Month,
            Year = request.Year,
            Notes = request.Notes ?? string.Empty,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
        };

        _dbContext.ApiHolidays.Add(holiday);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AddEntityIdResponse { Id = holiday.Id };
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateHolidayRequest request, CancellationToken cancellationToken = default)
    {
        var holiday = await _dbContext.ApiHolidays.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (holiday is null)
        {
            return false;
        }

        holiday.Order = request.Order;
        holiday.Code = string.IsNullOrWhiteSpace(request.Code) ? holiday.Code : request.Code;
        holiday.Designation = request.Designation.Trim();
        holiday.Description = request.Description ?? string.Empty;
        holiday.Day = request.Day;
        holiday.Month = request.Month;
        holiday.Year = request.Year;
        holiday.Notes = request.Notes ?? string.Empty;
        holiday.IsFixed = request.Fixed;
        holiday.IsDeleted = request.IsDeleted;
        holiday.UpdatedUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var holiday = await _dbContext.ApiHolidays.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (holiday is null)
        {
            return false;
        }

        _dbContext.ApiHolidays.Remove(holiday);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static HolidayResponse Map(ApiHoliday holiday)
    {
        return new HolidayResponse
        {
            Id = holiday.Id,
            Notes = holiday.Notes,
            CreatedAt = holiday.CreatedUtc,
            UpdatedAt = holiday.UpdatedUtc,
            UpdatedBy = Guid.Empty,
            IsDeleted = holiday.IsDeleted,
            Order = holiday.Order,
            Code = holiday.Code,
            Designation = holiday.Designation,
            Description = holiday.Description,
            Day = holiday.Day,
            Month = holiday.Month,
            Year = holiday.Year,
            Fixed = holiday.IsFixed
        };
    }
}
