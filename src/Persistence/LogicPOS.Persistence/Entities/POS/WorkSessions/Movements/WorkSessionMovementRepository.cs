using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Entities.POS.WorkSessions.Movements.Common;
using LogicPOS.Domain.Enums;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Entities.POS.WorkSessions.Movements;

public class WorkSessionMovementRepository : IWorkSessionMovementRepository
{
    private readonly LogicPOSDbContext _database;
    private readonly IWorkSessionPeriodRepository _workSessionPeriodRepository;

    public WorkSessionMovementRepository(LogicPOSDbContext database,
        IWorkSessionPeriodRepository workSessionPeriodRepository)
    {
        _database = database;
        _workSessionPeriodRepository = workSessionPeriodRepository;
    }

    public async Task<decimal> GetCashInTerminalAsync(Guid terminalId,
        CancellationToken ct = default)
    {
        var openTerminalSessionId =
            await _workSessionPeriodRepository.GetOpenTerminalSessionIdAsync(terminalId, ct);

        var lastClosingMovementAmount = await _database.WorkSessionMovements!
            .Where(x => x.Period!.CreatedWhere == terminalId && x.Type == WorkSessionMovementType.CashDrawerClose)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => x.Amount)
            .FirstOrDefaultAsync(ct);

        if (openTerminalSessionId is null)
        {
            return lastClosingMovementAmount;
        }

        var sessionMovements = await _database.WorkSessionMovements!
            .Where(x => (x.PeriodId == openTerminalSessionId)).AsNoTracking().ToListAsync(ct);

        decimal nonDocumentCashAmount = sessionMovements.Where(x =>
                x.Type == WorkSessionMovementType.CashDrawerIn || x.Type == WorkSessionMovementType.CashDrawerOut)
            .Sum(x => x.Amount);

        var sessionDocumentsIds = sessionMovements.Where(m => m.DocumentId != null).Select(m => m.DocumentId).ToList();

        var cashDocumentsIds = await _database.DocumentPaymentMethods
            .Where(dpm => sessionDocumentsIds.Contains(dpm.DocumentId) && dpm.PaymentMethod!.Acronym == "NU")
            .Select(dpm => dpm.DocumentId).ToListAsync(ct);
        
        var documentCashAmount = sessionMovements.Where(m => cashDocumentsIds.Contains(m.DocumentId ??Guid.Empty)).Sum(x => x.Amount);

        return lastClosingMovementAmount + nonDocumentCashAmount + documentCashAmount;
    }

   
}