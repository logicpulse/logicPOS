using LogicPOS.Domain.Entities.Common;

namespace LogicPOS.Domain.Repositories;

public interface IPaymentMethodRepository : IRepository.IWithDesignation
{
    Task<string> GetAcronymAsync(Guid id, CancellationToken ct = default);
    public Task<Guid?> GetCustomerCardIdAsync(CancellationToken ct = default);
}