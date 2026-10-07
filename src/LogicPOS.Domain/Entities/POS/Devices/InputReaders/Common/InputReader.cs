using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("sys_configurationinputreader")]
public class InputReader : Entity.WithCode.AndOrder.AndDesignation
{
    public string ReaderSizes { get; set; } = null!;

     public Task<Result> UpdateAsync(UpdateInputReaderDto dto,
                                     IInputReaderRepository repository,
                                     CancellationToken cancellationToken = default)
    {
        var updater = new InputReaderUpdater(this, dto, repository);
        return updater.UpdateAsync(cancellationToken);
    }

    public static Task<Result<InputReader>> CreateAsync(IInputReaderRepository repository,
                                                        CreateInputReaderDto dto,
                                                        CancellationToken cancellationToken = default)
    {
        var creator = new InputReaderCreator(dto, repository);
        return creator.CreateAsync(cancellationToken);
    }

    
}
