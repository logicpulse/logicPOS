using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("pos_configurationplacemovementtype")]
public class MovementType : Entity.WithCode.AndOrder.AndDesignation
{
    public bool VatDirectSelling { get; set; }
  
    public async Task<Result> UpdateAsync(UpdateMovementTypeDto dto,
                                          IMovementTypeRepository repository,
                                          CancellationToken cancellationToken = default)
    {
        var updater = new MovementTypeUpdater(this, dto, repository);
        return await updater.UpdateAsync(cancellationToken);
    }

    public static async Task<Result<MovementType>> CreateAsync(CreateMovementTypeDto dto,
                                                               IMovementTypeRepository repository,
                                                               CancellationToken cancellationToken = default)
    {
        var creator = new MovementTypeCreator(dto, repository);
        return await creator.CreateAsync(cancellationToken);
    }
}
