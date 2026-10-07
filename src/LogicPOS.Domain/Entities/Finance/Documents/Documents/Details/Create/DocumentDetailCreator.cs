using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Finance.Calculations;
using LogicPOS.Domain.Entities.Finance.Documents.Details.Common;
using LogicPOS.Domain.Results;
using LogicPOS.Domain.ValueObjects;

namespace LogicPOS.Domain.Entities.Utilities;

internal class DocumentDetailCreator : EntityCreator<DocumentDetail>
{
    private readonly CreateDocumentDetailDto _dto;
    private readonly DocumentDetailReferences _references;
    private ArticleDetail? _article;
    private VatRate? _vatRate;
    private VatExemptionReason? _vatExemptionReason;
    private bool _isPriceWithVat;
    private decimal _unitPrice;
    private bool UseAgtCalculations => _dto.UseAgtRounding && !_isPriceWithVat;
    public DocumentDetailCreator(CreateDocumentDetailDto dto,
                                 DocumentDetailReferences references) : base(new())
    {
        _dto = dto;
        _references = references;
    }

    protected override async Task AssignDataAsync(CancellationToken ct = default)
    {
        _article = await ResolveArticleAsync(ct);
        _isPriceWithVat = _article.PriceWithVat;
        _entity.Order = _article.Order;
        _entity.Code = _article.Code;
        _entity.Designation = _article.Designation;
        _entity.Unit = _article.Unit;
        _entity.Quantity = _dto.Quantity;
        _entity.ArticleId = _dto.ArticleId;
        _vatRate = await GetVatRateAsync(ct);
        _unitPrice = ResolveUnitPrice();
        _entity.Price = _unitPrice;

        _entity.Tax = new DocumentDetailTax
        {
            TaxId = _vatRate.Id,
            Designation = _vatRate.Designation,
            Percentage = _vatRate.Value,
            Code = _vatRate.TaxCode,
            Type = _vatRate.TaxType,
            CountryRegion = _vatRate.CountryRegion,
            Description = _vatRate.Description
        };

        _vatExemptionReason = await GetExemptionReasonAsync(ct);

        if (_vatExemptionReason != null)
        {
            _entity.VatExemptionReason = _vatExemptionReason.Designation;
            _entity.VatExemptionCode = _vatExemptionReason.Code;
        }
        _entity.VatExemptionReason = _vatExemptionReason?.Designation;
        _entity.VatExemptionCode = _vatExemptionReason?.Acronym;
        _entity.Discount = _dto.Discount;
        _entity.TotalNet = CalculateNetTotal();
        _entity.TotalDiscount = CalculateDiscountPrice();
        _entity.TotalTax = CalculateTaxPrice();
        _entity.TotalFinal = CalculateTotalFinal();
        _entity.PriceType = _dto.PriceType;
        _entity.Notes = _dto.Notes ?? _article.Notes;
        _entity.SerialNumber = _dto.SerialNumber;
    }

    private async Task<ArticleDetail> ResolveArticleAsync(CancellationToken ct)
    {
        if (_references.Lookup?.Articles.TryGetValue(_dto.ArticleId, out var article) == true)
        {
            return article;
        }

        return await _references.ArticleRepository.GetArticleDetailAsync(_dto.ArticleId, ct);
    }

    private decimal CalculateTotalFinal()
    {
        if (UseAgtCalculations)
        {
            return CalculateNetTotal().TruncateDecimal() + CalculateTaxPrice().ApplyAgtRounding();
        }
        return CalculateNetTotal() + CalculateTaxPrice();
    }

    private decimal CalculateNetTotal()
    {
        var netTotal = CalculateTotalWithoutDiscount() - CalculateDiscountPrice();
        if (UseAgtCalculations)
        {
            return netTotal.TruncateDecimal();
        }
        return netTotal;
    }

    private decimal CalculateTaxPrice()
    {
        var taxPrice = CalculateNetTotal() * _vatRate!.Value / 100;
        if (UseAgtCalculations)
        {
            return taxPrice.ApplyAgtRounding();
        }

        return taxPrice;
    }

    private decimal CalculateTotalWithoutDiscount()
    {
        return _unitPrice * _dto.Quantity;
    }

    private decimal CalculateDiscountPrice()
    {
        return CalculateTotalWithoutDiscount() * _dto.Discount / 100;
    }

    private async Task<VatRate> GetVatRateAsync(CancellationToken ct)
    {
        Guid vatRateId = _dto.VatRateId ?? _article!.VatRateId;
        if (_references.Lookup?.VatRates.TryGetValue(vatRateId, out var vatRate) == true)
        {
            return vatRate;
        }

        return (await _references.VatRateRepository.GetByIdAsync(vatRateId, ct))!;
    }

    private async Task<VatExemptionReason?> GetExemptionReasonAsync(CancellationToken ct)
    {
        Guid? exemptionId = null;
        if (_dto.VatExemptionId.HasValue)
        {
            exemptionId = _dto.VatExemptionId.Value;
        }
        else if (_vatRate!.Value == 0 && _article!.VatExemptionReasonId.HasValue)
        {
            exemptionId = _article.VatExemptionReasonId.Value;
        }

        if (!exemptionId.HasValue)
        {
            return null;
        }

        if (_references.Lookup?.Exemptions.TryGetValue(exemptionId.Value, out var exemption) == true)
        {
            return exemption;
        }

        return await _references.VatExemptionRepository.GetByIdAsync(exemptionId.Value, ct);
    }

    private decimal ResolveUnitPrice()
    {
        if (_dto.Price != null)
        {
            return _dto.Price.Value;
        }

        var catalogPrice = _dto.PriceType == null
            ? _article!.Price1
            : _article!.GetPriceByType(_dto.PriceType.Value);

        return UnitPriceCalculator.ToNetUnitPrice(catalogPrice, _vatRate!.Value, _isPriceWithVat);
    }

    protected override Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success());
    }

    protected override async Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        if (!await _references.ArticleExistsAsync(_dto.ArticleId, ct))
        {
            return Result.NotFound(nameof(Article), _dto.ArticleId.ToString());
        }

        if (_dto.VatExemptionId.HasValue && !await _references.VatExemptionExistsAsync(_dto.VatExemptionId.Value, ct))
        {
            return Result.NotFound(nameof(VatExemptionReason), _dto.VatExemptionId.ToString());
        }

        if (_dto.VatRateId.HasValue && !await _references.VatRateExistsAsync(_dto.VatRateId.Value, ct))
        {
            return Result.NotFound(nameof(VatRate), _dto.VatRateId.ToString());
        }

        return Result.Success();
    }
}