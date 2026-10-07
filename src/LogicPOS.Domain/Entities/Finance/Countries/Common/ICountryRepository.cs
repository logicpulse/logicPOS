using LogicPOS.Domain.Entities.Common;

namespace LogicPOS.Domain.Repositories;

public interface ICountryRepository : IRepository.IWithDesignation
{
    Task<bool> Code2ExistsAsync(string code2, CancellationToken cancellationToken = default);
    Task<bool> Code3ExistsAsync(string code3, CancellationToken cancellationToken = default);
}