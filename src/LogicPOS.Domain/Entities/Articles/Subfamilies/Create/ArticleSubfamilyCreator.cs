using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Errors;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class ArticleSubfamilyCreator : EntityCreator<ArticleSubfamily>
{
    private readonly CreateArticleSubfamilyDto _dto;
    private readonly IArticleSubfamilyRepository _repository;
    private readonly ArticleSubfamilyReferences _dependencies;

    public ArticleSubfamilyCreator(CreateArticleSubfamilyDto dto,
                                   IArticleSubfamilyRepository subfamilyRepository,
                                   ArticleSubfamilyReferences dependencies) : 
        base(new())
    {
        _dto = dto;
        _repository = subfamilyRepository;
        _dependencies = dependencies;
    }

    protected override async Task AssignDataAsync(CancellationToken ct = default)
    {
        _entity.Designation = _dto.Designation;
        _entity.Order  = _dto.Imported ? 0 : await _repository.GetNextOrderAsync(ct);
        _entity.Code = _dto.Imported ? "?" : await _repository.GetNextCodeAsync(ct);
        _entity.Button = _dto.Button ?? new();
        _entity.FamilyId = _dto.FamilyId;
        _entity.CommissionGroupId = _dto.CommissionGroupId;
        _entity.DiscountGroupId = _dto.DiscountGroupId;
        _entity.VatOnTableId = _dto.VatOnTableId;
        _entity.VatDirectSellingId = _dto.VatDirectSellingId;
        _entity.Notes = _dto.Notes;
        _entity.IsDeleted = _dto.IsDeleted ?? false;
    }

    protected override async Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        if (_dto.Imported)
        {
            return Result.Success();
        }
         var exists= await _repository.DesignationExistsAsync(_dto.FamilyId, _dto.Designation, ct);
        if (exists)
        {
            return Result.Failure(Error.DesignationExistsError);
        }

        return Result.Success();
    }

    
    protected override async Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        if (!await _dependencies.FamilyExistsAsync(_dto.FamilyId, ct))
        {
            return Result.NotFound(nameof(ArticleFamily), _dto.FamilyId.ToString());
        }
        
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

        if (_dto.VatOnTableId.HasValue && 
            !await _dependencies.VatRateExistsAsync(_dto.VatOnTableId.Value,
                                                       ct))
        {
            return Result.NotFound(nameof(VatRate), _dto.VatOnTableId.ToString());
        }

        if (_dto.VatDirectSellingId.HasValue && 
            !await _dependencies.VatRateExistsAsync(_dto.VatDirectSellingId.Value,
                                                       ct))
        {
            return Result.NotFound(nameof(VatRate), _dto.VatDirectSellingId.ToString());
        }

        return Result.Success();
    }
}