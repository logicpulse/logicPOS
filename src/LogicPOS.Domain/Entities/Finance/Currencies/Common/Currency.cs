using System.ComponentModel.DataAnnotations.Schema;
using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("cfg_configurationcurrency")]
public class Currency : Entity.WithCode.AndOrder.AndDesignation
{
    public string Acronym { get; set; } = null!;
    public string Symbol { get; set; } = null!;
    public string Entity { get; set; } = null!;
    
    [Column(TypeName = "decimal(18,6)")]
    public decimal ExchangeRate { get; set; }

    public Task<Result> UpdateAsync(UpdateCurrencyDto dto,
                                    ICurrencyRepository repository,
                                    CancellationToken cancellationToken = default)
    {
        var updater = new CurrencyUpdater(this, dto, repository);
        return updater.UpdateAsync(cancellationToken);
    }

    public static Task<Result<Currency>> CreateAsync(ICurrencyRepository repository,
                                                     CreateCurrencyDto dto,
                                                     CancellationToken cancellationToken = default)
    {
        var creator = new CurrencyCreator(dto,repository);
        return creator.CreateAsync(cancellationToken);
    }
}
