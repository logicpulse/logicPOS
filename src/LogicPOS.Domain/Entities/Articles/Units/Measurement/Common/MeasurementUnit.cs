using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("cfg_configurationunitmeasure")]
public class MeasurementUnit : Entity.WithCode.AndOrder.AndDesignation
{
    public string Acronym { get; set; } = null!;

    public async Task<Result> UpdateAsync(UpdateMeasurementUnitDto dto,
                                          IMeasurementUnitRepository repository,
                                          CancellationToken cancellationToken = default)
    {
        var updater = new MeasurementUnitUpdater(this, dto, repository);
        return await updater.UpdateAsync(cancellationToken);
    }

    public static async Task<Result<MeasurementUnit>> CreateAsync(CreateMeasurementUnitDto dto,
                                                                  IMeasurementUnitRepository repository,
                                                                  CancellationToken cancellationToken = default)
    {
        var creator = new MeasurementUnitCreator(dto, repository);
        return await creator.CreateAsync(cancellationToken);
    }
}
