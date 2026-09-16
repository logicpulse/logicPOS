using LogicPOS.ApiServer.Data;
using LogicPOS.ApiServer.Data.Entities;
using LogicPOS.ApiServer.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.ApiServer.Services;

public sealed class ArticleService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ArticleClassService _classService;
    private readonly ArticleSubfamilyService _subfamilyService;
    private readonly ArticleTypeService _typeService;
    private readonly MeasurementUnitService _measurementUnitService;
    private readonly SizeUnitService _sizeUnitService;
    private readonly VatRateService _vatRateService;

    public ArticleService(
        ApplicationDbContext dbContext,
        ArticleClassService classService,
        ArticleSubfamilyService subfamilyService,
        ArticleTypeService typeService,
        MeasurementUnitService measurementUnitService,
        SizeUnitService sizeUnitService,
        VatRateService vatRateService)
    {
        _dbContext = dbContext;
        _classService = classService;
        _subfamilyService = subfamilyService;
        _typeService = typeService;
        _measurementUnitService = measurementUnitService;
        _sizeUnitService = sizeUnitService;
        _vatRateService = vatRateService;
    }

    public async Task<ArticlesPaginatedResponse> GetPagedAsync(
        int page, int pageSize, string? search, Guid? familyId, Guid? subfamilyId, bool? favorite, bool? includeDeleted,
        CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 25 : pageSize;

        var query = _dbContext.ApiArticles.AsNoTracking();
        if (includeDeleted != true)
        {
            query = query.Where(item => !item.IsDeleted);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(item => item.Designation.Contains(search) || item.Code.Contains(search) || (item.Barcode != null && item.Barcode.Contains(search)));
        }

        if (favorite.HasValue)
        {
            query = query.Where(item => item.Favorite == favorite.Value);
        }

        if (subfamilyId.HasValue)
        {
            query = query.Where(item => item.SubfamilyId == subfamilyId.Value);
        }
        else if (familyId.HasValue)
        {
            var subfamilyIds = await _dbContext.ApiArticleSubfamilies
                .Where(item => item.FamilyId == familyId.Value)
                .Select(item => item.Id)
                .ToListAsync(cancellationToken);
            query = query.Where(item => subfamilyIds.Contains(item.SubfamilyId));
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)pageSize);

        var articles = await query
            .OrderBy(item => item.Order)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var viewModels = await MapToViewModelsAsync(articles, cancellationToken);

        return new ArticlesPaginatedResponse
        {
            Items = viewModels,
            ItemsCount = viewModels.Count,
            TotalItems = totalItems,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages
        };
    }

    public async Task<ArticleResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var article = await _dbContext.ApiArticles.AsNoTracking().SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (article is null)
        {
            return null;
        }

        var articleClass = await _classService.GetByIdInternalAsync(article.ClassId, cancellationToken);
        var subfamily = await _subfamilyService.GetByIdInternalAsync(article.SubfamilyId, cancellationToken);
        var type = await _typeService.GetByIdInternalAsync(article.TypeId, cancellationToken);
        var measurementUnit = await _measurementUnitService.GetByIdInternalAsync(article.MeasurementUnitId, cancellationToken);
        var sizeUnit = await _sizeUnitService.GetByIdInternalAsync(article.SizeUnitId, cancellationToken);
        var vatRate = await _vatRateService.GetByIdInternalAsync(article.VatDirectSellingId, cancellationToken);

        return MapToArticle(article, articleClass, subfamily, type, measurementUnit, sizeUnit, vatRate);
    }

    public async Task<ArticleViewModelResponse?> GetViewModelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var article = await _dbContext.ApiArticles.AsNoTracking().SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (article is null)
        {
            return null;
        }

        var list = await MapToViewModelsAsync([article], cancellationToken);
        return list.Single();
    }

    public async Task<ArticleViewModelResponse?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var article = await _dbContext.ApiArticles.AsNoTracking().SingleOrDefaultAsync(item => item.Code == code, cancellationToken);
        if (article is null)
        {
            return null;
        }

        var list = await MapToViewModelsAsync([article], cancellationToken);
        return list.Single();
    }

    public async Task<(bool Success, string? Field, string? Error, AddEntityIdResponse? Response)> CreateAsync(AddArticleRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await ValidateReferencesAsync(request.ClassId, request.SubfamilyId, request.TypeId, request.MeasurementUnitId, request.SizeUnitId, request.VatDirectSellingId, cancellationToken);
        if (validation is not null)
        {
            return (false, validation.Value.Field, validation.Value.Error, null);
        }

        var count = await _dbContext.ApiArticles.CountAsync(cancellationToken);
        var article = new ApiArticle
        {
            Id = Guid.NewGuid(),
            Order = (uint)(count + 1),
            Code = string.IsNullOrWhiteSpace(request.Code) ? $"A{count + 1}" : request.Code,
            CodeDealer = request.CodeDealer,
            Designation = request.Designation.Trim(),
            ClassId = request.ClassId,
            SubfamilyId = request.SubfamilyId,
            TypeId = request.TypeId,
            MeasurementUnitId = request.MeasurementUnitId,
            SizeUnitId = request.SizeUnitId,
            CommissionGroupId = request.CommissionGroupId,
            DiscountGroupId = request.DiscountGroupId,
            VatOnTableId = request.VatOnTableId,
            VatDirectSellingId = request.VatDirectSellingId,
            VatExemptionReasonId = request.VatExemptionReasonId,
            ButtonLabel = request.Button?.Label,
            ButtonImage = request.Button?.Image,
            Price1Value = request.Price1?.Value ?? 0,
            Price1PromotionValue = request.Price1?.PromotionValue ?? 0,
            Price1UsePromotion = request.Price1?.UsePromotion ?? false,
            Price2Value = request.Price2?.Value ?? 0,
            Price2PromotionValue = request.Price2?.PromotionValue ?? 0,
            Price2UsePromotion = request.Price2?.UsePromotion ?? false,
            Price3Value = request.Price3?.Value ?? 0,
            Price3PromotionValue = request.Price3?.PromotionValue ?? 0,
            Price3UsePromotion = request.Price3?.UsePromotion ?? false,
            Price4Value = request.Price4?.Value ?? 0,
            Price4PromotionValue = request.Price4?.PromotionValue ?? 0,
            Price4UsePromotion = request.Price4?.UsePromotion ?? false,
            Price5Value = request.Price5?.Value ?? 0,
            Price5PromotionValue = request.Price5?.PromotionValue ?? 0,
            Price5UsePromotion = request.Price5?.UsePromotion ?? false,
            PriceWithVat = request.PriceWithVat,
            Discount = request.Discount,
            DefaultQuantity = request.DefaultQuantity,
            MinimumStock = request.MinimumStock,
            Tare = request.Tare,
            Weight = request.Weight,
            Barcode = request.Barcode,
            PVPVariable = request.PVPVariable,
            Favorite = request.Favorite,
            UseWeighingBalance = request.UseWeighingBalance,
            IsComposed = request.IsComposed,
            UniqueArticles = request.UniqueArticles,
            IsSdrPackaging = request.IsSdrPackaging,
            BarcodeLabelPrintModel = request.BarcodeLabelPrintModel,
            Notes = request.Notes ?? string.Empty,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
            // request.TotalStock is intentionally not persisted: no Stocks subsystem exists yet.
        };

        _dbContext.ApiArticles.Add(article);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return (true, null, null, new AddEntityIdResponse { Id = article.Id });
    }

    public async Task<(bool Success, string? Field, string? Error)> UpdateAsync(Guid id, UpdateArticleRequest request, CancellationToken cancellationToken = default)
    {
        var article = await _dbContext.ApiArticles.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (article is null)
        {
            return (false, null, "Article não encontrado.");
        }

        var classId = request.ClassId ?? article.ClassId;
        var subfamilyId = request.SubfamilyId ?? article.SubfamilyId;
        var typeId = request.TypeId ?? article.TypeId;
        var measurementUnitId = request.MeasurementUnitId ?? article.MeasurementUnitId;
        var sizeUnitId = request.SizeUnitId ?? article.SizeUnitId;
        var vatDirectSellingId = request.VatDirectSellingId ?? article.VatDirectSellingId;

        var validation = await ValidateReferencesAsync(classId, subfamilyId, typeId, measurementUnitId, sizeUnitId, vatDirectSellingId, cancellationToken);
        if (validation is not null)
        {
            return (false, validation.Value.Field, validation.Value.Error);
        }

        article.Order = request.Order;
        article.Code = string.IsNullOrWhiteSpace(request.Code) ? article.Code : request.Code;
        article.CodeDealer = request.CodeDealer;
        article.Designation = request.Designation.Trim();
        article.ClassId = classId;
        article.SubfamilyId = subfamilyId;
        article.TypeId = typeId;
        article.MeasurementUnitId = measurementUnitId;
        article.SizeUnitId = sizeUnitId;
        article.CommissionGroupId = request.CommissionGroupId;
        article.DiscountGroupId = request.DiscountGroupId;
        article.VatOnTableId = request.VatOnTableId;
        article.VatDirectSellingId = vatDirectSellingId;
        article.VatExemptionReasonId = request.VatExemptionReasonId;
        article.ButtonLabel = request.Button?.Label ?? article.ButtonLabel;
        article.ButtonImage = request.Button?.Image ?? article.ButtonImage;

        if (request.Price1 is not null) { article.Price1Value = request.Price1.Value; article.Price1PromotionValue = request.Price1.PromotionValue; article.Price1UsePromotion = request.Price1.UsePromotion; }
        if (request.Price2 is not null) { article.Price2Value = request.Price2.Value; article.Price2PromotionValue = request.Price2.PromotionValue; article.Price2UsePromotion = request.Price2.UsePromotion; }
        if (request.Price3 is not null) { article.Price3Value = request.Price3.Value; article.Price3PromotionValue = request.Price3.PromotionValue; article.Price3UsePromotion = request.Price3.UsePromotion; }
        if (request.Price4 is not null) { article.Price4Value = request.Price4.Value; article.Price4PromotionValue = request.Price4.PromotionValue; article.Price4UsePromotion = request.Price4.UsePromotion; }
        if (request.Price5 is not null) { article.Price5Value = request.Price5.Value; article.Price5PromotionValue = request.Price5.PromotionValue; article.Price5UsePromotion = request.Price5.UsePromotion; }

        article.PriceWithVat = request.PriceWithVat;
        article.Discount = request.Discount;
        article.DefaultQuantity = request.DefaultQuantity;
        article.MinimumStock = request.MinimumStock;
        article.Tare = request.Tare;
        article.Weight = request.Weight;
        article.Barcode = request.Barcode;
        article.PVPVariable = request.PVPVariable;
        article.Favorite = request.Favorite;
        article.UseWeighingBalance = request.UseWeighingBalance;
        article.IsComposed = request.IsComposed;
        article.UniqueArticles = request.UniqueArticles;
        article.IsSdrPackaging = request.IsSdrPackaging;
        article.BarcodeLabelPrintModel = request.BarcodeLabelPrintModel;
        article.Notes = request.Notes ?? string.Empty;
        article.IsDeleted = request.IsDeleted;
        article.UpdatedUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return (true, null, null);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var article = await _dbContext.ApiArticles.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (article is null)
        {
            return false;
        }

        _dbContext.ApiArticles.Remove(article);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<(string Field, string Error)?> ValidateReferencesAsync(
        Guid classId, Guid subfamilyId, Guid typeId, Guid measurementUnitId, Guid sizeUnitId, Guid vatDirectSellingId,
        CancellationToken cancellationToken)
    {
        if (!await _classService.ExistsAsync(classId, cancellationToken))
        {
            return ("classId", "ClassId não existe.");
        }

        if (!await _subfamilyService.ExistsAsync(subfamilyId, cancellationToken))
        {
            return ("subfamilyId", "SubfamilyId não existe.");
        }

        if (!await _typeService.ExistsAsync(typeId, cancellationToken))
        {
            return ("typeId", "TypeId não existe.");
        }

        if (!await _measurementUnitService.ExistsAsync(measurementUnitId, cancellationToken))
        {
            return ("measurementUnitId", "MeasurementUnitId não existe.");
        }

        if (!await _sizeUnitService.ExistsAsync(sizeUnitId, cancellationToken))
        {
            return ("sizeUnitId", "SizeUnitId não existe.");
        }

        if (!await _vatRateService.ExistsAsync(vatDirectSellingId, cancellationToken))
        {
            return ("vatDirectSellingId", "VatDirectSellingId não existe.");
        }

        return null;
    }

    private async Task<List<ArticleViewModelResponse>> MapToViewModelsAsync(List<ApiArticle> articles, CancellationToken cancellationToken)
    {
        if (articles.Count == 0)
        {
            return [];
        }

        var subfamilyIds = articles.Select(item => item.SubfamilyId).Distinct().ToList();
        var subfamilies = await _dbContext.ApiArticleSubfamilies.AsNoTracking().Where(item => subfamilyIds.Contains(item.Id)).ToDictionaryAsync(item => item.Id, cancellationToken);

        var familyIds = subfamilies.Values.Select(item => item.FamilyId).Distinct().ToList();
        var families = await _dbContext.ApiArticleFamilies.AsNoTracking().Where(item => familyIds.Contains(item.Id)).ToDictionaryAsync(item => item.Id, cancellationToken);

        var typeIds = articles.Select(item => item.TypeId).Distinct().ToList();
        var types = await _dbContext.ApiArticleTypes.AsNoTracking().Where(item => typeIds.Contains(item.Id)).ToDictionaryAsync(item => item.Id, cancellationToken);

        var classIds = articles.Select(item => item.ClassId).Distinct().ToList();
        var classes = await _dbContext.ApiArticleClasses.AsNoTracking().Where(item => classIds.Contains(item.Id)).ToDictionaryAsync(item => item.Id, cancellationToken);

        var unitIds = articles.Select(item => item.MeasurementUnitId).Distinct().ToList();
        var units = await _dbContext.ApiMeasurementUnits.AsNoTracking().Where(item => unitIds.Contains(item.Id)).ToDictionaryAsync(item => item.Id, cancellationToken);

        var vatRateIds = articles.Select(item => item.VatDirectSellingId).Distinct().ToList();
        var vatRates = await _dbContext.ApiVatRates.AsNoTracking().Where(item => vatRateIds.Contains(item.Id)).ToDictionaryAsync(item => item.Id, cancellationToken);

        return articles.Select(article =>
        {
            var subfamily = subfamilies.GetValueOrDefault(article.SubfamilyId);
            var family = subfamily is not null ? families.GetValueOrDefault(subfamily.FamilyId) : null;
            var type = types.GetValueOrDefault(article.TypeId);
            var articleClass = classes.GetValueOrDefault(article.ClassId);
            var unit = units.GetValueOrDefault(article.MeasurementUnitId);
            var vatRate = vatRates.GetValueOrDefault(article.VatDirectSellingId);

            return new ArticleViewModelResponse
            {
                Id = article.Id,
                Notes = article.Notes,
                CreatedAt = article.CreatedUtc,
                UpdatedAt = article.UpdatedUtc,
                UpdatedBy = Guid.Empty,
                IsDeleted = article.IsDeleted,
                Order = article.Order,
                Code = article.Code,
                IsComposed = article.IsComposed,
                IsSdrPackaging = article.IsSdrPackaging,
                Family = family?.Designation ?? string.Empty,
                Subfamily = subfamily?.Designation ?? string.Empty,
                Designation = article.Designation,
                Type = type?.Designation ?? string.Empty,
                DefaultQuantity = article.DefaultQuantity,
                MinimumStock = article.MinimumStock,
                Price1 = article.Price1Value,
                Price2 = article.Price2Value,
                Price3 = article.Price3Value,
                Price4 = article.Price4Value,
                Price5 = article.Price5Value,
                Price1PromotionValue = article.Price1PromotionValue,
                Price2PromotionValue = article.Price2PromotionValue,
                Price3PromotionValue = article.Price3PromotionValue,
                Price4PromotionValue = article.Price4PromotionValue,
                Price5PromotionValue = article.Price5PromotionValue,
                Price1UsePromotion = article.Price1UsePromotion,
                Price2UsePromotion = article.Price2UsePromotion,
                Price3UsePromotion = article.Price3UsePromotion,
                Price4UsePromotion = article.Price4UsePromotion,
                Price5UsePromotion = article.Price5UsePromotion,
                VatDirectSelling = vatRate?.Value,
                Discount = article.Discount,
                Unit = unit?.Acronym ?? string.Empty,
                SubfamilyId = article.SubfamilyId,
                FamilyId = subfamily?.FamilyId ?? Guid.Empty,
                VatRateId = article.VatDirectSellingId,
                VatExemptionReasonId = article.VatExemptionReasonId,
                Button = article.ButtonLabel is null && article.ButtonImage is null ? null : new ButtonResponse { Label = article.ButtonLabel, Image = article.ButtonImage },
                PriceWithVat = article.PriceWithVat,
                ClassAcronym = articleClass?.Acronym ?? string.Empty,
                BarcodeLabelPrintModel = article.BarcodeLabelPrintModel
            };
        }).ToList();
    }

    private static ArticleResponse MapToArticle(
        ApiArticle article, ArticleClassResponse? articleClass, ArticleSubfamilyResponse? subfamily, ArticleTypeResponse? type,
        MeasurementUnitResponse? measurementUnit, SizeUnitResponse? sizeUnit, VatRateResponse? vatRate)
    {
        return new ArticleResponse
        {
            Id = article.Id,
            Notes = article.Notes,
            CreatedAt = article.CreatedUtc,
            UpdatedAt = article.UpdatedUtc,
            UpdatedBy = Guid.Empty,
            IsDeleted = article.IsDeleted,
            Code = article.Code,
            Order = article.Order,
            Designation = article.Designation,
            ClassId = article.ClassId,
            Class = articleClass,
            Subfamily = subfamily,
            SubfamilyId = article.SubfamilyId,
            Type = type,
            MeasurementUnit = measurementUnit,
            SizeUnit = sizeUnit,
            VatDirectSelling = vatRate,
            Button = article.ButtonLabel is null && article.ButtonImage is null ? null : new ButtonResponse { Label = article.ButtonLabel, Image = article.ButtonImage },
            Price1 = new ArticlePriceResponse { Value = article.Price1Value, PromotionValue = article.Price1PromotionValue, UsePromotion = article.Price1UsePromotion },
            Price2 = new ArticlePriceResponse { Value = article.Price2Value, PromotionValue = article.Price2PromotionValue, UsePromotion = article.Price2UsePromotion },
            Price3 = new ArticlePriceResponse { Value = article.Price3Value, PromotionValue = article.Price3PromotionValue, UsePromotion = article.Price3UsePromotion },
            Price4 = new ArticlePriceResponse { Value = article.Price4Value, PromotionValue = article.Price4PromotionValue, UsePromotion = article.Price4UsePromotion },
            Price5 = new ArticlePriceResponse { Value = article.Price5Value, PromotionValue = article.Price5PromotionValue, UsePromotion = article.Price5UsePromotion },
            PriceWithVat = article.PriceWithVat,
            Discount = article.Discount,
            DefaultQuantity = article.DefaultQuantity,
            MinimumStock = article.MinimumStock,
            Tare = article.Tare,
            Weight = article.Weight,
            Barcode = article.Barcode,
            PVPVariable = article.PVPVariable,
            Favorite = article.Favorite,
            UseWeighingBalance = article.UseWeighingBalance,
            IsComposed = article.IsComposed,
            UniqueArticles = article.UniqueArticles,
            IsSdrPackaging = article.IsSdrPackaging,
            BarcodeLabelPrintModel = article.BarcodeLabelPrintModel
        };
    }
}
