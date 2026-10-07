using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("erp_customertype")]
public class CustomerType : Entity.WithCode.AndOrder.AndDesignation
{

    public async Task<Result> UpdateAsync(UpdateCustomerTypeDto dto,
        ICustomerTypeRepository repository,
        CancellationToken cancellationToken = default)
    {
        var updater = new CustomerTypeUpdater(this, dto, repository);
        return await updater.UpdateAsync(cancellationToken);
    }

    public static async Task<Result<CustomerType>> CreateAsync(CreateCustomerTypeDto dto,
        ICustomerTypeRepository repository,
        CancellationToken cancellationToken = default)
    {
        var creator = new CustomerTypeCreator(dto, repository);
        return await creator.CreateAsync(cancellationToken);
    }
}