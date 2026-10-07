using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("erp_customerdiscountgroup")]
public class DiscountGroup : Entity.WithCode.AndOrder.AndDesignation
{
    public async Task<Result> UpdateAsync(UpdateDiscountGroupDto dto,
                                          IDiscountGroupRepository repository,
                                          CancellationToken cancellationToken = default)
    {
        var updater = new DiscountGroupUpdater(this, dto, repository);
        return await updater.UpdateAsync(cancellationToken);
    }

    public static async Task<Result<DiscountGroup>> CreateAsync(CreateDiscountGroupDto dto,
                                                                IDiscountGroupRepository repository,
                                                                CancellationToken cancellationToken = default)
    {
        var creator = new DiscountGroupCreator(dto, repository);
        return await creator.CreateAsync(cancellationToken);
    }
}
