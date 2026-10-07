using LogicPOS.Domain.Repositories;

namespace LogicPOS.Domain.Entities;

public record TerminalReferences(IPrinterRepository PrinterRepository,
                                 IPoleDisplayRepository PoleDisplayRepository,
                                 IWeighingMachineRepository WeighingMachineRepository,
                                 IPlaceRepository PlaceRepository,
                                 IInputReaderRepository InputReaderRepository)
{
    public async Task<bool> PrinterExistsAsync(
       Guid id,
       CancellationToken cancellationToken = default)
    {
        return await PrinterRepository.ExistsAsync(id, cancellationToken);
    }

    public async Task<bool> PoleDisplayExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await PoleDisplayRepository.ExistsAsync(id, cancellationToken);
    }

    public async Task<bool> WeighingMachineExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await WeighingMachineRepository.ExistsAsync(id, cancellationToken);
    }

    public async Task<bool> PlaceExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await PlaceRepository.ExistsAsync(id, cancellationToken);
    }

    public async Task<bool> InputReaderExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await InputReaderRepository.ExistsAsync(id, cancellationToken);
    }
}