using LogicPOS.ApiServer.Data;
using LogicPOS.ApiServer.Data.Entities;
using LogicPOS.ApiServer.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.ApiServer.Services;

public sealed class ArticleHierarchyService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ArticleService _articleService;

    public ArticleHierarchyService(ApplicationDbContext dbContext, ArticleService articleService)
    {
        _dbContext = dbContext;
        _articleService = articleService;
    }

    public async Task<(bool Success, string? Error, IReadOnlyList<ArticleChildResponse>? Response)> GetChildrenAsync(Guid articleId, CancellationToken cancellationToken = default)
    {
        if (!await _dbContext.ApiArticles.AnyAsync(a => a.Id == articleId, cancellationToken))
        {
            return (false, "Article não existe.", null);
        }

        var links = await _dbContext.ApiArticleChildren.AsNoTracking().Where(item => item.ParentArticleId == articleId).ToListAsync(cancellationToken);

        var result = new List<ArticleChildResponse>();
        foreach (var link in links)
        {
            var childArticle = await _articleService.GetByIdAsync(link.ChildArticleId, cancellationToken);
            result.Add(new ArticleChildResponse { Article = childArticle, Quantity = link.Quantity });
        }

        return (true, null, result);
    }

    public async Task<(bool Success, string? Field, string? Error, AddEntityIdResponse? Response)> AddChildrenAsync(Guid articleId, SetArticleChildrenRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await ValidateAsync(articleId, request, cancellationToken);
        if (validation is not null)
        {
            return (false, validation.Value.Field, validation.Value.Error, null);
        }

        foreach (var child in request.Children)
        {
            var existing = await _dbContext.ApiArticleChildren.SingleOrDefaultAsync(
                item => item.ParentArticleId == articleId && item.ChildArticleId == child.ArticleId, cancellationToken);

            if (existing is not null)
            {
                existing.Quantity += child.Quantity;
            }
            else
            {
                _dbContext.ApiArticleChildren.Add(new ApiArticleChild
                {
                    Id = Guid.NewGuid(),
                    ParentArticleId = articleId,
                    ChildArticleId = child.ArticleId,
                    Quantity = child.Quantity
                });
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return (true, null, null, new AddEntityIdResponse { Id = articleId });
    }

    public async Task<(bool Success, string? Field, string? Error)> ReplaceChildrenAsync(Guid articleId, SetArticleChildrenRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await ValidateAsync(articleId, request, cancellationToken);
        if (validation is not null)
        {
            return (false, validation.Value.Field, validation.Value.Error);
        }

        var existingLinks = await _dbContext.ApiArticleChildren.Where(item => item.ParentArticleId == articleId).ToListAsync(cancellationToken);
        _dbContext.ApiArticleChildren.RemoveRange(existingLinks);

        foreach (var child in request.Children)
        {
            _dbContext.ApiArticleChildren.Add(new ApiArticleChild
            {
                Id = Guid.NewGuid(),
                ParentArticleId = articleId,
                ChildArticleId = child.ArticleId,
                Quantity = child.Quantity
            });
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return (true, null, null);
    }

    private async Task<(string Field, string Error)?> ValidateAsync(Guid articleId, SetArticleChildrenRequest request, CancellationToken cancellationToken)
    {
        if (!await _dbContext.ApiArticles.AnyAsync(a => a.Id == articleId, cancellationToken))
        {
            return ("id", "Article não existe.");
        }

        foreach (var child in request.Children)
        {
            if (child.ArticleId == articleId)
            {
                return ("children", "Um artigo não pode ser filho de si próprio.");
            }

            if (!await _dbContext.ApiArticles.AnyAsync(a => a.Id == child.ArticleId, cancellationToken))
            {
                return ("children", $"ArticleId {child.ArticleId} não existe.");
            }
        }

        return null;
    }
}
