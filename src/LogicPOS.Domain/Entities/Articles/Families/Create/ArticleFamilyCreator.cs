using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;


internal class ArticleFamilyCreator : EntityCreator<ArticleFamily>
{
    private readonly CreateArticleFamilyDto _dto;
    private readonly IArticleFamilyRepository _repository;
    private readonly ArticleFamilyReferences _dependencies;


    public ArticleFamilyCreator(CreateArticleFamilyDto dto,
                                IArticleFamilyRepository repository,
                                ArticleFamilyReferences dependencies) : 
        base(new())
    {
        _dto = dto;
        _repository = repository;
        _dependencies = dependencies;
    }

    protected override async Task AssignDataAsync(CancellationToken ct = default)
    {
        _entity.Designation = _dto.Designation;
        _entity.Code = _dto.Imported ? "?" : await _repository.GetNextCodeAsync(ct);
        _entity.Order = _dto.Imported ? 0:  await _repository.GetNextOrderAsync(ct);
        _entity.Button = _dto.Button ?? new();
        _entity.CommissionGroupId = _dto.CommissionGroupId;
        _entity.DiscountGroupId = _dto.DiscountGroupId;
        _entity.Notes = _dto.Notes;
        _entity.IsDeleted = _dto.IsDeleted ?? false;
    }

    protected override async Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        if (_dto.Imported)
        {
            return Result.Success();
        }
        return await CheckForDesignationConflictAsync(_repository, _dto.Designation, ct);
    }

    protected override async Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        if (_dto.CommissionGroupId.HasValue &&
            !await _dependencies.CommissionGroupExistsAsync(_dto.CommissionGroupId.Value,
                                                            ct))
        {
            return Result.NotFound(nameof(CommissionGroup), _dto.CommissionGroupId.ToString());
        }

        if (_dto.DiscountGroupId.HasValue &&
            !await _dependencies.DiscountGroupExistsAsync(_dto.DiscountGroupId.Value,
                                                          ct))
        {
            return Result.NotFound(nameof(DiscountGroup), _dto.DiscountGroupId.ToString());
        }
        
        return Result.Success();
    }
}