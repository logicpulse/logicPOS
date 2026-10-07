using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Errors;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;


internal class ArticleCompositionCreator : EntityCreator<ArticleComposition>
{
    private readonly CreateArticleCompositionDto _dto;
    private readonly IArticleCompositionRepository _compositionRepository;
    private readonly IArticleRepository _articleRepository;

    public ArticleCompositionCreator(CreateArticleCompositionDto dto,
                                     IArticleCompositionRepository compositionRepository,
                                     IArticleRepository articleRepository) :
        base( new())
    {
        _dto = dto;
        _compositionRepository = compositionRepository;
        _articleRepository = articleRepository;
    }

    protected override Task AssignDataAsync(CancellationToken ct = default)
    {
        _entity.ChildId = _dto.ArticleChildId;
        _entity.ParentId = _dto.ArticleId;
        _entity.Quantity = _dto.Quantity;

        return Task.CompletedTask;
    }

    protected override async Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        var compositionExists = await _compositionRepository.CompositionExistsAsync(
            _dto.ArticleId,
            _dto.ArticleChildId,
            ct);

        if (compositionExists)
        {
            return Result.Failure(
                Error.Conflict(
                $"Composição já existe para o artigo {_dto.ArticleId} com filho {_dto.ArticleChildId}"));
        }

        return Result.Success();
    }

    protected override async Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        var articleExists = await _articleRepository.ExistsAsync(_dto.ArticleId, ct);

        if (!articleExists)
        {
            return Result.NotFound(nameof(Article), _dto.ArticleId.ToString());
        }

        var articleChildExists = await _articleRepository.ExistsAsync(_dto.ArticleChildId, ct);

        if (!articleChildExists)
        {
            return Result.NotFound(nameof(Article), _dto.ArticleChildId.ToString());
        }

        return Result.Success();
    }
}