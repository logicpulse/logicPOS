using LogicPOS.Domain.Repositories;

namespace LogicPOS.Domain.Entities;

public record CustomerReferences(IDiscountGroupRepository DiscountGroupRepository,
                                 ICustomerTypeRepository CustomerTypeRepository,
                                 IPriceTypeRepository PriceTypeRepository,
                                 ICountryRepository CountryRepository)
{
    public async Task<bool> DiscountGroupExistsAsync(Guid id,
                                                     CancellationToken cancellationToken = default)
    {
        return await DiscountGroupRepository.ExistsAsync(id, cancellationToken);
    }

    public async Task<bool> TypeExistsAsync(Guid id,
                                            CancellationToken cancellationToken = default)
    {
        return await CustomerTypeRepository.ExistsAsync(id, cancellationToken);
    }

    public async Task<bool> PriceTypeExistsAsync(Guid id,
                                                 CancellationToken cancellationToken = default)
    {
        return await PriceTypeRepository.ExistsAsync(id, cancellationToken);
    }

    public async Task<bool> CountryExistsAsync(Guid id,
                                               CancellationToken cancellationToken = default)
    {
        return await CountryRepository.ExistsAsync(id, cancellationToken);
    }


}