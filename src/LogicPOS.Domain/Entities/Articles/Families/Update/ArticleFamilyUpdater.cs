using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class ArticleFamilyUpdater : EntityUpdater<ArticleFamily>
{
    private readonly UpdateArticleFamilyDto _dto;
    private readonly ArticleFamilyReferences _dependencies;

    public ArticleFamilyUpdater(ArticleFamily entity,
                                UpdateArticleFamilyDto dto,
                                IArticleFamilyRepository repository,
                                ArticleFamilyReferences dependencies) : base(entity,repository)
    {
        _dto = dto;
        _dependencies = dependencies;
    }

    protected override async Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        return await CheckForCodeAndDesignationConflictsAsync(
            _dto.Code, 
            _dto.Designation, 
            ct);
    }

    protected override async Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        if (_dto.DiscountGroupId.HasValue && _dto.DiscountGroupId != Guid.Empty && 
            _dto.DiscountGroupId != _entity.DiscountGroupId )
        {
            if (!await _dependencies.DiscountGroupRepository.ExistsAsync(_dto.DiscountGroupId.Value, ct))
            {
                return Result.NotFound<DiscountGroup>(_dto.DiscountGroupId.Value.ToString());
            }
        }

        if (_dto.CommissionGroupId.HasValue && _dto.CommissionGroupId != Guid.Empty && 
            _dto.CommissionGroupId != _entity.CommissionGroupId)
        {
            if (!await _dependencies.CommissionGroupRepository.ExistsAsync(_dto.CommissionGroupId.Value, ct))
            {
                return Result.NotFound<CommissionGroup>(_dto.CommissionGroupId.Value.ToString());
            }
        }


        return Result.Success();
    }

    protected override void SetNewData()
    {
        _entity.Order = _dto.Order;
        _entity.Code = _dto.Code;
        _entity.Designation = _dto.Designation;

        _entity.DiscountGroupId = _dto.DiscountGroupId == Guid.Empty ? null : _dto.DiscountGroupId;
        _entity.CommissionGroupId = _dto.CommissionGroupId == Guid.Empty ? null : _dto.CommissionGroupId;
        
        _entity.Notes = _dto.Notes;
        _entity.IsDeleted = _dto.IsDeleted;
        _entity.Button = _dto.Button;
    }
}