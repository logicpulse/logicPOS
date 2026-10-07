using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Errors;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Common.Utilities;

public abstract class EntityUpdater<TEntity> where TEntity : Entity.Entity
{
    protected readonly TEntity _entity;
    protected readonly IRepository _repository;

    protected EntityUpdater(
        TEntity entity,
        IRepository repository
        )
    {
        _entity = entity;
        _repository = repository;
    }

    public virtual async Task<Result> UpdateAsync(CancellationToken cancellationToken = default)
    {
        Result updateResult = await CheckForExistenceAsync(cancellationToken);
        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        updateResult = await CheckForConflictsAsync(cancellationToken);
        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        SetNewData();

        return Result.Success();
    }

    protected abstract Task<Result> CheckForConflictsAsync(CancellationToken ct = default);

    protected async Task<Result> CheckForCodeAndDesignationConflictsAsync(
        string newCode,
        string newDesignation,
        CancellationToken cancellationToken = default)
    {
        var codeResult = await CheckForCodeConflictAsync(newCode, cancellationToken);
        if (codeResult.IsFailure)
        {
            return codeResult;
        }

        var designationResult = await CheckForDesignationConflictAsync(newDesignation, cancellationToken);
        if (designationResult.IsFailure)
        {
            return designationResult;
        }

        return Result.Success();
    }
    
    protected virtual async Task<Result> CheckForDesignationConflictAsync(
        string? newDesignation,
        CancellationToken cancellationToken = default)
    {
        var entity = (_entity as IEntityWithDesignation)!;

        if (newDesignation == null || newDesignation == entity.Designation)
        {
            return Result.Success();
        }

        if (await (_repository as IRepository.IWithDesignation)!.DesignationExistsAsync(newDesignation, cancellationToken))
        {
            return Result.Failure(Error.DesignationExistsError);
        }

        return Result.Success();
    }

    protected virtual async Task<Result> CheckForNameConflictAsync(string name,
                                                                   CancellationToken ct = default)
    {
        var entity = (_entity as IEntityWithName)!;

        if (name == entity.Name)
        {
            return Result.Success();
        }

        if (await (_repository as IRepository.IWithName)!.NameExistsAsync(name, ct))
        {
            return Result.Conflict($"Nome '{name}' já existe.");
        }

        return Result.Success();
    }

    protected virtual async Task<Result> CheckForCodeConflictAsync(
       string code,
       CancellationToken cancellationToken = default)
    {
        var entity = (_entity as IEntityWithCode)!;

        if (code == entity.Code)
        {
            return Result.Success();
        }

        if (await _repository.CodeExistsAsync(code, cancellationToken))
        {
            return Result.Conflict($"Código '{code}' já existe.");
        }

        return Result.Success();
    }

    protected abstract Task<Result> CheckForExistenceAsync(CancellationToken ct = default);

    protected abstract void SetNewData();
}
