using LogicPOS.Domain.Entities.Common;

namespace LogicPOS.Domain.Repositories;

public interface IFiscalYearRepository : IRepository.IWithDesignation
{
    Task<bool> AcronymExistsAsync(string acronym, CancellationToken ct);
}