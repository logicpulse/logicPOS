using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("cfg_configurationunitsize")]
public class SizeUnit : Entity.WithCode.AndOrder.AndDesignation
{

    public async Task<Result> UpdateAsync(UpdateSizeUnitDto dto,
                                          ISizeUnitRepository repository,
                                          CancellationToken cancellationToken = default)
    {
        var updater = new SizeUnitUpdater(this, dto, repository);
        return await updater.UpdateAsync(cancellationToken);
    }

    public static async Task<Result<SizeUnit>> CreateAsync(CreateSizeUnitDto dto,
                                                           ISizeUnitRepository repository,
                                                           CancellationToken cancellationToken = default)
    {
        var creator = new SizeUnitCreator(dto, repository);
        return await creator.CreateAsync(cancellationToken);
    }
}
