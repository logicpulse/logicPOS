using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("sys_configurationpoledisplay")]
public class PoleDisplay : Entity.WithCode.AndOrder.AndDesignation
{
    public string VendorId { get; set; } = null!;
    public string ProductId { get; set; } = null!;
    public string EndPoint { get; set; } = null!;
    public string CodeTable { get; set; } = null!;
    public string COMPort { get; set; } = null!;
    public uint CharactersPerLine { get; set; }
    public uint GoToStandByInSeconds { get; set; }
    public string? StandByLine1 { get; set; }
    public string? StandByLine2 { get; set; }

    public Task<Result> UpdateAsync(UpdatePoleDisplayDto dto,
                                    IPoleDisplayRepository repository,
                                    CancellationToken cancellationToken = default)
    {
        var updater = new PoleDisplayUpdater(this, dto, repository);
        return updater.UpdateAsync(cancellationToken);
    }

    public static Task<Result<PoleDisplay>> CreateAsync(IPoleDisplayRepository repository,
                                                        CreatePoleDisplayDto dto,
                                                        CancellationToken cancellationToken = default)
    {
        var creator = new PoleDisplayCreator(dto, repository);
        return creator.CreateAsync(cancellationToken);
    }
}
