using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;


//[Table("fin_warehouse")]
public class Warehouse : Entity.WithCode.AndOrder.AndDesignation
{
    public bool IsDefault { get; set; }
    public IList<WarehouseLocation>? Locations { get; set; }

    public static Task<Result<Warehouse>> CreateAsync(IWarehouseRepository repository,
                                                      CreateWarehouseDto dto,
                                                      CancellationToken cancellationToken = default)
    {
        var creator = new WarehouseCreator(repository, dto);
        return creator.CreateAsync(cancellationToken);
    }

    public Task<Result> UpdateAsync(IWarehouseRepository repository,
                                    UpdateWarehouseDto dto,
                                    CancellationToken cancellationToken = default)
    {
        var Updater = new WarehouseUpdater(this, repository, dto);
        return Updater.UpdateAsync(cancellationToken);
    }
}
