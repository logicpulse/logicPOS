using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("fin_configurationpricetype")]
public class PriceType : Entity.WithCode.AndOrder.AndDesignation
{
    public int EnumValue { get; set; }

    public async Task<Result> UpdateAsync(UpdatePriceTypeDto dto,
                                          IPriceTypeRepository repository,
                                          CancellationToken cancellationToken = default)
    {
        var updater = new PriceTypeUpdater(this, dto, repository);
        return await updater.UpdateAsync(cancellationToken);
    }

    public static async Task<Result<PriceType>> CreateAsync(CreatePriceTypeDto dto,
                                                            IPriceTypeRepository repository,
                                                            CancellationToken cancellationToken = default)
    {
        var creator = new PriceTypeCreator(dto, repository);
        return await creator.CreateAsync(cancellationToken);
    }

}
