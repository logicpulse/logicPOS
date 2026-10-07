using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Enums;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;


namespace LogicPOS.Domain.Entities;

//[Table("pos_configurationplace")]
public class Place : Entity.WithCode.AndOrder.AndDesignation
{
    public PriceType? PriceType { get; set; }
    public Guid PriceTypeId { get; set; }

    public MovementType? MovementType { get; set; }
    public Guid? MovementTypeId { get; set; }

    public string? ButtonImage { get; set; }
    public string? TypeSubtotal { get; set; }
    public string? AccountType { get; set; }
    public OrderPrintMode? OrderPrintMode { get; set; }

    public async Task<Result> UpdateAsync(UpdatePlaceDto dto,
                                          IPlaceRepository repository,
                                          PlaceReferences dependencies,
                                          CancellationToken cancellationToken = default)
    {
        var updater = new PlaceUpdater(this, dto, repository, dependencies);
        return await updater.UpdateAsync(cancellationToken);
    }

    public static async Task<Result<Place>> CreateAsync(CreatePlaceDto dto,
                                                        IPlaceRepository repository,
                                                        PlaceReferences dependencies,
                                                        CancellationToken cancellationToken = default)
    {
        var creator = new PlaceCreator(dto, repository, dependencies);
        return await creator.CreateAsync(cancellationToken);
    }

}
