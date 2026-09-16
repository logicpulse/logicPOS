using LogicPOS.ApiServer.Data;
using LogicPOS.ApiServer.Data.Entities;
using LogicPOS.ApiServer.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.ApiServer.Services;

public sealed class ArticleSubfamilyService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ArticleFamilyService _familyService;
    private readonly VatRateService _vatRateService;

    public ArticleSubfamilyService(ApplicationDbContext dbContext, ArticleFamilyService familyService, VatRateService vatRateService)
    {
        _dbContext = dbContext;
        _familyService = familyService;
        _vatRateService = vatRateService;
    }

    public async Task<IReadOnlyList<ArticleSubfamilyResponse>> GetAllAsync(Guid? familyId, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.ApiArticleSubfamilies.AsNoTracking().Where(item => !item.IsDeleted);
        if (familyId.HasValue)
        {
            query = query.Where(item => item.FamilyId == familyId.Value);
        }

        var items = await query.OrderBy(item => item.Order).ToListAsync(cancellationToken);
        if (items.Count == 0)
        {
            return [];
        }

        var families = await _dbContext.ApiArticleFamilies.AsNoTracking()
            .Where(item => items.Select(x => x.FamilyId).Contains(item.Id))
            .ToDictionaryAsync(item => item.Id, cancellationToken);
        var vatRateIds = items.SelectMany(item => new[] { item.VatOnTableId, item.VatDirectSellingId })
            .Where(id => id.HasValue).Select(id => id!.Value).Distinct().ToList();
        var vatRates = await _dbContext.ApiVatRates.AsNoTracking()
            .Where(item => vatRateIds.Contains(item.Id))
            .ToDictionaryAsync(item => item.Id, cancellationToken);

        return items.Select(item => Map(item, families.GetValueOrDefault(item.FamilyId), vatRates)).ToList();
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ApiArticleSubfamilies.AnyAsync(item => item.Id == id, cancellationToken);
    }

    public async Task<ArticleSubfamilyResponse?> GetByIdInternalAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.ApiArticleSubfamilies.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null)
        {
            return null;
        }

        var family = await _dbContext.ApiArticleFamilies.AsNoTracking().SingleOrDefaultAsync(x => x.Id == item.FamilyId, cancellationToken);
        var vatRates = await _dbContext.ApiVatRates.AsNoTracking()
            .Where(x => x.Id == item.VatOnTableId || x.Id == item.VatDirectSellingId)
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        return Map(item, family, vatRates);
    }

    public async Task<(bool Success, string? Error, AddEntityIdResponse? Response)> CreateAsync(AddArticleSubfamilyRequest request, CancellationToken cancellationToken = default)
    {
        if (!await _familyService.ExistsAsync(request.FamilyId, cancellationToken))
        {
            return (false, "FamilyId não existe.", null);
        }

        if (request.VatOnTableId.HasValue && !await _vatRateService.ExistsAsync(request.VatOnTableId.Value, cancellationToken))
        {
            return (false, "VatOnTableId não existe.", null);
        }

        if (request.VatDirectSellingId.HasValue && !await _vatRateService.ExistsAsync(request.VatDirectSellingId.Value, cancellationToken))
        {
            return (false, "VatDirectSellingId não existe.", null);
        }

        var count = await _dbContext.ApiArticleSubfamilies.CountAsync(cancellationToken);
        var item = new ApiArticleSubfamily
        {
            Id = Guid.NewGuid(),
            Order = (uint)(count + 1),
            Code = $"SF{count + 1}",
            Designation = request.Designation.Trim(),
            FamilyId = request.FamilyId,
            CommissionGroupId = request.CommissionGroupId,
            DiscountGroupId = request.DiscountGroupId,
            VatOnTableId = request.VatOnTableId,
            VatDirectSellingId = request.VatDirectSellingId,
            ButtonLabel = request.Button?.Label,
            ButtonImage = request.Button?.Image,
            Notes = request.Notes ?? string.Empty,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
        };

        _dbContext.ApiArticleSubfamilies.Add(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return (true, null, new AddEntityIdResponse { Id = item.Id });
    }

    public async Task<(bool Success, string? Error)> UpdateAsync(Guid id, UpdateArticleSubfamilyRequest request, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.ApiArticleSubfamilies.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null)
        {
            return (false, "Subfamily não encontrada.");
        }

        if (!await _familyService.ExistsAsync(request.FamilyId, cancellationToken))
        {
            return (false, "FamilyId não existe.");
        }

        if (request.VatOnTableId.HasValue && !await _vatRateService.ExistsAsync(request.VatOnTableId.Value, cancellationToken))
        {
            return (false, "VatOnTableId não existe.");
        }

        if (request.VatDirectSellingId.HasValue && !await _vatRateService.ExistsAsync(request.VatDirectSellingId.Value, cancellationToken))
        {
            return (false, "VatDirectSellingId não existe.");
        }

        item.Order = request.Order;
        item.Code = string.IsNullOrWhiteSpace(request.Code) ? item.Code : request.Code;
        item.Designation = request.Designation.Trim();
        item.FamilyId = request.FamilyId;
        item.CommissionGroupId = request.CommissionGroupId;
        item.DiscountGroupId = request.DiscountGroupId;
        item.VatOnTableId = request.VatOnTableId;
        item.VatDirectSellingId = request.VatDirectSellingId;
        item.ButtonLabel = request.Button?.Label ?? item.ButtonLabel;
        item.ButtonImage = request.Button?.Image ?? item.ButtonImage;
        item.Notes = request.Notes ?? string.Empty;
        item.IsDeleted = request.IsDeleted;
        item.UpdatedUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.ApiArticleSubfamilies.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null)
        {
            return false;
        }

        _dbContext.ApiArticleSubfamilies.Remove(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static ArticleSubfamilyResponse Map(ApiArticleSubfamily item, ApiArticleFamily? family, Dictionary<Guid, ApiVatRate> vatRates)
    {
        return new ArticleSubfamilyResponse
        {
            Id = item.Id,
            Notes = item.Notes,
            CreatedAt = item.CreatedUtc,
            UpdatedAt = item.UpdatedUtc,
            UpdatedBy = Guid.Empty,
            IsDeleted = item.IsDeleted,
            Order = item.Order,
            Code = item.Code,
            Designation = item.Designation,
            Family = family is null ? null : ArticleFamilyService.Map(family),
            FamilyId = item.FamilyId,
            CommissionGroupId = item.CommissionGroupId,
            DiscountGroupId = item.DiscountGroupId,
            VatOnTableId = item.VatOnTableId,
            VatOnTable = item.VatOnTableId.HasValue && vatRates.TryGetValue(item.VatOnTableId.Value, out var vatOnTable) ? VatRateService.Map(vatOnTable) : null,
            VatDirectSellingId = item.VatDirectSellingId,
            VatDirectSelling = item.VatDirectSellingId.HasValue && vatRates.TryGetValue(item.VatDirectSellingId.Value, out var vatDirect) ? VatRateService.Map(vatDirect) : null,
            Button = item.ButtonLabel is null && item.ButtonImage is null
                ? null
                : new ButtonResponse { Label = item.ButtonLabel, Image = item.ButtonImage }
        };
    }
}
