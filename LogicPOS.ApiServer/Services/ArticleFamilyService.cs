using LogicPOS.ApiServer.Data;
using LogicPOS.ApiServer.Data.Entities;
using LogicPOS.ApiServer.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.ApiServer.Services;

public sealed class ArticleFamilyService
{
    private readonly ApplicationDbContext _dbContext;

    public ArticleFamilyService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ArticleFamilyResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _dbContext.ApiArticleFamilies.AsNoTracking().Where(item => !item.IsDeleted).OrderBy(item => item.Order).ToListAsync(cancellationToken);
        return items.Select(Map).ToList();
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ApiArticleFamilies.AnyAsync(item => item.Id == id, cancellationToken);
    }

    public async Task<ArticleFamilyResponse?> GetByIdInternalAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.ApiArticleFamilies.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        return item is null ? null : Map(item);
    }

    public async Task<AddEntityIdResponse> CreateAsync(AddArticleFamilyRequest request, CancellationToken cancellationToken = default)
    {
        var count = await _dbContext.ApiArticleFamilies.CountAsync(cancellationToken);
        var item = new ApiArticleFamily
        {
            Id = Guid.NewGuid(),
            Order = (uint)(count + 1),
            Code = $"F{count + 1}",
            Designation = request.Designation.Trim(),
            CommissionGroupId = request.CommissionGroupId,
            DiscountGroupId = request.DiscountGroupId,
            ButtonLabel = request.Button?.Label,
            ButtonImage = request.Button?.Image,
            Notes = request.Notes ?? string.Empty,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
        };

        _dbContext.ApiArticleFamilies.Add(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new AddEntityIdResponse { Id = item.Id };
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateArticleFamilyRequest request, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.ApiArticleFamilies.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null)
        {
            return false;
        }

        item.Order = request.Order ?? item.Order;
        item.Code = string.IsNullOrWhiteSpace(request.Code) ? item.Code : request.Code;
        item.Designation = request.Designation.Trim();
        item.CommissionGroupId = request.CommissionGroupId;
        item.DiscountGroupId = request.DiscountGroupId;
        item.ButtonLabel = request.Button?.Label ?? item.ButtonLabel;
        item.ButtonImage = request.Button?.Image ?? item.ButtonImage;
        item.Notes = request.Notes ?? string.Empty;
        item.IsDeleted = request.IsDeleted ?? item.IsDeleted;
        item.UpdatedUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.ApiArticleFamilies.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null)
        {
            return false;
        }

        _dbContext.ApiArticleFamilies.Remove(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    internal static ArticleFamilyResponse Map(ApiArticleFamily item)
    {
        return new ArticleFamilyResponse
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
            CommissionGroupId = item.CommissionGroupId,
            DiscountGroupId = item.DiscountGroupId,
            Button = item.ButtonLabel is null && item.ButtonImage is null
                ? null
                : new ButtonResponse { Label = item.ButtonLabel, Image = item.ButtonImage }
        };
    }
}
