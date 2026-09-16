using LogicPOS.ApiServer.DTOs;
using LogicPOS.ApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogicPOS.ApiServer.Controllers;

[AllowAnonymous]
[Route("articles")]
[Route("api/articles")]
public sealed class ArticlesController : ApiControllerBase
{
    private readonly ArticleService _articleService;

    public ArticlesController(ArticleService articleService)
    {
        _articleService = articleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] string? search,
        [FromQuery] Guid? familyId,
        [FromQuery] Guid? subfamilyId,
        [FromQuery] bool? favorite,
        [FromQuery] bool? includeDeleted,
        CancellationToken cancellationToken)
    {
        return Ok(await _articleService.GetPagedAsync(page ?? 1, pageSize ?? 25, search, familyId, subfamilyId, favorite, includeDeleted, cancellationToken));
    }

    [HttpGet("code/{code}")]
    public async Task<IActionResult> GetByCode(string code, CancellationToken cancellationToken)
    {
        var article = await _articleService.GetByCodeAsync(code, cancellationToken);
        return article is null ? NotFound() : Ok(article);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var article = await _articleService.GetByIdAsync(id, cancellationToken);
        return article is null ? NotFound() : Ok(article);
    }

    [HttpGet("{id:guid}/view")]
    public async Task<IActionResult> GetViewModel(Guid id, CancellationToken cancellationToken)
    {
        var article = await _articleService.GetViewModelAsync(id, cancellationToken);
        return article is null ? NotFound() : Ok(article);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AddArticleRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Designation))
        {
            return BadRequest(new { errors = new[] { new { name = "designation", reason = "Designation é obrigatório." } } });
        }

        var (success, field, error, response) = await _articleService.CreateAsync(request, cancellationToken);
        if (!success)
        {
            return BadRequest(new { errors = new[] { new { name = field, reason = error } } });
        }

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateArticleRequest request, CancellationToken cancellationToken)
    {
        var (success, field, error) = await _articleService.UpdateAsync(id, request, cancellationToken);
        if (!success)
        {
            return error == "Article não encontrado." ? NotFound() : BadRequest(new { errors = new[] { new { name = field, reason = error } } });
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _articleService.DeleteAsync(id, cancellationToken);
        return deleted ? Ok() : NotFound();
    }
}
