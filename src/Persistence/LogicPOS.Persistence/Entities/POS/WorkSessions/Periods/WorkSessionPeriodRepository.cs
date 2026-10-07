using LogicPOS.Domain.Enums;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Entities.POS.WorkSessions.Periods;

public class WorkSessionPeriodRepository : IWorkSessionPeriodRepository
{
    private readonly LogicPOSDbContext _database;
    public WorkSessionPeriodRepository(LogicPOSDbContext database)
    {
        _database = database;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _database.WorkSessionPeriods!.AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Guid?> GetOpenDayIdAsync(CancellationToken cancellationToken = default)
    {
        var workSessionPeriod = await _database.WorkSessionPeriods!.FirstOrDefaultAsync(x => x.Type == WorkSessionPeriodType.Day && 
                                                                                        x.Status == WorkSessionPeriodStatus.Open, 
                                                                                        cancellationToken);
        return workSessionPeriod?.Id;
    }

    public async Task<Guid?> GetOpenTerminalSessionIdAsync(Guid terminalId, CancellationToken cancellationToken = default)
    {
        var workSessionPeriod = await _database.WorkSessionPeriods!.AsNoTracking().FirstOrDefaultAsync(x => x.Type == WorkSessionPeriodType.Terminal && 
                                                                                        x.Status == WorkSessionPeriodStatus.Open && 
                                                                                        x.CreatedWhere == terminalId, 
                                                                                        cancellationToken);
        return workSessionPeriod?.Id;
    }

    public async Task<bool> HasAnyOpenTerminalAsync(CancellationToken cancellationToken = default)
    {
        return await _database.WorkSessionPeriods!.AnyAsync(x => x.Type == WorkSessionPeriodType.Terminal &&
                                                            x.Status == WorkSessionPeriodStatus.Open, 
                                                            cancellationToken);
    }

    public async Task<bool> HasAnyOpenDayAsync(CancellationToken cancellationToken = default)
    {
        return await _database.WorkSessionPeriods!.AnyAsync(x => x.Type == WorkSessionPeriodType.Day && 
                                                                 x.Status == WorkSessionPeriodStatus.Open, 
                                                                 cancellationToken);
    }


    public async Task<bool> TerminalHasAnyOpenSessionAsync(Guid terminalId, CancellationToken cancellationToken = default)
    {
        return await _database.WorkSessionPeriods!.AnyAsync(x => x.Type == WorkSessionPeriodType.Terminal && 
                                                                 x.Status == WorkSessionPeriodStatus.Open && 
                                                                 x.CreatedWhere == terminalId, 
                                                                 cancellationToken);
    }
}