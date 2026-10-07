using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class ArticleSubfamilyUpdater : EntityUpdater<ArticleSubfamily>
{
    private readonly UpdateArticleSubfamilyDto _dto;
    private readonly ArticleSubfamilyReferences _dependencies;

    public ArticleSubfamilyUpdater(ArticleSubfamily entity,
                                   UpdateArticleSubfamilyDto dto,
                                   IArticleSubfamilyRepository repository,
                                   ArticleSubfamilyReferences dependencies) : base(entity,repository)
    {
        _dto = dto;
        _dependencies = dependencies;
    }

    protected override async Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        return await CheckForCodeAndDesignationConflictsAsync(_dto.Code, _dto.Designation, ct);
    }

    protected override async Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        if (_dto.FamilyId != _entity.FamilyId &&
            !await _dependencies.FamilyRepository.ExistsAsync(_dto.FamilyId, ct))
        {
            return Result.NotFound(nameof(ArticleFamily), _dto.FamilyId.ToString());
        }

        if (_dto.DiscountGroupId.HasValue && _dto.DiscountGroupId != Guid.Empty && 
            _dto.DiscountGroupId != _entity.DiscountGroupId )
        {
            if (!await _dependencies.DiscountGroupRepository.ExistsAsync(_dto.DiscountGroupId.Value, ct))
            {
                return Result.NotFound(nameof(DiscountGroup), _dto.DiscountGroupId.ToString());
            }
        }

        if (_dto.CommissionGroupId.HasValue && _dto.CommissionGroupId != Guid.Empty && 
            _dto.CommissionGroupId != _entity.CommissionGroupId)
        {
            if (!await _dependencies.CommissionGroupRepository.ExistsAsync(_dto.CommissionGroupId.Value, ct))
            {
                return Result.NotFound(nameof(CommissionGroup), _dto.CommissionGroupId.ToString());
            }
        }

        if (_dto.VatOnTableId.HasValue && _dto.VatOnTableId != Guid.Empty &&
            _dto.VatOnTableId != _entity.VatOnTableId)
        {
            if (!await _dependencies.VatRateRepository.ExistsAsync(_dto.VatOnTableId.Value, ct))
            {
                return Result.NotFound(nameof(VatRate), _dto.VatOnTableId.ToString());
            }
        }

        if (_dto.VatDirectSellingId.HasValue && _dto.VatDirectSellingId != Guid.Empty && 
            _dto.VatDirectSellingId != _entity.VatDirectSellingId)
        {
            if (!await _dependencies.VatRateRepository.ExistsAsync(_dto.VatDirectSellingId.Value, ct))
            {
                return Result.NotFound(nameof(VatRate), _dto.VatDirectSellingId.ToString());
            }
        }


        return Result.Success();
    }

    protected override void SetNewData()
    {
        _entity.Order = _dto.Order;
        _entity.Code = _dto.Code;
        _entity.Designation = _dto.Designation;
        _entity.FamilyId = _dto.FamilyId;
        _entity.DiscountGroupId = _dto.DiscountGroupId == Guid.Empty ? null : _dto.DiscountGroupId;
        _entity.CommissionGroupId = _dto.CommissionGroupId == Guid.Empty ? null : _dto.CommissionGroupId;
        _entity.VatOnTableId = _dto.VatOnTableId == Guid.Empty ? null : _dto.VatOnTableId;
        _entity.VatDirectSellingId = _dto.VatDirectSellingId == Guid.Empty ? null : _dto.VatDirectSellingId;
        
        _entity.Notes = _dto.Notes;
        _entity.IsDeleted = _dto.IsDeleted;
        _entity.Button = _dto.Button;
    }
}