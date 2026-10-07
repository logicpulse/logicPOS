using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Errors;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;
using LogicPOS.Domain.ValueObjects;

namespace LogicPOS.Domain.Entities.Utilities;

internal class ArticleCreator : EntityCreator<Article>
{
    private readonly CreateArticleDto _dto;
    private readonly IArticleRepository _repository;
    private readonly ArticleReferences _dependencies;

    public ArticleCreator(CreateArticleDto dto,
                          IArticleRepository repository,
                          ArticleReferences dependencies) :
        base(new())
    {
        _dto = dto;
        _repository = repository;
        _dependencies = dependencies;
    }

    protected override async Task AssignDataAsync(CancellationToken ct = default)
    {
        _entity.Order  = _dto.Imported  ? 0 :  await _repository.GetNextOrderAsync(ct);
        _entity.Code = _dto.Imported ? "?" :  await _repository.GetNextCodeAsync(ct);
        _entity.CodeDealer = _dto.CodeDealer;
        _entity.Designation = _dto.Designation;
        _entity.Button = _dto.Button ?? new();
        _entity.Price1 = _dto.Price1;
        _entity.Price2 = _dto.Price2 ?? ArticlePrice.Default();
        _entity.Price3 = _dto.Price3 ?? ArticlePrice.Default();
        _entity.Price4 = _dto.Price4 ?? ArticlePrice.Default();
        _entity.Price5 = _dto.Price5 ?? ArticlePrice.Default();
        _entity.PriceWithVat = _dto.PriceWithVat ?? false;
        _entity.Discount = _dto.Discount ?? 0;
        _entity.DefaultQuantity = _dto.DefaultQuantity ?? 1;
        _entity.MinimumStock = _dto.MinimumStock ?? 0;
        _entity.Tare = _dto.Tare ?? 0;
        _entity.Weight = _dto.Weight ?? 0;
        _entity.Barcode = _dto.Barcode;
        _entity.PVPVariable = _dto.PVPVariable ?? false;
        _entity.IsTicketing = _dto.IsTicketing;
        _entity.Favorite = _dto.Favorite;
        _entity.UseWeighingBalance = _dto.UseWeighingBalance ?? false;
        _entity.SubfamilyId = _dto.SubfamilyId;
        _entity.TypeId = _dto.TypeId;
        _entity.ClassId = _dto.ClassId;
        _entity.MeasurementUnitId = _dto.MeasurementUnitId;
        _entity.SizeUnitId = _dto.SizeUnitId;
        _entity.CommissionGroupId = _dto.CommissionGroupId;
        _entity.DiscountGroupId = _dto.DiscountGroupId;
        _entity.VatOnTableId = _dto.VatOnTableId;
        _entity.VatDirectSellingId = _dto.VatDirectSellingId;
        _entity.VatExemptionReasonId = _dto.VatExemptionReasonId;
        _entity.IsComposed = _dto.IsComposed;
        _entity.UniqueArticles = _dto.UniqueArticles;
        _entity.IsSdrPackaging = _dto.IsSdrPackaging;
        _entity.Notes = _dto.Notes;
        _entity.BarcodeLabelPrintModel = _dto.BarcodeLabelPrintModel;
        _entity.IsDeleted = _dto.IsDeleted ?? false;
        if(!_dto.Imported && !string.IsNullOrEmpty(_dto.Code))
        {
            _entity.Code = _dto.Code;
        }
    }

    protected override async Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        if (_dto.Imported)
        {
            return Result.Success();
        }
        
        var codeConflictsResult= await CheckForCodeConflictAsync(ct);
        if (codeConflictsResult.IsFailure)
        {
            return codeConflictsResult;
        }
        return await CheckForDesignationConflictAsync(ct);
        
    }
    
    private async Task<Result> CheckForDesignationConflictAsync(CancellationToken ct)
    {
        var exists = await _repository.DesignationExistsAsync(_dto.SubfamilyId, _dto.Designation, ct);
        
        if (exists)
        {
            return Result.Failure(Error.DesignationExistsError);
        }
        
        return Result.Success();
    }
     private async Task<Result> CheckForCodeConflictAsync(CancellationToken ct)
    {
        if (string.IsNullOrEmpty(_dto.Code))
        {
            return Result.Success();
        }
        var codeExists = await _repository.CodeExistsAsync(_dto.Code);
        if (codeExists)
        {
            return Result.Failure(Error.Conflict($"O código {_dto.Code} já existe"));
        }
        return Result.Success();
    } 
    protected override async Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        if (!await _dependencies.SizeUnitExistsAsync(_dto.SizeUnitId, ct))
        {
            return Result.NotFound(nameof(SizeUnit),_dto.SizeUnitId.ToString());
        }

        if (!await _dependencies.MeasurementUnitExistsAsync(_dto.MeasurementUnitId, ct))
        {
            return Result.NotFound(nameof(MeasurementUnit),_dto.MeasurementUnitId.ToString());
        }

        if (!await _dependencies.ClassExistsAsync(_dto.ClassId, ct))
        {
            return Result.NotFound(nameof(ArticleClass),_dto.ClassId.ToString());
        }

        if (!await _dependencies.TypeExistsAsync(_dto.TypeId, ct))
        {
            return Result.NotFound(nameof(ArticleType),_dto.TypeId.ToString());
        }

        if (!await _dependencies.SubfamilyExistsAsync(_dto.SubfamilyId, ct))
        {
            return Result.NotFound(nameof(ArticleSubfamily),_dto.SubfamilyId.ToString());
        }

        if (_dto.CommissionGroupId.HasValue &&
            !await _dependencies.CommissionGroupExistsAsync(_dto.CommissionGroupId.Value,
                                                            ct))
        {
            return Result.NotFound(nameof(CommissionGroup),_dto.CommissionGroupId.ToString());
        }

        if (_dto.DiscountGroupId.HasValue &&
            !await _dependencies.DiscountGroupExistsAsync(_dto.DiscountGroupId.Value,
                                                          ct))
        {
            return Result.NotFound(nameof(DiscountGroup),_dto.DiscountGroupId.ToString());
        }

        if (_dto.VatOnTableId.HasValue &&
            !await _dependencies.VatRateExistsAsync(_dto.VatOnTableId.Value,
                                                    ct))
        {
            return Result.NotFound(nameof(VatRate),_dto.VatOnTableId.ToString());
        }

        if (!await _dependencies.VatRateExistsAsync(_dto.VatDirectSellingId, ct))
        {
            return Result.NotFound(nameof(VatRate),_dto.VatDirectSellingId.ToString());
        }

        if (_dto.VatExemptionReasonId.HasValue &&
            !await _dependencies.VatExemptionReasonExistsAsync(_dto.VatExemptionReasonId.Value,
                                                              ct))
        {
            return Result.NotFound(nameof(VatExemptionReason),_dto.VatExemptionReasonId.ToString());
        }

       
        return Result.Success();
    }
}