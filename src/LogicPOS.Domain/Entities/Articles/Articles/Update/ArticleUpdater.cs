using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Errors;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class ArticleUpdater : EntityUpdater<Article>
{
    private readonly UpdateArticleDto _dto;
    private readonly ArticleReferences _dependencies;

    public ArticleUpdater(Article entity,
        UpdateArticleDto dto,
        IArticleRepository repository,
        ArticleReferences dependencies) : base(entity, repository)
    {
        _dto = dto;
        _dependencies = dependencies;
    }

    protected override async Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        return await CheckForCodeAndDesignationConflictsAsync(ct);
    }

    private async Task<Result> CheckForCodeAndDesignationConflictsAsync(
        CancellationToken ct = default)
    {
        var codeResult = await CheckForCodeConflictAsync(_dto.Code, ct);
        if (codeResult.IsFailure)
        {
            return codeResult;
        }

        var designationResult = await CheckForDesignationConflictAsync(ct);
        if (designationResult.IsFailure)
        {
            return designationResult;
        }

        return Result.Success();
    }

    private async Task<Result> CheckForDesignationConflictAsync(CancellationToken ct = default)
    {
        Guid subfamilyId = _dto.SubfamilyId;
        string designation = _dto.Designation;

        if (designation == _entity.Designation && subfamilyId == _entity.SubfamilyId)
        {
            return Result.Success();
        }

        var articleRepository = _repository as IArticleRepository;

        if (await articleRepository!.DesignationExistsAsync(subfamilyId,
                designation, ct))
        {
            return Result.Failure(Error.DesignationExistsError);
        }

        return Result.Success();
    }

    protected override async Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        if (_dto.SizeUnitId != _entity.SizeUnitId &&
            !await _dependencies.SizeUnitRepository.ExistsAsync(_dto.SizeUnitId, ct))
        {
            return Result.NotFound(nameof(SizeUnit), _dto.SizeUnitId.ToString());
        }

        if (_dto.MeasurementUnitId != _entity.MeasurementUnitId &&
            !await _dependencies.MeasurementUnitRepository.ExistsAsync(_dto.MeasurementUnitId, ct))
        {
            return Result.NotFound(nameof(MeasurementUnit), _dto.MeasurementUnitId.ToString());
        }

        if (_dto.TypeId != _entity.TypeId &&
            !await _dependencies.ArticleTypeRepository.ExistsAsync(_dto.TypeId, ct))
        {
            return Result.NotFound(nameof(ArticleType), _dto.TypeId.ToString());
        }

        if (_dto.ClassId != _entity.ClassId &&
            !await _dependencies.ArticleClassRepository.ExistsAsync(_dto.ClassId, ct))
        {
            return Result.NotFound(nameof(ArticleClass), _dto.ClassId.ToString());
        }

        if (_dto.SubfamilyId != _entity.SubfamilyId &&
            !await _dependencies.ArticleSubfamilyRepository.ExistsAsync(_dto.SubfamilyId, ct))
        {
            return Result.NotFound(nameof(ArticleSubfamily), _dto.SubfamilyId.ToString());
        }

        if (_dto.DiscountGroupId is { } discountGroupId && discountGroupId != Guid.Empty &&
            discountGroupId != _entity.DiscountGroupId &&
            !await _dependencies.DiscountGroupRepository.ExistsAsync(discountGroupId, ct))
        {
            return Result.NotFound(nameof(DiscountGroup), discountGroupId.ToString());
        }

        if (_dto.CommissionGroupId is { } commissionGroupId && commissionGroupId != Guid.Empty &&
            commissionGroupId != _entity.CommissionGroupId &&
            !await _dependencies.CommissionGroupRepository.ExistsAsync(commissionGroupId, ct))
        {
            return Result.NotFound(nameof(CommissionGroup), commissionGroupId.ToString());
        }

        if (_dto.VatExemptionReasonId is { } vatExemptionReasonId && vatExemptionReasonId != Guid.Empty &&
            vatExemptionReasonId != _entity.VatExemptionReasonId &&
            !await _dependencies.VatExemptionReasonRepository.ExistsAsync(vatExemptionReasonId, ct))
        {
            return Result.NotFound(nameof(VatExemptionReason), vatExemptionReasonId.ToString());
        }

        if (_dto.VatOnTableId is { } vatOnTableId && vatOnTableId != Guid.Empty &&
            vatOnTableId != _entity.VatOnTableId &&
            !await _dependencies.VatRateRepository.ExistsAsync(vatOnTableId, ct))
        {
            return Result.NotFound(nameof(VatRate), vatOnTableId.ToString());
        }

        if (_dto.VatDirectSellingId != _entity.VatDirectSellingId &&
            !await _dependencies.VatRateRepository.ExistsAsync(_dto.VatDirectSellingId, ct))
        {
            return Result.NotFound(nameof(VatRate), _dto.VatDirectSellingId.ToString());
        }

        return Result.Success();
    }

    protected override void SetNewData()
    {
        _entity.Code = _dto.Code;
        _entity.Order = _dto.Order;
        _entity.Designation = _dto.Designation;
        _entity.Barcode = _dto.Barcode;
        _entity.SizeUnitId = _dto.SizeUnitId;
        _entity.MeasurementUnitId = _dto.MeasurementUnitId;
        _entity.TypeId = _dto.TypeId;
        _entity.ClassId = _dto.ClassId;

        _entity.DiscountGroupId = _dto.DiscountGroupId == Guid.Empty ? null : _dto.DiscountGroupId;

        _entity.CommissionGroupId = _dto.CommissionGroupId == Guid.Empty ? null : _dto.CommissionGroupId;

        _entity.VatExemptionReasonId = _dto.VatExemptionReasonId == Guid.Empty ? null : _dto.VatExemptionReasonId;

        _entity.VatOnTableId = _dto.VatOnTableId == Guid.Empty ? null : _dto.VatOnTableId;

        _entity.VatDirectSellingId = _dto.VatDirectSellingId;
        _entity.Notes = _dto.Notes;
        _entity.Button = _dto.Button;
        _entity.DefaultQuantity = _dto.DefaultQuantity;
        _entity.Discount = _dto.Discount;
        _entity.Favorite = _dto.Favorite;
        _entity.IsComposed = _dto.IsComposed;
        _entity.MinimumStock = _dto.MinimumStock;
        _entity.PVPVariable = _dto.PVPVariable;
        _entity.Tare = _dto.Tare;
        _entity.UniqueArticles = _dto.UniqueArticles;
        _entity.IsSdrPackaging = _dto.IsSdrPackaging;
        _entity.IsTicketing = _dto.IsTicketing;
        _entity.UseWeighingBalance = _dto.UseWeighingBalance;
        _entity.SubfamilyId = _dto.SubfamilyId;
        _entity.Weight = _dto.Weight;
        _entity.Price1 = _dto.Price1;
        _entity.Price2 = _dto.Price2;
        _entity.Price3 = _dto.Price3;
        _entity.Price4 = _dto.Price4;
        _entity.Price5 = _dto.Price5;
        _entity.PriceWithVat = _dto.PriceWithVat;
        _entity.CodeDealer = _dto.CodeDealer;
        _entity.IsDeleted = _dto.IsDeleted;
        _entity.BarcodeLabelPrintModel = _dto.BarcodeLabelPrintModel;
    }
}