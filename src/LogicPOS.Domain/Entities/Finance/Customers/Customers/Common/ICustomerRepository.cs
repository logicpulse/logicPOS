using LogicPOS.Domain.Entities.Common;
using LogicPOS.Domain.ValueObjects;

namespace LogicPOS.Domain.Repositories;

public interface ICustomerRepository : IRepository.IWithName {
   Task<bool> FiscalNumberExistsAsync(string fiscalNumber, CancellationToken ct = default);
   Task<Guid> CreateCustomerFromDocumentAsync(DocumentCustomer documentCustomer, CancellationToken ct = default);
   Task<DocumentCustomer> CreateDocumentCustomerAsync(Guid customerId, CancellationToken ct = default);
   Task<(string Name, string FiscalNumber)> GetCustomerNameAndFiscalNumberAsync(Guid customerId, CancellationToken ct = default);
   Task<bool> CardNumberExistsAsync(string cardNumber, CancellationToken ct = default);
   Task<Guid?> GetFinalConsumerIdAsync(CancellationToken ct = default);
   Task<bool> IsFinalConsumerAsync(Guid customerId, CancellationToken ct = default);

}