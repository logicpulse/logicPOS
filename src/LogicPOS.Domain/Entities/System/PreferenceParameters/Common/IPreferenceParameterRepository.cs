using LogicPOS.Domain.Entities.Common;

namespace LogicPOS.Domain.Repositories;

public interface IPreferenceParameterRepository : IRepository
{
    Task<bool> TokenExistsAsync(string token, CancellationToken ct = default);
    Task<string?> GetValueByTokenAsync(string token, CancellationToken ct = default);
}