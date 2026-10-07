using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("cfg_configurationholidays")]
public class Holiday : Entity.WithCode.AndOrder.AndDesignation
{
    public string? Description { get; set; }
    public uint Year { get; set; }
    public uint Month { get; set; }
    public uint Day { get; set; }
    public bool Fixed { get; set; }

    public Task<Result> UpdateAsync(UpdateHolidayDto dto,
                                    IHolidayRepository repository,
                                    CancellationToken cancellationToken = default)
    {
        var updater = new HolidayUpdater(this, dto, repository);
        return updater.UpdateAsync(cancellationToken);
    }

    public static Task<Result<Holiday>> CreateAsync(IHolidayRepository repository,
                                                    CreateHolidayDto dto,
                                                    CancellationToken cancellationToken = default)
    {
        var creator = new HolidayCreator(dto, repository);
        return creator.CreateAsync(cancellationToken);
    }

}
