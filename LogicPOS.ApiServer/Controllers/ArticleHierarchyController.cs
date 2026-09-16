using LogicPOS.ApiServer.DTOs;
using LogicPOS.ApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogicPOS.ApiServer.Controllers;

[AllowAnonymous]
[Route("articles")]
[Route("api/articles")]
public sealed class ArticleHierarchyController : ApiControllerBase
{
    private readonly ArticleHierarchyService _articleHierarchyService;

    public ArticleHierarchyController(ArticleHierarchyService articleHierarchyService)
    {
        _articleHierarchyService = articleHierarchyService;
    }

    [HttpGet("{id:guid}/children")]
    public async Task<IActionResult> GetChildren(Guid id, CancellationToken cancellationToken)
    {
        var (success, error, response) = await _articleHierarchyService.GetChildrenAsync(id, cancellationToken);
        return success ? Ok(response) : NotFound();
    }

    [HttpPost("{id:guid}/children")]
    public async Task<IActionResult> AddChildren(Guid id, [FromBody] SetArticleChildrenRequest request, CancellationToken cancellationToken)
    {
        var (success, field, error, response) = await _articleHierarchyService.AddChildrenAsync(id, request, cancellationToken);
        if (!success)
        {
            return error == "Article não existe." ? NotFound() : BadRequest(new { errors = new[] { new { name = field, reason = error } } });
        }

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{id:guid}/children")]
    public async Task<IActionResult> ReplaceChildren(Guid id, [FromBody] SetArticleChildrenRequest request, CancellationToken cancellationToken)
    {
        var (success, field, error) = await _articleHierarchyService.ReplaceChildrenAsync(id, request, cancellationToken);
        if (!success)
        {
            return error == "Article não existe." ? NotFound() : BadRequest(new { errors = new[] { new { name = field, reason = error } } });
        }

        return NoContent();
    }
}
