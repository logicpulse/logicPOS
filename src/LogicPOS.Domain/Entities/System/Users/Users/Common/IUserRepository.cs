using LogicPOS.Domain.Entities.Common;

namespace LogicPOS.Domain.Repositories;

public interface IUserRepository : IRepository.IWithName {
    public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
    public Task<bool> LoginExistsAsync(string login, CancellationToken ct = default);
}