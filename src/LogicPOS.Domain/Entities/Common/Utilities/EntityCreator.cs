using LogicPOS.Domain.Errors;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Common.Utilities;

public abstract class EntityCreator<TEntity> where TEntity : Entity.Entity
{
    protected readonly TEntity _entity;

    protected EntityCreator(TEntity entity)
    {
        _entity = entity;
    }

    public virtual async Task<Result<TEntity>> CreateAsync(CancellationToken ct = default)
    {
        var existenceResult = await CheckForExistenceAsync(ct);
        if (existenceResult.IsFailure)
        {
            return existenceResult.ToGenericFailure<TEntity>();
        }

        await AssignDataAsync(ct);

        var hasNoConflictsResult = await CheckForConflictsAsync(ct);

        if (hasNoConflictsResult.IsFailure)
        {
            return hasNoConflictsResult.ToGenericFailure<TEntity>();
        }

        return _entity;
    }

    protected abstract Task<Result> CheckForConflictsAsync(CancellationToken ct = default);

    protected async Task<Result> CheckForDesignationConflictAsync(IRepository repository,
        string designation,
        CancellationToken cancellationToken = default)
    {
        if (await (repository as IRepository.IWithDesignation)!.DesignationExistsAsync(designation, cancellationToken))
        {
            return Result.Failure(Error.DesignationExistsError);
        }

        return Result.Success();
    }

    protected abstract Task<Result> CheckForExistenceAsync(CancellationToken ct = default);

    protected abstract Task AssignDataAsync(CancellationToken ct = default);
}