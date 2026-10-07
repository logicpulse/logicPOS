using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("cfg_configurationcountry")]
public class Country : Entity.WithCode.AndOrder.AndDesignation
{
    public Country() { }
    public string Code2 { get; set; } = null!;
    public string Code3 { get; set; } = null!;
    public string Capital { get; set; } = null!;
    public string TLD { get; set; } = null!;
    public string Currency { get; set; } = null!;
    public string CurrencyCode { get; set; } = null!;
    public string FiscalNumberRegex { get; set; } = null!;
    public string ZipCodeRegex { get; set; } = null!;

    public Task<Result> UpdateAsync(UpdateCountryDto dto,
                                    ICountryRepository repository,
                                    CancellationToken cancellationToken = default)
    {
        var updater = new CountryUpdater(this, dto, repository);
        return updater.UpdateAsync(cancellationToken);
    }

    public static Task<Result<Country>> CreateAsync(ICountryRepository repository,
                                                    CreateCountryDto dto,
                                                    CancellationToken cancellationToken = default)
    {
        var creator = new CountryCreator(repository, dto);
        return creator.CreateAsync(cancellationToken);
    }

}
