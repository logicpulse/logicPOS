using LogicPOS.ApiServer.Data;
using LogicPOS.ApiServer.Data.Entities;
using LogicPOS.ApiServer.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.ApiServer.Services;

public sealed class ArticleClassService
{
    private readonly ApplicationDbContext _dbContext;

    public ArticleClassService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ArticleClassResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _dbContext.ApiArticleClasses.AsNoTracking().Where(item => !item.IsDeleted).OrderBy(item => item.Order).ToListAsync(cancellationToken);
        return items.Select(Map).ToList();
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ApiArticleClasses.AnyAsync(item => item.Id == id, cancellationToken);
    }

    public async Task<ArticleClassResponse?> GetByIdInternalAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.ApiArticleClasses.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        return item is null ? null : Map(item);
    }

    public async Task<AddEntityIdResponse> CreateAsync(AddArticleClassRequest request, CancellationToken cancellationToken = default)
    {
        var count = await _dbContext.ApiArticleClasses.CountAsync(cancellationToken);
        var item = new ApiArticleClass
        {
            Id = Guid.NewGuid(),
            Order = (uint)(count + 1),
            Code = $"AC{count + 1}",
            Designation = request.Designation.Trim(),
            Acronym = request.Acronym ?? string.Empty,
            WorkInStock = request.WorkInStock,
            Notes = request.Notes ?? string.Empty,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
        };

        _dbContext.ApiArticleClasses.Add(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new AddEntityIdResponse { Id = item.Id };
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateArticleClassRequest request, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.ApiArticleClasses.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null)
        {
            return false;
        }

        item.Order = request.Order;
        item.Code = string.IsNullOrWhiteSpace(request.Code) ? item.Code : request.Code;
        item.Designation = request.Designation.Trim();
        item.Acronym = request.Acronym ?? string.Empty;
        item.WorkInStock = request.WorkInStock;
        item.Notes = request.Notes ?? string.Empty;
        item.IsDeleted = request.IsDeleted;
        item.UpdatedUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.ApiArticleClasses.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null)
        {
            return false;
        }

        _dbContext.ApiArticleClasses.Remove(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    internal static ArticleClassResponse Map(ApiArticleClass item)
    {
        return new ArticleClassResponse
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
            Acronym = item.Acronym,
            WorkInStock = item.WorkInStock
        };
    }
}
