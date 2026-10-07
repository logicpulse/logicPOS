using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("fin_documentfinanceyears")]
public class FiscalYear : Entity.WithCode.AndOrder.AndDesignation
{
    public int Year { get; set; }
    public string Acronym {  get; set; } = null!;
    public bool SeriesForEachTerminal { get; set; }

    public bool IsClosed() => IsDeleted == true;

    public static async Task<Result<FiscalYear>> CreateAsync(CreateFiscalYearDto dto,
                                                             IFiscalYearRepository repository,
                                                             CancellationToken cancellationToken = default)
    {
        var creator = new FiscalYearCreator(dto, repository);
        return await creator.CreateAsync(cancellationToken);
    }
    
}
