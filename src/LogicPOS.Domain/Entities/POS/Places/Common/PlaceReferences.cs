using LogicPOS.Domain.Repositories;

namespace LogicPOS.Domain.Entities.Dtos;

public record PlaceReferences(
    IPriceTypeRepository PriceTypeRepository,
    IMovementTypeRepository MovementTypeRepository
)
{
    public async Task<bool> PriceTypeExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await PriceTypeRepository.ExistsAsync(id, cancellationToken);
    }

    public async Task<bool> MovementTypeExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await MovementTypeRepository.ExistsAsync(id, cancellationToken);
    }
}